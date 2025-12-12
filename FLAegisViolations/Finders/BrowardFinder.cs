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
using AegisViolationsAPI;
using Microsoft.Extensions.Configuration;
using HuurApi.Models;
using AegisBroward.API.Services;
using AegisBroward.API.Models;
using AegisViolationsAPI.Helpers;

namespace FLAegisViolations.Finders
{
    public class BrowardFinder : IAegisAPIFinder, IDisposable
    {
        private readonly BrowardClerkApiClient _apiClient;

        public string Name => "Broward Clerk";
        public string State => "FL";
        public string Link => "https://api.browardclerk.org";
        
        public event EventHandler<FinderErrorEventArgs>? Error;

        public BrowardFinder()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var settings = new AegisBrowardSettings();
            configuration.GetSection("AegisBroward").Bind(settings);

            var baseUrl = settings?.BaseUrl ?? string.Empty;
            var apiKey = settings?.ApiKey ?? string.Empty;

            _apiClient = new BrowardClerkApiClient(baseUrl, apiKey);
        }

        public async Task<List<ParkingViolation>> Find(string licensePlate, string state)
        {
            try
            {
                var response = await _apiClient.SearchViolationsByPlateAsync(licensePlate, state);
                if (response != null && response.Success && response.Data != null)
                {
                    var violations = new List<ParkingViolation>();
                    foreach (var v in response.Data)
                    {
                        var pv = new ParkingViolation
                        {
                            CitationNumber = v.CitationNumber ?? string.Empty,
                            Agency = Name,
                            Tag = v.LicensePlate ?? string.Empty,
                            State = v.State ?? string.Empty,
                            IssueDate = v.IssueDate,
                            Amount = v.Amount,
                            Currency = "USD",
                            PaymentStatus = Helper.GetStaus(v.Status),
                            FineType = Const.FT_PARKING,
                            Link = _apiClient.BaseUrl ?? string.Empty
                        };

                        violations.Add(pv);
                    }
                    return violations;
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, new FinderErrorEventArgs
                {
                    FinderName = Name,
                    LicensePlate = licensePlate,
                    State = state ?? string.Empty,
                    Exception = ex,
                    Message = ex.Message
                });
            }
            return new List<ParkingViolation>();
        }

        public void Dispose()
        {
            _apiClient?.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}


