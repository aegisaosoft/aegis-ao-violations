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
using HuurApi.Models;

namespace AegisViolationsAPI.Abstr
{
    /// <summary>
    /// Generic base class for Socrata Open Data API parking violations finders
    /// </summary>
    /// <typeparam name="TResponse">City-specific response model type</typeparam>
    public abstract class ASocrataBase<TResponse> : IAegisAPIFinder where TResponse : class
    {
        protected string DatasetId = "";
        protected string BaseUrl = "";
        protected int DefaultLimit = 1000;
        
        // Query field names - override in derived classes if different
        protected virtual string PlateFieldName => "plate";
        protected virtual string StateFieldName => "state";
        protected virtual string OrderByFieldName => "issue_date";

        private readonly HttpClient _httpClient;
        private readonly string _appToken = "9gGE7tnFW9yyAh8GxW12veILS";
        private bool _disposed;

        public string Link { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        public event EventHandler<FinderErrorEventArgs>? Error;

        protected ASocrataBase()
        {
            _httpClient = new HttpClient();

            if (!string.IsNullOrEmpty(_appToken))
            {
                _httpClient.DefaultRequestHeaders.Add("X-App-Token", _appToken);
            }

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Map city-specific response to ParkingViolation - must be implemented by derived classes
        /// </summary>
        protected abstract ParkingViolation MapToParkingViolation(TResponse response);

        /// <summary>
        /// Find parking violations by license plate and state
        /// </summary>
        public async Task<List<ParkingViolation>> Find(string licensePlate, string state)
        {
            var violations = new List<ParkingViolation>();

            try
            {
                if (string.IsNullOrWhiteSpace(licensePlate))
                {
                    return violations;
                }

                var normalizedPlate = licensePlate.Trim().ToUpperInvariant().Replace(" ", "");
                var normalizedState = state?.Trim().ToUpperInvariant() ?? State;

                var url = BuildQueryUrl(normalizedPlate, normalizedState, DefaultLimit, 0);
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    RaiseError(licensePlate, normalizedState,
                        new HttpRequestException($"API returned {response.StatusCode}"),
                        $"Failed to fetch violations: {response.StatusCode}. {errorContent}");
                    return violations;
                }

                var cityViolations = await response.Content.ReadFromJsonAsync<List<TResponse>>();

                if (cityViolations == null || cityViolations.Count == 0)
                {
                    return violations;
                }

                foreach (var cityViolation in cityViolations)
                {
                    var violation = MapToParkingViolation(cityViolation);
                    violations.Add(violation);
                }
            }
            catch (HttpRequestException ex)
            {
                RaiseError(licensePlate, state ?? State, ex, $"HTTP request failed: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                RaiseError(licensePlate, state ?? State, ex, "Request timed out");
            }
            catch (Exception ex)
            {
                RaiseError(licensePlate, state ?? State, ex, $"Unexpected error: {ex.Message}");
            }

            return violations;
        }

        /// <summary>
        /// Find violations with pagination support
        /// </summary>
        public async Task<List<ParkingViolation>> FindWithPaging(string licensePlate, string state, int limit = 1000, int offset = 0)
        {
            var violations = new List<ParkingViolation>();

            try
            {
                var normalizedPlate = licensePlate.Trim().ToUpperInvariant().Replace(" ", "");
                var normalizedState = state?.Trim().ToUpperInvariant() ?? State;

                var url = BuildQueryUrl(normalizedPlate, normalizedState, limit, offset);
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var cityViolations = await response.Content.ReadFromJsonAsync<List<TResponse>>();

                if (cityViolations != null)
                {
                    violations.AddRange(cityViolations.Select(MapToParkingViolation));
                }
            }
            catch (Exception ex)
            {
                RaiseError(licensePlate, state ?? State, ex, ex.Message);
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

        /// <summary>
        /// Build the SoQL query URL - can be overridden for custom query logic
        /// </summary>
        protected virtual string BuildQueryUrl(string plate, string state, int limit, int offset)
        {
            var url = $"{BaseUrl}/{DatasetId}.json?" +
                      $"$where=upper({PlateFieldName})='{plate}' AND upper({StateFieldName})='{state}'" +
                      $"&$limit={limit}" +
                      $"&$order={OrderByFieldName} DESC";

            if (offset > 0)
            {
                url += $"&$offset={offset}";
            }

            return url;
        }

        protected void RaiseError(string licensePlate, string state, Exception exception, string message)
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

        // Helper methods for parsing
        protected static decimal ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            return decimal.TryParse(value, out var result) ? result : 0;
        }

        protected static DateTime? ParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, out var result))
                return result;

            if (DateTime.TryParseExact(value, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out result))
                return result;

            if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", null, System.Globalization.DateTimeStyles.None, out result))
                return result;

            return null;
        }

        protected static int DeterminePaymentStatus(decimal amountDue, string? status)
        {
            if (amountDue == 0)
                return Const.P_PAID;

            if (!string.IsNullOrEmpty(status))
            {
                var upperStatus = status.ToUpperInvariant();
                if (upperStatus.Contains("PAID"))
                    return Const.P_PAID;
                if (upperStatus.Contains("HEARING") || upperStatus.Contains("DISPUTE") || upperStatus.Contains("CONTEST"))
                    return Const.P_DISPUTED;
            }

            return Const.P_NEW;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
