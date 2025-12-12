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
using System.Text;
using System.Text.Json;
using AegisBroward.API.Models;
using AegisViolationsAPI;

namespace AegisBroward.API.Services
{
    public class BrowardClerkApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public string BaseUrl => _baseUrl;

        public BrowardClerkApiClient(string baseUrl, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("BaseUrl must not be null or empty", nameof(baseUrl));
            }
            _baseUrl = baseUrl.TrimEnd('/');
            _apiKey = apiKey ?? string.Empty;
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
                // _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
                // _httpClient.DefaultRequestHeaders.Add("ApiKey", _apiKey);
            }
        }

        // Case Detail endpoints
        public async Task<ApiResponse<CaseSummary>> GetCaseSummaryAsync(string caseNumber)
        {
            var url = $"{_baseUrl}/api/cases/{Uri.EscapeDataString(caseNumber)}/summary";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<CaseSummary>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<CaseSummary> { Success = false, Message = "Empty response" };
            }
            return new ApiResponse<CaseSummary> { Success = false, Message = content };
        }

        public async Task<ApiResponse<List<Party>>> GetCasePartiesAsync(string caseNumber)
        {
            var url = $"{_baseUrl}/api/cases/{Uri.EscapeDataString(caseNumber)}/parties";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<Party>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<List<Party>> { Success = false, Message = "Empty response", Data = new List<Party>() };
            }
            return new ApiResponse<List<Party>> { Success = false, Message = content, Data = new List<Party>() };
        }

        public async Task<ApiResponse<List<CaseEvent>>> GetCaseEventsAsync(string caseNumber)
        {
            var url = $"{_baseUrl}/api/cases/{Uri.EscapeDataString(caseNumber)}/events";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<CaseEvent>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<List<CaseEvent>> { Success = false, Message = "Empty response", Data = new List<CaseEvent>() };
            }
            return new ApiResponse<List<CaseEvent>> { Success = false, Message = content, Data = new List<CaseEvent>() };
        }

        public async Task<ApiResponse<List<Hearing>>> GetCaseHearingsAsync(string caseNumber)
        {
            var url = $"{_baseUrl}/api/cases/{Uri.EscapeDataString(caseNumber)}/hearings";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<Hearing>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<List<Hearing>> { Success = false, Message = "Empty response", Data = new List<Hearing>() };
            }
            return new ApiResponse<List<Hearing>> { Success = false, Message = content, Data = new List<Hearing>() };
        }

        // Searches endpoints
        public async Task<ApiResponse<List<CaseSummary>>> SearchCaseFilingsAsync(CaseFilingsSearchRequest request)
        {
            var url = $"{_baseUrl}/api/search/case-filings";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<CaseSummary>>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<List<CaseSummary>> { Success = false, Message = "Empty response", Data = new List<CaseSummary>() };
            }
            return new ApiResponse<List<CaseSummary>> { Success = false, Message = body, Data = new List<CaseSummary>() };
        }

        public async Task<ApiResponse<List<CaseSummary>>> SearchByPartyNameAsync(PartyNameSearchRequest request)
        {
            var url = $"{_baseUrl}/api/search/party-name";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<CaseSummary>>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<List<CaseSummary>> { Success = false, Message = "Empty response", Data = new List<CaseSummary>() };
            }
            return new ApiResponse<List<CaseSummary>> { Success = false, Message = body, Data = new List<CaseSummary>() };
        }

        public async Task<ApiResponse<CaseSummary>> FindCaseByCitationNumberAsync(NumberSearchRequest request)
        {
            var url = $"{_baseUrl}/api/search/number";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<CaseSummary>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<CaseSummary> { Success = false, Message = "Empty response" };
            }
            return new ApiResponse<CaseSummary> { Success = false, Message = body };
        }

        // Helper: find case by citation number (treated as number search)
        public Task<ApiResponse<CaseSummary>> FindCaseByCitationNumberAsync(string citationNumber)
        {
            var request = new NumberSearchRequest
            {
                CaseNumber = citationNumber
            };
            return FindCaseByCitationNumberAsync(request);
        }

        public async Task<ApiResponse<List<CaseSummary>>> SearchByPartyIdAsync(PartyIdSearchRequest request)
        {
            var url = $"{_baseUrl}/api/search/party-id";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<CaseSummary>>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<List<CaseSummary>> { Success = false, Message = "Empty response", Data = new List<CaseSummary>() };
            }
            return new ApiResponse<List<CaseSummary>> { Success = false, Message = body, Data = new List<CaseSummary>() };
        }

        public async Task<ApiResponse<List<Hearing>>> SearchHearingsAsync(HearingSearchRequest request)
        {
            var url = $"{_baseUrl}/api/search/hearing";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<Hearing>>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<List<Hearing>> { Success = false, Message = "Empty response", Data = new List<Hearing>() };
            }
            return new ApiResponse<List<Hearing>> { Success = false, Message = body, Data = new List<Hearing>() };
        }

        public async Task<ApiResponse<List<CaseSummary>>> SearchDispositionsAsync(DispositionSearchRequest request)
        {
            var url = $"{_baseUrl}/api/search/disposition";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<CaseSummary>>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new ApiResponse<List<CaseSummary>> { Success = false, Message = "Empty response", Data = new List<CaseSummary>() };
            }
            return new ApiResponse<List<CaseSummary>> { Success = false, Message = body, Data = new List<CaseSummary>() };
        }

        public async Task<ApiResponse<List<ViolationResult>>> SearchViolationsByPlateAsync(
            string licensePlate,
            string state,
            string? vehicleType = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"license_plate={Uri.EscapeDataString(licensePlate)}",
                    $"state={Uri.EscapeDataString(state)}"
                };

                if (!string.IsNullOrEmpty(vehicleType))
                {
                    queryParams.Add($"vehicle_type={Uri.EscapeDataString(vehicleType)}");
                }

                var queryString = string.Join("&", queryParams);
                var requestUrl = $"{_baseUrl}/api/violations/search?{queryString}";

                Console.WriteLine($"Making GET request to: {requestUrl}");

                var response = await _httpClient.GetAsync(requestUrl);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {content}");

                if (response.IsSuccessStatusCode)
                {
                var result = JsonSerializer.Deserialize<ApiResponse<List<ViolationResult>>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return result ?? new ApiResponse<List<ViolationResult>> { Success = false, Message = "Empty response", Data = new List<ViolationResult>() };
                }
                else
                {
                    return new ApiResponse<List<ViolationResult>>
                    {
                        Success = false,
                        Message = $"API Error: {response.StatusCode} - {content}",
                        Data = new List<ViolationResult>()
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new ApiResponse<List<ViolationResult>>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}",
                    Data = new List<ViolationResult>()
                };
            }
        }

        public async Task<ApiResponse<List<ViolationResult>>> SearchViolationsByPlatePostAsync(
            ViolationSearchRequest request)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/api/violations/search";
                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                Console.WriteLine($"Making POST request to: {requestUrl}");
                Console.WriteLine($"Request Body: {jsonContent}");

                var response = await _httpClient.PostAsync(requestUrl, httpContent);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {content}");

                if (response.IsSuccessStatusCode)
                {
                var result = JsonSerializer.Deserialize<ApiResponse<List<ViolationResult>>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return result ?? new ApiResponse<List<ViolationResult>> { Success = false, Message = "Empty response", Data = new List<ViolationResult>() };
                }
                else
                {
                    return new ApiResponse<List<ViolationResult>>
                    {
                        Success = false,
                        Message = $"API Error: {response.StatusCode} - {content}",
                        Data = new List<ViolationResult>()
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new ApiResponse<List<ViolationResult>>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}",
                    Data = new List<ViolationResult>()
                };
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }

        // Payment (placeholder): Actual API endpoints may differ; confirm with official docs
        public async Task<CitationPaymentResponse> PayCitationAsync(CitationPaymentRequest request)
        {
            var url = $"{_baseUrl}/api/payments/citation";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var parsed = JsonSerializer.Deserialize<CitationPaymentResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return parsed ?? new CitationPaymentResponse { Success = false, Message = "Empty response" };
            }
            return new CitationPaymentResponse { Success = false, Message = body };
        }
    }
}


