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
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HuurApi.Models;

namespace AegisViolationsAPI.Finders;

/// <summary>
/// Provider ID for OnlineServicesHub/Duncan Solutions portals
/// </summary>
public static class OnlineServicesHubProvider
{
    /// <summary>
    /// Philadelphia Parking Authority
    /// </summary>
    public const int Philadelphia = 100;
    
    // Add more cities as needed
    // public const int AnotherCity = 101;
}

/// <summary>
/// Finder for OnlineServicesHub parking portals (Duncan Solutions platform)
/// Supports Philadelphia and other cities using this platform
/// </summary>
public class OnlineServicesHubFinder : IAegisAPIFinder, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly CookieContainer _cookies;
    private readonly string _baseUrl;
    private readonly string _city;
    private readonly int _providerId;
    private string? _verificationToken;
    private bool _isInitialized;
    private bool _disposed;

    private static readonly Dictionary<string, (string Url, int ProviderId)> KnownPortals = new()
    {
        ["philadelphia"] = ("https://onlineserviceshub.com/ParkingPortal/Philadelphia", OnlineServicesHubProvider.Philadelphia),
        // Add more cities as discovered
    };

    private static readonly Dictionary<string, string> StateMap = new()
    {
        ["AL"] = "Alabama", ["AK"] = "Alaska", ["AZ"] = "Arizona", ["AR"] = "Arkansas",
        ["CA"] = "California", ["CO"] = "Colorado", ["CT"] = "Connecticut", ["DE"] = "Delaware",
        ["DC"] = "District of Columbia", ["FL"] = "Florida", ["GA"] = "Georgia", ["HI"] = "Hawaii",
        ["ID"] = "Idaho", ["IL"] = "Illinois", ["IN"] = "Indiana", ["IA"] = "Iowa",
        ["KS"] = "Kansas", ["KY"] = "Kentucky", ["LA"] = "Louisiana", ["ME"] = "Maine",
        ["MD"] = "Maryland", ["MA"] = "Massachusetts", ["MI"] = "Michigan", ["MN"] = "Minnesota",
        ["MS"] = "Mississippi", ["MO"] = "Missouri", ["MT"] = "Montana", ["NE"] = "Nebraska",
        ["NV"] = "Nevada", ["NH"] = "New Hampshire", ["NJ"] = "New Jersey", ["NM"] = "New Mexico",
        ["NY"] = "New York", ["NC"] = "North Carolina", ["ND"] = "North Dakota", ["OH"] = "Ohio",
        ["OK"] = "Oklahoma", ["OR"] = "Oregon", ["PA"] = "Pennsylvania", ["RI"] = "Rhode Island",
        ["SC"] = "South Carolina", ["SD"] = "South Dakota", ["TN"] = "Tennessee", ["TX"] = "Texas",
        ["UT"] = "Utah", ["VT"] = "Vermont", ["VA"] = "Virginia", ["WA"] = "Washington",
        ["WV"] = "West Virginia", ["WI"] = "Wisconsin", ["WY"] = "Wyoming"
    };

    /// <inheritdoc />
    public string Link => _baseUrl;

    public string Name => "Philadelphia Parking Authority";

    public string State => "PA";

    /// <inheritdoc />
    public event EventHandler<FinderErrorEventArgs>? Error;

    /// <summary>
    /// Creates a new OnlineServicesHub finder for the specified city
    /// </summary>
    /// <param name="city">City name (e.g., "philadelphia")</param>
    public OnlineServicesHubFinder(string city = "philadelphia")
    {
        _city = city.ToLower();

        if (!KnownPortals.TryGetValue(_city, out var portalInfo))
            throw new ArgumentException($"Unknown city: {city}. Available: {string.Join(", ", KnownPortals.Keys)}");

        _baseUrl = portalInfo.Url;
        _providerId = portalInfo.ProviderId;
        _cookies = new CookieContainer();

        var handler = new HttpClientHandler
        {
            CookieContainer = _cookies,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true
        };

        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
    }

    /// <inheritdoc />
    public async Task<List<ParkingViolation>> Find(string licensePlate, string state)
    {
        var violations = new List<ParkingViolation>();

        try
        {
            await InitializeAsync();

            // Try API approach first
            var apiResult = await TryApiSearchAsync(licensePlate, state);
            if (apiResult != null && apiResult.Count > 0)
                return apiResult;

            // Fallback to form submission
            var formResult = await FormSearchAsync(licensePlate, state);
            if (formResult != null)
                violations.AddRange(formResult);
        }
        catch (Exception ex)
        {
            OnError(licensePlate, state, ex);
        }

        return violations;
    }

    /// <summary>
    /// Initialize session - get cookies and verification token
    /// </summary>
    private async Task InitializeAsync()
    {
        if (_isInitialized) return;

        var response = await _httpClient.GetAsync(_baseUrl);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        // Extract anti-forgery token
        var tokenMatch = Regex.Match(html, @"__RequestVerificationToken['""]?\s*(?:value|:)\s*['""]([^'""]+)['""]");
        if (tokenMatch.Success)
        {
            _verificationToken = tokenMatch.Groups[1].Value;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var tokenInput = doc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']");
        if (tokenInput != null)
        {
            _verificationToken = tokenInput.GetAttributeValue("value", _verificationToken);
        }

        _isInitialized = true;
    }

    /// <summary>
    /// Try to call the underlying API directly
    /// </summary>
    private async Task<List<ParkingViolation>?> TryApiSearchAsync(string licensePlate, string state)
    {
        var apiEndpoints = new[]
        {
            $"{_baseUrl}/api/Ticket/Search",
            $"{_baseUrl}/Ticket/Search",
            $"{_baseUrl}/api/Citation/Search",
        };

        var requestBody = new
        {
            LicensePlate = licensePlate,
            PlateState = state,
            State = StateMap.GetValueOrDefault(state.ToUpper(), state),
            SearchType = "Plate"
        };

        foreach (var endpoint in apiEndpoints)
        {
            try
            {
                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestBody),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                if (!string.IsNullOrEmpty(_verificationToken))
                {
                    _httpClient.DefaultRequestHeaders.Remove("RequestVerificationToken");
                    _httpClient.DefaultRequestHeaders.Add("RequestVerificationToken", _verificationToken);
                }

                var response = await _httpClient.PostAsync(endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return ParseApiResponse(json, licensePlate, state);
                }
            }
            catch
            {
                // Try next endpoint
            }
        }

        return null;
    }

    /// <summary>
    /// Parse JSON API response into ParkingViolation list
    /// </summary>
    private List<ParkingViolation>? ParseApiResponse(string json, string licensePlate, string state)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            var violations = new List<ParkingViolation>();
            System.Text.Json.JsonElement dataElement;

            // Find data array in response
            if (root.TryGetProperty("data", out dataElement) ||
                root.TryGetProperty("Data", out dataElement) ||
                root.TryGetProperty("results", out dataElement) ||
                root.TryGetProperty("Results", out dataElement) ||
                root.TryGetProperty("tickets", out dataElement) ||
                root.TryGetProperty("Tickets", out dataElement))
            {
                // Data in nested property
            }
            else if (root.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                dataElement = root;
            }
            else
            {
                // Single citation
                var single = ParseViolationFromJson(root, licensePlate, state);
                if (single != null) violations.Add(single);
                return violations;
            }

            if (dataElement.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var item in dataElement.EnumerateArray())
                {
                    var violation = ParseViolationFromJson(item, licensePlate, state);
                    if (violation != null)
                        violations.Add(violation);
                }
            }

            return violations;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Parse single violation from JSON element
    /// </summary>
    private ParkingViolation? ParseViolationFromJson(System.Text.Json.JsonElement element, string licensePlate, string state)
    {
        var citationNumber = GetJsonString(element, "citationNumber", "CitationNumber", "ticketNumber", "TicketNumber", "number", "Number");
        if (string.IsNullOrEmpty(citationNumber))
            return null;

        var violation = new ParkingViolation
        {
            CitationNumber = citationNumber,
            Tag = licensePlate,
            State = state,
            Provider = _providerId,
            Agency = GetAgencyName(),
            Address = GetJsonString(element, "location", "Location", "address", "Address"),
            Note = GetJsonString(element, "violationDescription", "ViolationDescription", "violation", "Violation", "description", "Description"),
            Link = _baseUrl,
            Currency = "USD",
            IsActive = true,
            FineType = 1 // Parking violation
        };

        // Parse amount
        var totalDue = GetJsonDecimal(element, "totalDue", "TotalDue", "total", "Total", "amountDue", "AmountDue", "balance", "Balance");
        violation.Amount = totalDue ?? 0;

        // Parse date
        violation.IssueDate = GetJsonDate(element, "issueDate", "IssueDate", "date", "Date", "violationDate", "ViolationDate");

        // Parse status
        var status = GetJsonString(element, "status", "Status");
        violation.PaymentStatus = ParsePaymentStatus(status);

        return violation;
    }

    private string? GetJsonString(System.Text.Json.JsonElement element, params string[] propertyNames)
    {
        foreach (var name in propertyNames)
        {
            if (element.TryGetProperty(name, out var prop) && prop.ValueKind == System.Text.Json.JsonValueKind.String)
                return prop.GetString();
        }
        return null;
    }

    private decimal? GetJsonDecimal(System.Text.Json.JsonElement element, params string[] propertyNames)
    {
        foreach (var name in propertyNames)
        {
            if (element.TryGetProperty(name, out var prop))
            {
                if (prop.ValueKind == System.Text.Json.JsonValueKind.Number)
                    return prop.GetDecimal();
                if (prop.ValueKind == System.Text.Json.JsonValueKind.String &&
                    decimal.TryParse(prop.GetString()?.Replace("$", "").Replace(",", ""), out var val))
                    return val;
            }
        }
        return null;
    }

    private DateTime? GetJsonDate(System.Text.Json.JsonElement element, params string[] propertyNames)
    {
        foreach (var name in propertyNames)
        {
            if (element.TryGetProperty(name, out var prop) && prop.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                if (DateTime.TryParse(prop.GetString(), out var date))
                    return date;
            }
        }
        return null;
    }

    /// <summary>
    /// Fallback: Submit form and parse HTML response
    /// </summary>
    private async Task<List<ParkingViolation>?> FormSearchAsync(string licensePlate, string state)
    {
        var formData = new Dictionary<string, string>
        {
            ["SearchType"] = "Plate",
            ["LicensePlate"] = licensePlate,
            ["PlateNumber"] = licensePlate,
            ["PlateState"] = state,
            ["State"] = StateMap.GetValueOrDefault(state.ToUpper(), state)
        };

        if (!string.IsNullOrEmpty(_verificationToken))
            formData["__RequestVerificationToken"] = _verificationToken;

        var content = new FormUrlEncodedContent(formData);

        var formEndpoints = new[]
        {
            $"{_baseUrl}/Ticket/Search",
            $"{_baseUrl}/Search",
            _baseUrl
        };

        foreach (var endpoint in formEndpoints)
        {
            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var html = await response.Content.ReadAsStringAsync();
                    var violations = ParseHtmlResponse(html, licensePlate, state);
                    if (violations != null && violations.Count > 0)
                        return violations;
                }
            }
            catch
            {
                // Try next endpoint
            }
        }

        return null;
    }

    /// <summary>
    /// Parse HTML response for violations
    /// </summary>
    private List<ParkingViolation>? ParseHtmlResponse(string html, string licensePlate, string state)
    {
        var violations = new List<ParkingViolation>();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Try to find results in tables
        var tables = doc.DocumentNode.SelectNodes("//table");
        if (tables != null)
        {
            foreach (var table in tables)
            {
                var rows = table.SelectNodes(".//tr");
                if (rows == null) continue;

                foreach (var row in rows.Skip(1)) // Skip header
                {
                    var cells = row.SelectNodes(".//td");
                    if (cells == null || cells.Count < 2) continue;

                    var citationNum = cells[0].InnerText.Trim();
                    if (string.IsNullOrEmpty(citationNum) || citationNum.ToLower().Contains("citation"))
                        continue;

                    var violation = new ParkingViolation
                    {
                        CitationNumber = citationNum,
                        Tag = licensePlate,
                        State = state,
                        Provider = _providerId,
                        Agency = GetAgencyName(),
                        Link = _baseUrl,
                        Currency = "USD",
                        IsActive = true,
                        FineType = 1
                    };

                    // Try to get amount from last cell
                    var lastCellText = cells[cells.Count - 1].InnerText;
                    var amountMatch = Regex.Match(lastCellText, @"\$?([\d,]+\.?\d*)");
                    if (amountMatch.Success && decimal.TryParse(amountMatch.Groups[1].Value.Replace(",", ""), out var amount))
                        violation.Amount = amount;

                    violations.Add(violation);
                }
            }
        }

        // If no table found, try regex patterns on entire HTML
        if (violations.Count == 0)
        {
            var violation = ParseSingleViolationFromHtml(html, licensePlate, state);
            if (violation != null)
                violations.Add(violation);
        }

        return violations;
    }

    /// <summary>
    /// Parse single violation from HTML using regex patterns
    /// </summary>
    private ParkingViolation? ParseSingleViolationFromHtml(string html, string licensePlate, string state)
    {
        // Look for citation number pattern
        var citationMatch = Regex.Match(html, @"(?:Citation|Ticket)\s*(?:#|Number)?[:\s]*([A-Z0-9\-]+)", RegexOptions.IgnoreCase);
        if (!citationMatch.Success)
            return null;

        var violation = new ParkingViolation
        {
            CitationNumber = citationMatch.Groups[1].Value.Trim(),
            Tag = licensePlate,
            State = state,
            Provider = _providerId,
            Agency = GetAgencyName(),
            Link = _baseUrl,
            Currency = "USD",
            IsActive = true,
            FineType = 1
        };

        // Extract amount
        var totalMatch = Regex.Match(html, @"(?:Total|Amount)\s*(?:Due)?[:\s]*\$?([\d,]+\.?\d*)", RegexOptions.IgnoreCase);
        if (totalMatch.Success && decimal.TryParse(totalMatch.Groups[1].Value.Replace(",", ""), out var total))
            violation.Amount = total;

        // Extract date
        var dateMatch = Regex.Match(html, @"(?:Issue|Violation)\s*Date[:\s]*([\d/\-]+)", RegexOptions.IgnoreCase);
        if (dateMatch.Success && DateTime.TryParse(dateMatch.Groups[1].Value, out var issueDate))
            violation.IssueDate = issueDate;

        // Extract location
        var locMatch = Regex.Match(html, @"(?:Location|Address)[:\s]*([^\n<]+)", RegexOptions.IgnoreCase);
        if (locMatch.Success)
            violation.Address = locMatch.Groups[1].Value.Trim();

        // Extract violation description
        var violMatch = Regex.Match(html, @"(?:Violation|Description)[:\s]*([^\n<]+)", RegexOptions.IgnoreCase);
        if (violMatch.Success)
            violation.Note = violMatch.Groups[1].Value.Trim();

        // Extract status
        var statusMatch = Regex.Match(html, @"Status[:\s]*([^\n<]+)", RegexOptions.IgnoreCase);
        if (statusMatch.Success)
            violation.PaymentStatus = ParsePaymentStatus(statusMatch.Groups[1].Value.Trim());

        return violation;
    }

    /// <summary>
    /// Get agency name based on city
    /// </summary>
    private string GetAgencyName()
    {
        return _city.ToLower() switch
        {
            "philadelphia" => "Philadelphia Parking Authority",
            _ => $"OnlineServicesHub - {_city}"
        };
    }

    /// <summary>
    /// Parse payment status from string
    /// </summary>
    private int ParsePaymentStatus(string? status)
    {
        if (string.IsNullOrEmpty(status))
            return 0; // Unknown

        status = status.ToLower();

        if (status.Contains("paid"))
            return 1; // Paid
        if (status.Contains("open") || status.Contains("unpaid") || status.Contains("due"))
            return 0; // Unpaid
        if (status.Contains("dispute") || status.Contains("appeal"))
            return 2; // Disputed
        if (status.Contains("void") || status.Contains("dismiss"))
            return 3; // Voided

        return 0;
    }

    /// <summary>
    /// Raise error event
    /// </summary>
    private void OnError(string licensePlate, string state, Exception ex)
    {
        Error?.Invoke(this, new FinderErrorEventArgs
        {
            FinderName = nameof(OnlineServicesHubFinder),
            LicensePlate = licensePlate,
            State = state,
            Exception = ex,
            Message = ex.Message
        });
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _httpClient.Dispose();
        _disposed = true;
    }
}
