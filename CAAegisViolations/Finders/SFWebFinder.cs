/*
 *
 * Copyright (c) 2025 Alexander Orlov.
 * 34 Middletown Ave Atlantic Highlands NJ 07716
 *
 * THIS SOFTWARE IS THE CONFIDENTIAL AND PROPRIETARY INFORMATION OF
 * Alexander Orlov. ("CONFIDENTIAL INFORMATION"). YOU SHALL NOT DISCLOSE
 * SUCH CONFIDENTIAL INFORMATION AND SHALL USE IT ONLY IN ACCORDANCE
 * WITH THE TERMS OF THE LICENSE AGREEMENT YOU ENTERED INTO WITH
 * Alexander Orlov.
 *
 * Author: Alexander Orlov
 *
 */

using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AegisViolationsAPI;
using HuurApi.Models;

namespace CAAegisViolations.Finders
{
    /// <summary>
    /// San Francisco Parking Citations finder using SFMTA payment portal web scraping
    /// Supports search by license plate with real payment status
    /// Portal: https://wmq.etimspayments.com/pbw/include/sanfrancisco/
    /// </summary>
    public class SFWebFinder : IAegisAPIFinder
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookies;
        private bool _disposed;

        private const string BaseUrl = "https://wmq.etimspayments.com";
        private const string InputPage = "/pbw/include/sanfrancisco/input.jsp";
        private const string SearchUrl = "/pbw/inputAction.doh";

        public string Link { get; set; } = "https://wmq.etimspayments.com/pbw/include/sanfrancisco/main.jsp";
        public string Name { get; set; } = "San Francisco SFMTA";
        public string State { get; set; } = "CA";

        public event EventHandler<FinderErrorEventArgs>? Error;

        public SFWebFinder()
        {
            _cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                UseCookies = true,
                AllowAutoRedirect = true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", 
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        }

        public async Task<List<ParkingViolation>> Find(string licensePlate, string state)
        {
            var violations = new List<ParkingViolation>();

            try
            {
                if (string.IsNullOrWhiteSpace(licensePlate))
                    return violations;

                var normalizedPlate = licensePlate.Trim().ToUpperInvariant().Replace(" ", "");
                var normalizedState = state?.Trim().ToUpperInvariant() ?? "CA";

                // Step 1: Get the input page to establish session and get any hidden fields
                var inputPageResponse = await _httpClient.GetAsync(InputPage);
                if (!inputPageResponse.IsSuccessStatusCode)
                {
                    RaiseError(licensePlate, normalizedState, 
                        new HttpRequestException($"Failed to load input page: {inputPageResponse.StatusCode}"),
                        "Failed to load SF citation search page");
                    return violations;
                }

                var inputPageHtml = await inputPageResponse.Content.ReadAsStringAsync();

                // Step 2: Extract any hidden form fields (CSRF tokens, etc.)
                var hiddenFields = ExtractHiddenFields(inputPageHtml);

                // Step 3: Submit search by license plate
                var formData = new Dictionary<string, string>
                {
                    { "searchby", "L" }, // L = License plate, C = Citation number
                    { "plateState", normalizedState },
                    { "plateNumber", normalizedPlate },
                    { "siteId", "sanfrancisco" }
                };

                // Add any extracted hidden fields
                foreach (var field in hiddenFields)
                {
                    if (!formData.ContainsKey(field.Key))
                        formData[field.Key] = field.Value;
                }

                var content = new FormUrlEncodedContent(formData);
                
                // Set referrer
                _httpClient.DefaultRequestHeaders.Referrer = new Uri(BaseUrl + InputPage);

                var searchResponse = await _httpClient.PostAsync(SearchUrl, content);
                var responseHtml = await searchResponse.Content.ReadAsStringAsync();

                // Step 4: Parse the results
                violations = ParseSearchResults(responseHtml, normalizedPlate, normalizedState);
            }
            catch (HttpRequestException ex)
            {
                RaiseError(licensePlate, state ?? "CA", ex, $"HTTP request failed: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                RaiseError(licensePlate, state ?? "CA", ex, "Request timed out");
            }
            catch (Exception ex)
            {
                RaiseError(licensePlate, state ?? "CA", ex, $"Unexpected error: {ex.Message}");
            }

            return violations;
        }

        private Dictionary<string, string> ExtractHiddenFields(string html)
        {
            var fields = new Dictionary<string, string>();

            // Match hidden input fields
            var pattern = @"<input[^>]*type=[""']hidden[""'][^>]*name=[""']([^""']+)[""'][^>]*value=[""']([^""']*)[""'][^>]*>";
            var matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 3)
                {
                    var name = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    fields[name] = value;
                }
            }

            // Also check for reversed attribute order
            pattern = @"<input[^>]*name=[""']([^""']+)[""'][^>]*value=[""']([^""']*)[""'][^>]*type=[""']hidden[""'][^>]*>";
            matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 3)
                {
                    var name = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    if (!fields.ContainsKey(name))
                        fields[name] = value;
                }
            }

            return fields;
        }

        private List<ParkingViolation> ParseSearchResults(string html, string plate, string state)
        {
            var violations = new List<ParkingViolation>();

            // Check for "no citations found" message
            if (html.Contains("No citations found", StringComparison.OrdinalIgnoreCase) ||
                html.Contains("no outstanding", StringComparison.OrdinalIgnoreCase) ||
                html.Contains("0 citation", StringComparison.OrdinalIgnoreCase))
            {
                return violations;
            }

            // Parse citation table rows
            // SF typically shows results in a table with columns:
            // Citation Number | Issue Date | Violation | Fine Amount | Due | Status

            // Pattern to match table rows with citation data
            var rowPattern = @"<tr[^>]*>.*?<td[^>]*>([^<]*)</td>.*?<td[^>]*>([^<]*)</td>.*?<td[^>]*>([^<]*)</td>.*?<td[^>]*>\$?([0-9,.]+)</td>.*?<td[^>]*>\$?([0-9,.]+)</td>.*?</tr>";
            var matches = Regex.Matches(html, rowPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                try
                {
                    var citationNumber = match.Groups[1].Value.Trim();
                    var issueDateStr = match.Groups[2].Value.Trim();
                    var violationDesc = match.Groups[3].Value.Trim();
                    var fineAmountStr = match.Groups[4].Value.Trim();
                    var amountDueStr = match.Groups[5].Value.Trim();

                    // Skip if doesn't look like a citation number
                    if (string.IsNullOrEmpty(citationNumber) || citationNumber.Length < 5)
                        continue;

                    var fineAmount = ParseDecimal(fineAmountStr);
                    var amountDue = ParseDecimal(amountDueStr);

                    var violation = new ParkingViolation
                    {
                        Id = citationNumber,
                        CitationNumber = citationNumber,
                        NoticeNumber = citationNumber,
                        Provider = 1, // SF SFMTA
                        Agency = "SFMTA",
                        Address = "San Francisco, CA",
                        Tag = plate,
                        State = state,
                        IssueDate = ParseDate(issueDateStr),
                        Amount = amountDue > 0 ? amountDue : fineAmount,
                        Currency = "USD",
                        PaymentStatus = DeterminePaymentStatus(amountDue, fineAmount),
                        FineType = 0, // Parking
                        Note = StripHtml(violationDesc),
                        Link = Link,
                        IsActive = amountDue > 0
                    };

                    violations.Add(violation);
                }
                catch
                {
                    // Skip malformed rows
                }
            }

            // Alternative parsing if standard table pattern doesn't match
            if (violations.Count == 0)
            {
                violations = ParseAlternativeFormat(html, plate, state);
            }

            return violations;
        }

        private List<ParkingViolation> ParseAlternativeFormat(string html, string plate, string state)
        {
            var violations = new List<ParkingViolation>();

            // Try to find citation numbers and amounts in different format
            var citationPattern = @"citation[^0-9]*(\d{8,12})";
            var amountPattern = @"\$\s*([0-9,.]+)";

            var citationMatches = Regex.Matches(html, citationPattern, RegexOptions.IgnoreCase);
            var amountMatches = Regex.Matches(html, amountPattern, RegexOptions.IgnoreCase);

            // Simple pairing - may need adjustment based on actual page structure
            for (int i = 0; i < citationMatches.Count && i < amountMatches.Count; i++)
            {
                var citationNumber = citationMatches[i].Groups[1].Value;
                var amount = ParseDecimal(amountMatches[i].Groups[1].Value);

                if (!string.IsNullOrEmpty(citationNumber))
                {
                    violations.Add(new ParkingViolation
                    {
                        Id = citationNumber,
                        CitationNumber = citationNumber,
                        NoticeNumber = citationNumber,
                        Provider = 1,
                        Agency = "SFMTA",
                        Address = "San Francisco, CA",
                        Tag = plate,
                        State = state,
                        Amount = amount,
                        Currency = "USD",
                        PaymentStatus = amount > 0 ? 0 : 1, // 0 = unpaid, 1 = paid
                        FineType = 0,
                        Link = Link,
                        IsActive = amount > 0
                    });
                }
            }

            return violations;
        }

        private static int DeterminePaymentStatus(decimal amountDue, decimal originalFine)
        {
            if (amountDue == 0)
                return Const.P_PAID; // Paid

            if (amountDue < originalFine)
                return Const.P_PARTIAL; // Partial payment

            return Const.P_PAID; // Unpaid/New
        }

        private static decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            // Remove currency symbols and commas
            var cleaned = Regex.Replace(value, @"[^0-9.]", "");
            return decimal.TryParse(cleaned, out var result) ? result : 0;
        }

        private static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, out var result))
                return result;

            // Try common formats
            string[] formats = { "MM/dd/yyyy", "M/d/yyyy", "yyyy-MM-dd", "MM-dd-yyyy" };
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, null, 
                    System.Globalization.DateTimeStyles.None, out result))
                    return result;
            }

            return null;
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            return Regex.Replace(html, "<[^>]*>", " ").Trim();
        }

        private void RaiseError(string licensePlate, string state, Exception exception, string message)
        {
            Error?.Invoke(this, new FinderErrorEventArgs
            {
                FinderName = Name,
                LicensePlate = licensePlate,
                State = state,
                Exception = exception,
                Message = message
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}
