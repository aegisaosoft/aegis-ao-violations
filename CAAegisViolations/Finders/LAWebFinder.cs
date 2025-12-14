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
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AegisViolationsAPI;
using HuurApi.Models;

namespace CAAegisViolations.Finders
{
    /// <summary>
    /// Los Angeles Parking Citations finder with real payment status
    /// Uses hybrid approach:
    /// 1. Socrata API to find citations by license plate
    /// 2. LADOT payment portal to check real payment status
    /// </summary>
    public class LAWebFinder : IAegisAPIFinder
    {
        private readonly HttpClient _socrataClient;
        private readonly HttpClient _webClient;
        private readonly CookieContainer _cookies;
        private readonly string _appToken = "9gGE7tnFW9yyAh8GxW12veILS";
        private bool _disposed;

        // Socrata API
        private const string SocrataBaseUrl = "https://data.lacity.org/resource";
        private const string DatasetId = "4f5p-udkv";

        // Payment portal
        private const string PaymentBaseUrl = "https://wmq.etimspayments.com";
        private const string PaymentInputPage = "/pbw/include/la/input.jsp";
        private const string PaymentSearchUrl = "/pbw/inputAction.doh";

        public string Link { get; set; } = "https://data.lacity.org/Transportation/Parking-Citations/4f5p-udkv";
        public string Name { get; set; } = "Los Angeles LADOT";
        public string State { get; set; } = "CA";

        public event EventHandler<FinderErrorEventArgs>? Error;

        public LAWebFinder()
        {
            // Socrata client
            _socrataClient = new HttpClient
            {
                BaseAddress = new Uri(SocrataBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _socrataClient.DefaultRequestHeaders.Add("X-App-Token", _appToken);
            _socrataClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Web scraping client
            _cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                UseCookies = true,
                AllowAutoRedirect = true
            };

            _webClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(PaymentBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _webClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _webClient.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
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

                // Step 1: Get citations from Socrata API
                var citationsFromApi = await GetCitationsFromSocrata(normalizedPlate, normalizedState);

                if (citationsFromApi.Count == 0)
                    return violations;

                // Step 2: Check payment status for each citation via web portal
                foreach (var citation in citationsFromApi)
                {
                    var paymentInfo = await CheckPaymentStatus(citation.TicketNumber);
                    
                    var violation = MapToViolation(citation, paymentInfo, normalizedPlate, normalizedState);
                    violations.Add(violation);

                    // Small delay to avoid rate limiting
                    await Task.Delay(200);
                }
            }
            catch (Exception ex)
            {
                RaiseError(licensePlate, state ?? "CA", ex, $"Error: {ex.Message}");
            }

            return violations;
        }

        private async Task<List<LaApiResponse>> GetCitationsFromSocrata(string plate, string state)
        {
            var citations = new List<LaApiResponse>();

            try
            {
                var url = $"/{DatasetId}.json?" +
                          $"$where=upper(plate)='{plate}' AND upper(state)='{state}'" +
                          $"&$limit=100" +
                          $"&$order=issue_date DESC";

                var response = await _socrataClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<LaApiResponse>>();
                    if (result != null)
                        citations = result;
                }
            }
            catch (Exception ex)
            {
                RaiseError(plate, state, ex, $"Socrata API error: {ex.Message}");
            }

            return citations;
        }

        private async Task<PaymentStatusInfo> CheckPaymentStatus(string? citationNumber)
        {
            var info = new PaymentStatusInfo
            {
                Status = PaymentStatusType.Unknown,
                AmountDue = null
            };

            if (string.IsNullOrEmpty(citationNumber))
                return info;

            try
            {
                // Get input page first to establish session
                await _webClient.GetAsync(PaymentInputPage);

                // Submit citation number search
                var formData = new Dictionary<string, string>
                {
                    { "citationNumber", citationNumber },
                    { "siteId", "la" }
                };

                var content = new FormUrlEncodedContent(formData);
                _webClient.DefaultRequestHeaders.Referrer = new Uri(PaymentBaseUrl + PaymentInputPage);

                var response = await _webClient.PostAsync(PaymentSearchUrl, content);
                var html = await response.Content.ReadAsStringAsync();

                info = ParsePaymentStatus(html, citationNumber);
            }
            catch
            {
                // If web check fails, return unknown status
                info.Status = PaymentStatusType.Unknown;
            }

            return info;
        }

        private PaymentStatusInfo ParsePaymentStatus(string html, string citationNumber)
        {
            var info = new PaymentStatusInfo
            {
                Status = PaymentStatusType.Unknown,
                AmountDue = null
            };

            // Check for "citation not found" or "paid" messages
            var lowerHtml = html.ToLowerInvariant();

            if (lowerHtml.Contains("not found") || 
                lowerHtml.Contains("no citation") ||
                lowerHtml.Contains("cannot be found"))
            {
                // Citation not in system - might be old/paid
                info.Status = PaymentStatusType.NotFound;
                return info;
            }

            if (lowerHtml.Contains("paid in full") ||
                lowerHtml.Contains("this citation has been paid") ||
                lowerHtml.Contains("balance: $0") ||
                lowerHtml.Contains("amount due: $0"))
            {
                info.Status = PaymentStatusType.Paid;
                info.AmountDue = 0;
                return info;
            }

            if (lowerHtml.Contains("dismissed") ||
                lowerHtml.Contains("voided"))
            {
                info.Status = PaymentStatusType.Dismissed;
                info.AmountDue = 0;
                return info;
            }

            if (lowerHtml.Contains("hearing") ||
                lowerHtml.Contains("under review") ||
                lowerHtml.Contains("contested"))
            {
                info.Status = PaymentStatusType.Disputed;
                return info;
            }

            // Try to extract amount due
            var amountPattern = @"(?:amount\s*due|balance|total)[^\$]*\$\s*([0-9,.]+)";
            var amountMatch = Regex.Match(html, amountPattern, RegexOptions.IgnoreCase);

            if (amountMatch.Success)
            {
                var amountStr = amountMatch.Groups[1].Value.Replace(",", "");
                if (decimal.TryParse(amountStr, out var amount))
                {
                    info.AmountDue = amount;
                    info.Status = amount > 0 ? PaymentStatusType.Unpaid : PaymentStatusType.Paid;
                }
            }

            // If we found the citation but couldn't determine status, assume unpaid
            if (html.Contains(citationNumber) && info.Status == PaymentStatusType.Unknown)
            {
                info.Status = PaymentStatusType.Unpaid;
            }

            return info;
        }

        private ParkingViolation MapToViolation(LaApiResponse citation, PaymentStatusInfo paymentInfo, 
            string plate, string state)
        {
            var fineAmount = ParseDecimal(citation.FineAmount);
            var amountDue = paymentInfo.AmountDue ?? fineAmount;

            return new ParkingViolation
            {
                Id = citation.TicketNumber,
                CitationNumber = citation.TicketNumber,
                NoticeNumber = citation.TicketNumber,
                Provider = 2, // LA LADOT
                Agency = citation.Agency ?? "LADOT",
                Address = BuildAddress(citation),
                Tag = plate,
                State = state,
                IssueDate = ParseIssueDateTime(citation.IssueDate, citation.IssueTime),
                Amount = amountDue,
                Currency = "USD",
                PaymentStatus = MapPaymentStatus(paymentInfo.Status, amountDue, fineAmount),
                FineType = 0, // Parking
                Note = BuildNote(citation, paymentInfo),
                Link = "https://wmq.etimspayments.com/pbw/include/la/input.jsp",
                IsActive = paymentInfo.Status == PaymentStatusType.Unpaid || 
                          paymentInfo.Status == PaymentStatusType.Unknown
            };
        }

        private static int MapPaymentStatus(PaymentStatusType status, decimal amountDue, decimal originalFine)
        {
            return status switch
            {
                PaymentStatusType.Paid => Const.P_PAID,      // P_PAID
                PaymentStatusType.Dismissed => Const.P_PAID, // Treat as paid (no amount due)
                PaymentStatusType.Disputed => Const.P_DISPUTED,  // P_DISPUTED
                PaymentStatusType.Unpaid => Const.P_NEW,    // P_NEW
                PaymentStatusType.NotFound => amountDue > 0 ? Const.P_NEW : Const.P_PAID, // Check amount
                _ => 0 // Unknown = assume unpaid
            };
        }

        private static string BuildAddress(LaApiResponse citation)
        {
            if (!string.IsNullOrEmpty(citation.Location))
                return citation.Location + ", Los Angeles, CA";

            if (!string.IsNullOrEmpty(citation.StreetName))
                return citation.StreetName + ", Los Angeles, CA";

            return "Los Angeles, CA";
        }

        private static string BuildNote(LaApiResponse citation, PaymentStatusInfo paymentInfo)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(citation.ViolationCode))
                parts.Add($"Code: {citation.ViolationCode}");

            if (!string.IsNullOrEmpty(citation.ViolationDescription))
                parts.Add($"Violation: {citation.ViolationDescription}");

            if (!string.IsNullOrEmpty(citation.Make))
                parts.Add($"Vehicle: {citation.Make}");

            // Add payment status info
            if (paymentInfo.Status != PaymentStatusType.Unknown)
                parts.Add($"Status: {paymentInfo.Status}");

            return string.Join(" | ", parts);
        }

        private static DateTime? ParseIssueDateTime(string? date, string? time)
        {
            if (string.IsNullOrWhiteSpace(date))
                return null;

            if (!DateTime.TryParse(date, out var result))
            {
                if (!DateTime.TryParseExact(date, "MM/dd/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out result))
                    return null;
            }

            // Add time if available (format typically HHMM like "1430")
            if (!string.IsNullOrWhiteSpace(time) && time.Length >= 4)
            {
                if (int.TryParse(time.Substring(0, 2), out var hours) &&
                    int.TryParse(time.Substring(2, 2), out var minutes))
                {
                    result = result.AddHours(hours).AddMinutes(minutes);
                }
            }

            return result;
        }

        private static decimal ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            var cleaned = Regex.Replace(value, @"[^0-9.]", "");
            return decimal.TryParse(cleaned, out var result) ? result : 0;
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
                _socrataClient?.Dispose();
                _webClient?.Dispose();
                _disposed = true;
            }
        }
    }

    internal enum PaymentStatusType
    {
        Unknown,
        Paid,
        Unpaid,
        Disputed,
        Dismissed,
        NotFound
    }

    internal class PaymentStatusInfo
    {
        public PaymentStatusType Status { get; set; }
        public decimal? AmountDue { get; set; }
    }

    /// <summary>
    /// Response model for LA Socrata API
    /// </summary>
    public class LaApiResponse
    {
        [JsonPropertyName("ticket_number")]
        public string? TicketNumber { get; set; }

        [JsonPropertyName("issue_date")]
        public string? IssueDate { get; set; }

        [JsonPropertyName("issue_time")]
        public string? IssueTime { get; set; }

        [JsonPropertyName("plate")]
        public string? Plate { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("violation_code")]
        public string? ViolationCode { get; set; }

        [JsonPropertyName("violation_description")]
        public string? ViolationDescription { get; set; }

        [JsonPropertyName("fine_amount")]
        public string? FineAmount { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("street_name")]
        public string? StreetName { get; set; }

        [JsonPropertyName("meter_id")]
        public string? MeterId { get; set; }

        [JsonPropertyName("agency")]
        public string? Agency { get; set; }

        [JsonPropertyName("latitude")]
        public string? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public string? Longitude { get; set; }
    }
}
