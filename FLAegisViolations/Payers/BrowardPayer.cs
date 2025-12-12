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
using Microsoft.Extensions.Configuration;
using AegisBroward.API.Services;
using AegisBroward.API.Models;
using AegisViolationsAPI;

namespace FLAegisViolations.Payers
{
    public class BrowardPayer : IAegisAPIPayer, IDisposable
    {
        private readonly BrowardClerkApiClient _apiClient;
        private readonly PaymentCard? _defaultPaymentCard = null;

        public string Name => "Broward Clerk";
        public string State => "FL";

        public BrowardPayer()
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

        public void Dispose()
        {
            _apiClient?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<bool> Pay(string citation, decimal amount)
        {
            // Verify case exists for the given citation
            var caseResult = await _apiClient.FindCaseByCitationNumberAsync(citation);
            if (caseResult == null || !caseResult.Success || caseResult.Data == null)
            {
                return false;
            }

            var payReq = new CitationPaymentRequest
            {
                CitationNumber = citation,
                Amount = amount,
                Card = _defaultPaymentCard
            };

            var payRes = await _apiClient.PayCitationAsync(payReq);
            return payRes != null && payRes.Success;
        }
    }
}
