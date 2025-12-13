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

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using HuurApi.Models;
using AegisViolationsAPI;

namespace NYAegisViolations.Finders
{
    /// <summary>
    /// NYC Parking Violations finder using Socrata Open Data API
    /// Dataset: Open Parking and Camera Violations (nc67-uf89)
    /// https://data.cityofnewyork.us/City-Government/Open-Parking-and-Camera-Violations/nc67-uf89
    /// </summary>
    public class NYCFinder : IAegisAPIFinder
    {
        private const string DatasetId = "nc67-uf89";
        private const string BaseUrl = "https://data.cityofnewyork.us/resource";
        private const int DefaultLimit = 1000;
        
        private readonly HttpClient _httpClient;
        private readonly string? _appToken;
        private bool _disposed;

        public string Link => $"https://data.cityofnewyork.us/City-Government/Open-Parking-and-Camera-Violations/{DatasetId}";
        public string Name => "NYC";
        public string State => "NY";

        public event EventHandler<FinderErrorEventArgs>? Error;

        /// <summary>
        /// Initialize NYCFinder with optional Socrata app token
        /// </summary>
        /// <param name="appToken">Optional Socrata application token for higher rate limits</param>
        /// <param name="httpClient">Optional HttpClient instance for dependency injection</param>
        public NYCFinder(string? appToken = null, HttpClient? httpClient = null)
        {
            _appToken = "9gGE7tnFW9yyAh8GxW12veILS"; // appToken;
            _httpClient = httpClient ?? new HttpClient();
            
            if (!string.IsNullOrEmpty(_appToken))
            {
                _httpClient.DefaultRequestHeaders.Add("X-App-Token", _appToken);
            }
            
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Find parking violations by license plate and state
        /// </summary>
        /// <param name="licensePlate">License plate number</param>
        /// <param name="state">State code (e.g., "NY", "NJ")</param>
        /// <returns>List of parking violations</returns>
        public async Task<List<ParkingViolation>> Find(string licensePlate, string state)
        {
            var violations = new List<ParkingViolation>();
            
            try
            {
                if (string.IsNullOrWhiteSpace(licensePlate))
                {
                    return violations;
                }

                // Normalize license plate (uppercase, no spaces)
                var normalizedPlate = licensePlate.Trim().ToUpperInvariant().Replace(" ", "");
                var normalizedState = state?.Trim().ToUpperInvariant() ?? "NY";

                // Build SoQL query URL
                // Using $where clause for case-insensitive search
                var url = $"{BaseUrl}/{DatasetId}.json?" +
                          $"$where=upper(plate)='{normalizedPlate}' AND upper(state)='{normalizedState}'" +
                          $"&$limit={DefaultLimit}" +
                          $"&$order=issue_date DESC";

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    RaiseError(licensePlate, state ?? "NY", 
                        new HttpRequestException($"API returned {response.StatusCode}"),
                        $"Failed to fetch violations: {response.StatusCode}. {errorContent}");
                    return violations;
                }

                var nycViolations = await response.Content.ReadFromJsonAsync<List<NycViolationResponse>>();
                
                if (nycViolations == null || nycViolations.Count == 0)
                {
                    return violations;
                }

                // Map NYC violations to ParkingViolation model
                foreach (var nycViolation in nycViolations)
                {
                    var violation = MapToParkingViolation(nycViolation);
                    violations.Add(violation);
                }
            }
            catch (HttpRequestException ex)
            {
                RaiseError(licensePlate, state ?? "NY", ex, $"HTTP request failed: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                RaiseError(licensePlate, state ?? "NY", ex, "Request timed out");
            }
            catch (Exception ex)
            {
                RaiseError(licensePlate, state ?? "NY", ex, $"Unexpected error: {ex.Message}");
            }

            return violations;
        }

        /// <summary>
        /// Find violations with pagination support for large result sets
        /// </summary>
        public async Task<List<ParkingViolation>> FindWithPaging(string licensePlate, string state, int limit = 1000, int offset = 0)
        {
            var violations = new List<ParkingViolation>();
            
            try
            {
                var normalizedPlate = licensePlate.Trim().ToUpperInvariant().Replace(" ", "");
                var normalizedState = state?.Trim().ToUpperInvariant() ?? "NY";

                var url = $"{BaseUrl}/{DatasetId}.json?" +
                          $"$where=upper(plate)='{normalizedPlate}' AND upper(state)='{normalizedState}'" +
                          $"&$limit={limit}" +
                          $"&$offset={offset}" +
                          $"&$order=issue_date DESC";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var nycViolations = await response.Content.ReadFromJsonAsync<List<NycViolationResponse>>();
                
                if (nycViolations != null)
                {
                    violations.AddRange(nycViolations.Select(MapToParkingViolation));
                }
            }
            catch (Exception ex)
            {
                RaiseError(licensePlate, state ?? "NY", ex, ex.Message);
            }

            return violations;
        }

        /// <summary>
        /// Get all violations (paginated) for a license plate
        /// </summary>
        public async Task<List<ParkingViolation>> FindAll(string licensePlate, string state, int maxRecords = 10000)
        {
            var allViolations = new List<ParkingViolation>();
            var offset = 0;
            const int pageSize = 1000;

            while (offset < maxRecords)
            {
                var batch = await FindWithPaging(licensePlate, state, pageSize, offset);
                
                if (batch.Count == 0)
                    break;
                    
                allViolations.AddRange(batch);
                
                if (batch.Count < pageSize)
                    break;
                    
                offset += pageSize;
            }

            return allViolations;
        }

        private ParkingViolation MapToParkingViolation(NycViolationResponse nycViolation)
        {
            // Calculate total amount due
            var fineAmount = ParseDecimal(nycViolation.FineAmount);
            var penaltyAmount = ParseDecimal(nycViolation.PenaltyAmount);
            var interestAmount = ParseDecimal(nycViolation.InterestAmount);
            var reductionAmount = ParseDecimal(nycViolation.ReductionAmount);
            var paymentAmount = ParseDecimal(nycViolation.PaymentAmount);
            var amountDue = ParseDecimal(nycViolation.AmountDue);

            // Determine payment status based on amount due and violation status
            var paymentStatus = DeterminePaymentStatus(amountDue, nycViolation.ViolationStatus);

            return new ParkingViolation
            {
                Id = nycViolation.SummonsNumber,
                CitationNumber = nycViolation.SummonsNumber,
                NoticeNumber = nycViolation.SummonsNumber,
                Provider = 0, // NYC DOF
                Agency = $"NYC {nycViolation.IssuingAgency ?? "DOF"}",
                Address = $"Precinct {nycViolation.Precinct}, {nycViolation.County} County",
                Tag = nycViolation.Plate,
                State = nycViolation.State,
                IssueDate = ParseDateTime(nycViolation.IssueDate),
                Amount = amountDue > 0 ? amountDue : fineAmount + penaltyAmount + interestAmount - reductionAmount,
                Currency = "USD",
                PaymentStatus = paymentStatus,
                FineType = 0, // Parking
                Note = BuildNote(nycViolation),
                Link = BuildSummonsLink(nycViolation.SummonsImage),
                IsActive = amountDue > 0
            };
        }

        private static int DeterminePaymentStatus(decimal amountDue, string? violationStatus)
        {
            if (amountDue == 0)
                return Const.P_PAID; // Paid
            
            if (!string.IsNullOrEmpty(violationStatus))
            {
                var status = violationStatus.ToUpperInvariant();
                if (status.Contains("PAID"))
                    return Const.P_PAID; // Paid
                if (status.Contains("HEARING") || status.Contains("DISPUTE"))
                    return Const.P_DISPUTED; // Disputed
            }
            
            return Const.P_NEW; // Unpaid
        }

        private static string BuildNote(NycViolationResponse violation)
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrEmpty(violation.Violation))
                parts.Add($"Violation: {violation.Violation}");
            
            if (!string.IsNullOrEmpty(violation.ViolationTime))
                parts.Add($"Time: {violation.ViolationTime}");
            
            if (!string.IsNullOrEmpty(violation.LicenseType))
                parts.Add($"License Type: {violation.LicenseType}");
            
            if (!string.IsNullOrEmpty(violation.ViolationStatus))
                parts.Add($"Status: {violation.ViolationStatus}");

            // Add financial breakdown if there are penalties or interest
            var penalty = ParseDecimal(violation.PenaltyAmount);
            var interest = ParseDecimal(violation.InterestAmount);
            
            if (penalty > 0)
                parts.Add($"Penalty: ${penalty:F2}");
            
            if (interest > 0)
                parts.Add($"Interest: ${interest:F2}");

            return string.Join(" | ", parts);
        }

        private static string? BuildSummonsLink(SummonsImageInfo? summonsImage)
        {
            if (summonsImage == null || string.IsNullOrEmpty(summonsImage.Url))
                return null;

            // Return the URL directly from the object
            return summonsImage.Url;
        }

        private static string? BuildSummonsLinkLegacy(string? summonsImage)
        {
            if (string.IsNullOrEmpty(summonsImage))
                return null;

            // Extract URL from "View Summons (url)" format if present
            if (summonsImage.Contains("http"))
            {
                var startIndex = summonsImage.IndexOf("http", StringComparison.OrdinalIgnoreCase);
                var endIndex = summonsImage.IndexOf(')', startIndex);
                
                if (endIndex > startIndex)
                    return summonsImage.Substring(startIndex, endIndex - startIndex);
                
                return summonsImage.Substring(startIndex);
            }

            return summonsImage;
        }

        private static decimal ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;
            
            return decimal.TryParse(value, out var result) ? result : 0;
        }

        private static DateTime? ParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Try parsing various date formats
            if (DateTime.TryParse(value, out var result))
                return result;

            // Try MM/dd/yyyy format
            if (DateTime.TryParseExact(value, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out result))
                return result;

            // Try ISO format
            if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", null, System.Globalization.DateTimeStyles.None, out result))
                return result;

            return null;
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
                // Only dispose if we created the HttpClient
                // Don't dispose if it was injected
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Response model for NYC Open Parking and Camera Violations API
    /// </summary>
    internal class NycViolationResponse
    {
        [JsonPropertyName("plate")]
        public string? Plate { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("license_type")]
        public string? LicenseType { get; set; }

        [JsonPropertyName("summons_number")]
        public string? SummonsNumber { get; set; }

        [JsonPropertyName("issue_date")]
        public string? IssueDate { get; set; }

        [JsonPropertyName("violation_time")]
        public string? ViolationTime { get; set; }

        [JsonPropertyName("violation")]
        public string? Violation { get; set; }

        [JsonPropertyName("judgment_entry_date")]
        public string? JudgmentEntryDate { get; set; }

        [JsonPropertyName("fine_amount")]
        public string? FineAmount { get; set; }

        [JsonPropertyName("penalty_amount")]
        public string? PenaltyAmount { get; set; }

        [JsonPropertyName("interest_amount")]
        public string? InterestAmount { get; set; }

        [JsonPropertyName("reduction_amount")]
        public string? ReductionAmount { get; set; }

        [JsonPropertyName("payment_amount")]
        public string? PaymentAmount { get; set; }

        [JsonPropertyName("amount_due")]
        public string? AmountDue { get; set; }

        [JsonPropertyName("precinct")]
        public string? Precinct { get; set; }

        [JsonPropertyName("county")]
        public string? County { get; set; }

        [JsonPropertyName("issuing_agency")]
        public string? IssuingAgency { get; set; }

        [JsonPropertyName("violation_status")]
        public string? ViolationStatus { get; set; }

        [JsonPropertyName("summons_image")]
        public SummonsImageInfo? SummonsImage { get; set; }
    }

    /// <summary>
    /// Summons image information
    /// </summary>
    internal class SummonsImageInfo
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
