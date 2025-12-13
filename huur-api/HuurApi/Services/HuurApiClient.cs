/*
 *
 * Copyright (c) 2024 Alexander Orlov.
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

using HuurApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace HuurApi.Services;

/// <summary>
/// Client for interacting with the Huur API
/// </summary>
public class HuurApiClient : IHuurApiClient
{
    private readonly HttpClient _httpClient;
    private readonly HuurApiOptions _options;
    private readonly ILogger<HuurApiClient> _logger;

    /// <summary>
    /// Initializes a new instance of the HuurApiClient
    /// </summary>
    /// <param name="httpClient">HTTP client for making API requests</param>
    /// <param name="options">Configuration options for the API client</param>
    /// <param name="logger">Logger for recording client operations</param>
    public HuurApiClient(HttpClient httpClient, IOptions<HuurApiOptions> options, ILogger<HuurApiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        
        if (!string.IsNullOrEmpty(_options.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _options.ApiKey);
        }
    }

    /// <summary>
    /// Signs in a user with the provided credentials
    /// </summary>
    /// <param name="signinRequest">User sign-in credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sign-in response containing user data and token</returns>
    public async Task<SigninResponse> SigninAsync(SigninRequest signinRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonConvert.SerializeObject(signinRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var fullUrl = $"{_options.BaseUrl}/UserAuth/signin";
            var response = await _httpClient.PostAsync(fullUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var signinResponse = JsonConvert.DeserializeObject<SigninResponse>(responseContent);
            
            return signinResponse ?? new SigninResponse 
            { 
                Reason = -1,
                Message = "Sign-in failed",
                Result = new SigninResult()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing in user: {Email}", signinRequest.Email);
            throw;
        }
    }

    /// <summary>
    /// Gets EZ-Pass charges between two dates. Requires Authorization.
    /// </summary>
    /// <param name="dateFrom">Start date (as string, exactly as API expects in path)</param>
    /// <param name="dateTo">End date (as string, exactly as API expects in path)</param>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>EZ-Pass charges response</returns>
    public async Task<EzpassChargesResponse> GetEzpassChargesAsync(string dateFrom, string dateTo, string bearerToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            // Prepare request with Authorization header
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/TollPayment/get-ezpass-charges/{Uri.EscapeDataString(dateFrom)}/{Uri.EscapeDataString(dateTo)}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var ezpassChargesResponse = JsonConvert.DeserializeObject<EzpassChargesResponse>(content);
            return ezpassChargesResponse ?? new EzpassChargesResponse
            {
                Reason = -1,
                Message = "Failed to deserialize failed payments response",
                Result = new List<EzpassCharge>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching EZ-Pass charges for {DateFrom} - {DateTo}", dateFrom, dateTo);
            throw;
        }
    }

    /// <summary>
    /// Gets Parking Violations between two dates. Requires Authorization.
    /// </summary>
    /// <param name="dateFrom">Start date (as string, exactly as API expects in path)</param>
    /// <param name="dateTo">End date (as string, exactly as API expects in path)</param>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parking violations response</returns>
    public async Task<ParkingViolationsResponse> GetParkingViolationsAsync(string dateFrom, string dateTo, string bearerToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            // Prepare request with Authorization header
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/ExternalViolation/get-all?dateFrom={Uri.EscapeDataString(dateFrom)}&dateTo={Uri.EscapeDataString(dateTo)}");
            //using var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/ExternalViolation/get-all");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.ReasonPhrase?.ToLower().Equals("not found") == true)
            {
                return new ParkingViolationsResponse
                {
                    Reason = -1,
                    Message = "Violations not Found",
                    Result = new List<ParkingViolation>()
                };

            }
            else
                response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var parkingViolationsResponse = JsonConvert.DeserializeObject<ParkingViolationsResponse>(content);
            return parkingViolationsResponse ?? new ParkingViolationsResponse
            {
                Reason = -1,
                Message = "Failed to deserialize Parking Violations response",
                Result = new List<ParkingViolation>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Parking Violations for {DateFrom} - {DateTo}", dateFrom, dateTo);
            throw;
        }
    }

    /// <summary>
    /// Gets failed payments. Requires Authorization with Owner or Employee role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="ownerId">Optional owner ID to filter results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Failed payments response</returns>
    public async Task<FailedPaymentsResponse> GetFailedPaymentsAsync(string bearerToken, string? ownerId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            // Build URL with optional ownerId query parameter
            var url = $"{_options.BaseUrl}/TollPayment/get-failed-payments";
            if (!string.IsNullOrWhiteSpace(ownerId))
            {
                url += $"?ownerId={Uri.EscapeDataString(ownerId)}";
            }

            // Prepare request with Authorization header
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var failedPaymentsResponse = JsonConvert.DeserializeObject<FailedPaymentsResponse>(content);
            
            return failedPaymentsResponse ?? new FailedPaymentsResponse 
            { 
                Reason = -1,
                Message = "Failed to deserialize failed payments response",
                Result = new List<FailedPayment>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching failed payments for ownerId: {OwnerId}", ownerId ?? "all");
            throw;
        }
    }

    /// <summary>
    /// Gets complete payments. Requires Authorization with Owner or Employee role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="ownerId">Optional owner ID to filter results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete payments response</returns>
    public async Task<CompletePaymentsResponse> GetCompletePaymentsAsync(string bearerToken, string? ownerId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            // Build URL with optional ownerId query parameter
            var url = $"{_options.BaseUrl}/TollPayment/get-complete-payments";
            if (!string.IsNullOrWhiteSpace(ownerId))
            {
                url += $"?ownerId={Uri.EscapeDataString(ownerId)}";
            }

            // Prepare request with Authorization header
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var completePaymentsResponse = JsonConvert.DeserializeObject<CompletePaymentsResponse>(content);
            
            return completePaymentsResponse ?? new CompletePaymentsResponse 
            { 
                Reason = -1,
                Message = "Failed to deserialize complete payments response",
                Result = new List<CompletePayment>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching complete payments for ownerId: {OwnerId}", ownerId ?? "all");
            throw;
        }
    }

    /// <summary>
    /// Creates a new parking violation. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="parkingViolation">Parking violation data to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response indicating success or failure of the creation operation</returns>
    public async Task<CreateParkingViolationResponse> CreateParkingViolationAsync(string bearerToken, ParkingViolation parkingViolation, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        if (parkingViolation == null)
        {
            throw new ArgumentNullException(nameof(parkingViolation));
        }

        try
        {
            var json = JsonConvert.SerializeObject(parkingViolation);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Prepare request with Authorization header
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/ExternalViolation/create");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            request.Content = content;

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var createResponse = JsonConvert.DeserializeObject<CreateParkingViolationResponse>(responseContent);
            
            return createResponse ?? new CreateParkingViolationResponse
            {
                Reason = -1,
                Message = "Failed to deserialize create parking violation response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parking violation");
            throw;
        }
    }

    /// <summary>
    /// Gets a parking violation by its ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Violation ID to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parking violation response with the requested violation</returns>
    public async Task<CreateParkingViolationResponse> GetViolationByIdAsync(string bearerToken, string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            // Prepare request with Authorization header
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/ExternalViolation/get/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var violationResponse = JsonConvert.DeserializeObject<CreateParkingViolationResponse>(content);
            
            return violationResponse ?? new CreateParkingViolationResponse
            {
                Reason = -1,
                Message = "Failed to deserialize violation response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching violation with ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a parking violation by its ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Violation ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response indicating success or failure of the deletion operation</returns>
    public async Task<CreateParkingViolationResponse> DeleteViolationAsync(string bearerToken, string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            // Prepare request with Authorization header
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"{_options.BaseUrl}/ExternalViolation/delete/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var deleteResponse = JsonConvert.DeserializeObject<CreateParkingViolationResponse>(content);
            
            return deleteResponse ?? new CreateParkingViolationResponse
            {
                Reason = -1,
                Message = "Failed to deserialize delete violation response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting violation with ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Gets car list by license plate. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cars list response</returns>
    public async Task<CarListByLicensePlateResponse> GetCarListByLicensePlateAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/Cars/CarListByLicensePlate");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var carsResponse = JsonConvert.DeserializeObject<CarListByLicensePlateResponse>(responseContent);

            return carsResponse ?? new CarListByLicensePlateResponse
            {
                Reason = -1,
                Message = "Failed to deserialize car list response",
                Result = new List<Car>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting car list by license plate");
            throw;
        }
    }

    /// <summary>
    /// Creates a new car. Requires Authorization with Owner or Employee role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Car creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Car creation response</returns>
    public async Task<CreateCarResponse> CreateCarAsync(string bearerToken, CreateCarRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/Cars");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            httpRequest.Content = content;

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var createResponse = JsonConvert.DeserializeObject<CreateCarResponse>(responseContent);

            return createResponse ?? new CreateCarResponse
            {
                Reason = -1,
                Message = "Failed to deserialize car creation response"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating car with title: {Title}", request.Title);
            throw;
        }
    }

    /// <summary>
    /// Gets all cars. Requires Authorization with Owner or Employee role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Request with role parameter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All cars response</returns>
    public async Task<GetCarsAllResponse> GetCarsAllAsync(string bearerToken, GetCarsAllRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/Cars/all");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            httpRequest.Content = content;

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getCarsAllResponse = JsonConvert.DeserializeObject<GetCarsAllResponse>(responseContent);

            return getCarsAllResponse ?? new GetCarsAllResponse
            {
                Reason = -1,
                Message = "Failed to deserialize cars all response",
                Result = new List<CarAll>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all cars with role: {Role}", request.Role);
            throw;
        }
    }

    /// <summary>
    /// Gets all license plates. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>License plates response</returns>
    public async Task<GetLicensePlatesResponse> GetLicensePlatesAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/Cars/GetLicensePlates");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var licensePlatesResponse = JsonConvert.DeserializeObject<GetLicensePlatesResponse>(responseContent);

            return licensePlatesResponse ?? new GetLicensePlatesResponse
            {
                Reason = -1,
                Message = "Failed to deserialize license plates response",
                Result = new List<LicensePlate>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting license plates");
            throw;
        }
    }

    /// <summary>
    /// Gets all external vehicle license plates. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>License plates response</returns>
    public async Task<GetLicensePlatesResponse> GetExternalVehicleLicensePlatesAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/ExternalVehicles/GetLicensePlates");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var licensePlatesResponse = JsonConvert.DeserializeObject<GetLicensePlatesResponse>(responseContent);

            return licensePlatesResponse ?? new GetLicensePlatesResponse
            {
                Reason = -1,
                Message = "Failed to deserialize external vehicle license plates response",
                Result = new List<LicensePlate>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting external vehicle license plates");
            throw;
        }
    }

    /// <summary>
    /// Gets all users. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users list response</returns>
    public async Task<GetUsersResponse> GetUsersAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/Users");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getUsersResponse = JsonConvert.DeserializeObject<GetUsersResponse>(responseContent);

            return getUsersResponse ?? new GetUsersResponse
            {
                Reason = -1,
                Message = "Failed to deserialize users response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users list");
            throw;
        }
    }

    /// <summary>
    /// Gets all external daily invoices. Requires Authorization.
    /// </summary>
    public async Task<GetAllExternalTollDailyInvoicesResponse> GetAllExternalTollDailyInvoicesAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/api/ExternalTollDailyInvoice/get-all");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getAllInvoicesResponse = JsonConvert.DeserializeObject<GetAllExternalTollDailyInvoicesResponse>(responseContent);

            return getAllInvoicesResponse ?? new GetAllExternalTollDailyInvoicesResponse
            {
                Reason = -1,
                Message = "Failed to deserialize invoices response",
                Result = new List<ExternalTollDailyInvoice>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all external daily invoices");
            throw;
        }
    }

    /// <summary>
    /// Gets external daily invoice by ID. Requires Authorization.
    /// </summary>
    public async Task<GetExternalTollDailyInvoiceByIdResponse> GetExternalTollDailyInvoiceByIdAsync(string bearerToken, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/api/ExternalTollDailyInvoice/get/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getInvoiceResponse = JsonConvert.DeserializeObject<GetExternalTollDailyInvoiceByIdResponse>(responseContent);

            return getInvoiceResponse ?? new GetExternalTollDailyInvoiceByIdResponse
            {
                Reason = -1,
                Message = "Failed to deserialize invoice response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting external daily invoice by ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Gets external daily invoices by toll ID. Requires Authorization.
    /// </summary>
    public async Task<GetExternalTollDailyInvoiceByTollIdResponse> GetExternalTollDailyInvoiceByTollIdAsync(string bearerToken, GetInvoiceByTollIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/api/ExternalTollDailyInvoice/get-by-toll-id");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Add query parameters
            var queryParams = new List<string>();
            if (request.TollId > 0)
            {
                queryParams.Add($"tollId={request.TollId}");
            }

            if (queryParams.Any())
            {
                httpRequest.RequestUri = new Uri($"{_options.BaseUrl}/api/ExternalTollDailyInvoice/get-by-toll-id?{string.Join("&", queryParams)}");
            }

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getInvoicesResponse = JsonConvert.DeserializeObject<GetExternalTollDailyInvoiceByTollIdResponse>(responseContent);

            return getInvoicesResponse ?? new GetExternalTollDailyInvoiceByTollIdResponse
            {
                Reason = -1,
                Message = "Failed to deserialize invoices by toll ID response",
                Result = new List<ExternalTollDailyInvoice>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting external daily invoices by toll ID: {TollId}", request.TollId);
            throw;
        }
    }

    /// <summary>
    /// Gets external daily invoices by plate number. Requires Authorization.
    /// </summary>
    public async Task<GetExternalTollDailyInvoiceByPlateResponse> GetExternalTollDailyInvoiceByPlateAsync(string bearerToken, GetInvoiceByPlateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/api/ExternalTollDailyInvoice/get-by-plate");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Add query parameters
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(request.PlateNumber))
            {
                queryParams.Add($"plateNumber={Uri.EscapeDataString(request.PlateNumber)}");
            }
            if (!string.IsNullOrEmpty(request.PlateState))
            {
                queryParams.Add($"plateState={Uri.EscapeDataString(request.PlateState)}");
            }

            if (queryParams.Any())
            {
                httpRequest.RequestUri = new Uri($"{_options.BaseUrl}/api/ExternalTollDailyInvoice/get-by-plate?{string.Join("&", queryParams)}");
            }

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getInvoicesResponse = JsonConvert.DeserializeObject<GetExternalTollDailyInvoiceByPlateResponse>(responseContent);

            return getInvoicesResponse ?? new GetExternalTollDailyInvoiceByPlateResponse
            {
                Reason = -1,
                Message = "Failed to deserialize invoices by plate response",
                Result = new List<ExternalTollDailyInvoice>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting external daily invoices by plate: {PlateNumber} {PlateState}", request.PlateNumber, request.PlateState);
            throw;
        }
    }

    /// <summary>
    /// Creates a new external daily invoice. Requires Authorization with Admin role.
    /// </summary>
    public async Task<CreateExternalTollDailyInvoiceResponse> CreateExternalTollDailyInvoiceAsync(string bearerToken, CreateExternalTollDailyInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/api/ExternalTollDailyInvoice/create");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var jsonContent = JsonConvert.SerializeObject(request);
            httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var createInvoiceResponse = JsonConvert.DeserializeObject<CreateExternalTollDailyInvoiceResponse>(responseContent);

            return createInvoiceResponse ?? new CreateExternalTollDailyInvoiceResponse
            {
                Reason = -1,
                Message = "Failed to deserialize create invoice response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating external daily invoice");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing external daily invoice. Requires Authorization with Admin role.
    /// </summary>
    public async Task<UpdateExternalTollDailyInvoiceResponse> UpdateExternalTollDailyInvoiceAsync(string bearerToken, string id, UpdateExternalTollDailyInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{_options.BaseUrl}/api/ExternalTollDailyInvoice/update/{id}");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var jsonContent = JsonConvert.SerializeObject(request);
            httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var updateInvoiceResponse = JsonConvert.DeserializeObject<UpdateExternalTollDailyInvoiceResponse>(responseContent);

            return updateInvoiceResponse ?? new UpdateExternalTollDailyInvoiceResponse
            {
                Reason = -1,
                Message = "Failed to deserialize update invoice response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating external daily invoice: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes an external daily invoice by ID. Requires Authorization with Admin role.
    /// </summary>
    public async Task<DeleteExternalTollDailyInvoiceResponse> DeleteExternalTollDailyInvoiceAsync(string bearerToken, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_options.BaseUrl}/api/ExternalTollDailyInvoice/delete/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var deleteInvoiceResponse = JsonConvert.DeserializeObject<DeleteExternalTollDailyInvoiceResponse>(responseContent);

            return deleteInvoiceResponse ?? new DeleteExternalTollDailyInvoiceResponse
            {
                Reason = -1,
                Message = "Failed to deserialize delete invoice response",
                Result = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting external daily invoice: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Updates payment status for multiple external toll daily invoices. Requires Authorization and admin role.
    /// </summary>
    public async Task<UpdatePaymentStatusResponse> UpdatePaymentStatusAsync(string bearerToken, UpdatePaymentStatusRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{_options.BaseUrl}/api/ExternalTollDailyInvoice/update-payment-status");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Add companyId as query parameter
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(request.CompanyId))
            {
                queryParams.Add($"companyId={Uri.EscapeDataString(request.CompanyId)}");
            }

            if (queryParams.Any())
            {
                httpRequest.RequestUri = new Uri($"{_options.BaseUrl}/api/ExternalTollDailyInvoice/update-payment-status?{string.Join("&", queryParams)}");
            }

            // Serialize request body
            var jsonContent = JsonConvert.SerializeObject(request);
            httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var updatePaymentStatusResponse = JsonConvert.DeserializeObject<UpdatePaymentStatusResponse>(responseContent);

            return updatePaymentStatusResponse ?? new UpdatePaymentStatusResponse
            {
                Reason = -1,
                Message = "Failed to deserialize update payment status response",
                Result = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for invoices: {Ids}, Status: {Status}, Company: {CompanyId}", 
                string.Join(",", request.Ids), request.PaymentStatus, request.CompanyId);
            throw;
        }
    }

    /// <summary>
    /// Gets all companies. Requires Authorization with Admin role.
    /// </summary>
    public async Task<GetCompaniesAllResponse> GetCompaniesAllAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/Companies/all");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var companiesResponse = JsonConvert.DeserializeObject<GetCompaniesAllResponse>(responseContent);

            return companiesResponse ?? new GetCompaniesAllResponse
            {
                Reason = -1,
                Message = "Failed to deserialize companies all response",
                Result = new List<Company>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting companies all");
            throw;
        }
    }

    /// <summary>
    /// Gets active companies. Requires Authorization with Admin role.
    /// </summary>
    public async Task<GetCompaniesActiveResponse> GetCompaniesActiveAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/Companies/active");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var companiesResponse = JsonConvert.DeserializeObject<GetCompaniesActiveResponse>(responseContent);

            return companiesResponse ?? new GetCompaniesActiveResponse
            {
                Reason = -1,
                Message = "Failed to deserialize companies active response",
                Result = new List<Company>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting companies active");
            throw;
        }
    }

    /// <summary>
    /// Updates a parking violation by ID. Requires Authorization.
    /// </summary>
    public async Task<UpdateParkingViolationResponse> UpdateParkingViolationAsync(string bearerToken, string id, UpdateParkingViolationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{_options.BaseUrl}/ExternalViolation/update/{id}");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Serialize request body
            var jsonContent = JsonConvert.SerializeObject(request);
            httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var updateResponse = JsonConvert.DeserializeObject<UpdateParkingViolationResponse>(responseContent);

            return updateResponse ?? new UpdateParkingViolationResponse
            {
                Reason = -1,
                Message = "Failed to deserialize update violation response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parking violation: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Gets a parking violation by agency and notice number. Requires Authorization.
    /// </summary>
    public async Task<GetParkingViolationByAgencyResponse> GetParkingViolationByAgencyAsync(string bearerToken, string agency, string noticeNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/ExternalViolation/{Uri.EscapeDataString(agency)}/notices/{Uri.EscapeDataString(noticeNumber)}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getResponse = JsonConvert.DeserializeObject<GetParkingViolationByAgencyResponse>(responseContent);

            return getResponse ?? new GetParkingViolationByAgencyResponse
            {
                Reason = -1,
                Message = "Failed to deserialize violation by agency response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting parking violation by agency: {Agency}, {NoticeNumber}", agency, noticeNumber);
            throw;
        }
    }

    /// <summary>
    /// Gets all parking violations with optional filters. Requires Authorization.
    /// </summary>
    public async Task<ParkingViolationsResponse> GetAllParkingViolationsAsync(string bearerToken, DateTime? dateFrom = null, DateTime? dateTo = null, string? companyId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/ExternalViolation/get-all");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Add query parameters
            var queryParams = new List<string>();
            if (dateFrom.HasValue)
            {
                queryParams.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");
            }
            if (dateTo.HasValue)
            {
                queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                queryParams.Add($"CompanyId={Uri.EscapeDataString(companyId)}");
            }

            if (queryParams.Any())
            {
                httpRequest.RequestUri = new Uri($"{_options.BaseUrl}/ExternalViolation/get-all?{string.Join("&", queryParams)}");
            }

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getAllResponse = JsonConvert.DeserializeObject<ParkingViolationsResponse>(responseContent);

            return getAllResponse ?? new ParkingViolationsResponse
            {
                Reason = -1,
                Message = "Failed to deserialize violations response",
                Result = new List<ParkingViolation>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all parking violations");
            throw;
        }
    }

    /// <summary>
    /// Gets a parking violation by ID. Requires Authorization.
    /// </summary>
    public async Task<GetParkingViolationResponse> GetParkingViolationByIdAsync(string bearerToken, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/ExternalViolation/get/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getResponse = JsonConvert.DeserializeObject<GetParkingViolationResponse>(responseContent);

            return getResponse ?? new GetParkingViolationResponse
            {
                Reason = -1,
                Message = "Failed to deserialize violation by ID response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting parking violation by ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new parking violation. Requires Authorization.
    /// </summary>
    public async Task<CreateParkingViolationResponse> CreateParkingViolationAsync(string bearerToken, CreateParkingViolationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/ExternalViolation/create");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Serialize request body
            var jsonContent = JsonConvert.SerializeObject(request);
            httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var createResponse = JsonConvert.DeserializeObject<CreateParkingViolationResponse>(responseContent);

            return createResponse ?? new CreateParkingViolationResponse
            {
                Reason = -1,
                Message = "Failed to deserialize create violation response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parking violation");
            throw;
        }
    }

    /// <summary>
    /// Deletes a parking violation by ID. Requires Authorization.
    /// </summary>
    public async Task<DeleteParkingViolationResponse> DeleteParkingViolationAsync(string bearerToken, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_options.BaseUrl}/ExternalViolation/delete/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var deleteResponse = JsonConvert.DeserializeObject<DeleteParkingViolationResponse>(responseContent);

            return deleteResponse ?? new DeleteParkingViolationResponse
            {
                Reason = -1,
                Message = "Failed to deserialize delete violation response",
                Result = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parking violation: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Updates payment status for multiple parking violations. Requires Authorization and admin role.
    /// </summary>
    public async Task<UpdateViolationPaymentStatusResponse> UpdateViolationPaymentStatusAsync(string bearerToken, UpdateViolationPaymentStatusRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{_options.BaseUrl}/ExternalViolation/update-payment-status");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Serialize request body
            var jsonContent = JsonConvert.SerializeObject(request);
            httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var updatePaymentStatusResponse = JsonConvert.DeserializeObject<UpdateViolationPaymentStatusResponse>(responseContent);

            return updatePaymentStatusResponse ?? new UpdateViolationPaymentStatusResponse
            {
                Reason = -1,
                Message = "Failed to deserialize update violation payment status response",
                Result = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for violations: {Ids}, Status: {Status}, Company: {CompanyId}", 
                string.Join(",", request.Ids), request.PaymentStatus, request.CompanyId);
            throw;
        }
    }

    /// <summary>
    /// Gets external toll daily invoices by company ID. Requires Authorization with Admin role.
    /// </summary>
    public async Task<GetExternalTollDailyInvoiceByCompanyResponse> GetExternalTollDailyInvoiceByCompanyAsync(string bearerToken, string companyId, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/api/ExternalDailyInvoice/get-by-company/{Uri.EscapeDataString(companyId)}");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Add query parameters
            var queryParams = new List<string>();
            if (dateFrom.HasValue)
            {
                queryParams.Add($"date={dateFrom.Value:yyyy-MM-dd}");
            }

            /*
            if (dateTo.HasValue)
            {
                queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-ddTHH:mm:ss.fffZ}");
            }
            */

            if (queryParams.Any())
            {
                httpRequest.RequestUri = new Uri($"{_options.BaseUrl}/api/ExternalDailyInvoice/get-by-company/{Uri.EscapeDataString(companyId)}?{string.Join("&", queryParams)}");
            }

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API returned error status {StatusCode}: {Content}", response.StatusCode, responseContent);
                return new GetExternalTollDailyInvoiceByCompanyResponse
                {
                    Reason = (int)response.StatusCode,
                    Message = $"API returned error: {response.StatusCode} - {responseContent}",
                    Result = null
                };
            }

            var getByCompanyResponse = JsonConvert.DeserializeObject<GetExternalTollDailyInvoiceByCompanyResponse>(responseContent);

            return getByCompanyResponse ?? new GetExternalTollDailyInvoiceByCompanyResponse
            {
                Reason = -1,
                Message = "Failed to deserialize invoices by company response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting external daily invoices by company: {CompanyId}", companyId);
            throw;
        }
    }

    /// <summary>
    /// Gets all external toll daily invoices with optional filters. Requires Authorization.
    /// </summary>
    public async Task<GetAllExternalTollDailyInvoicesResponse> GetExternalTollDailyInvoiceAllTollAsync(string bearerToken, DateTime? dateFrom = null, DateTime? dateTo = null, string? companyId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/api/ExternalDailyInvoice/get-all-toll");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // Add query parameters
            var queryParams = new List<string>();
            if (dateFrom.HasValue)
            {
                queryParams.Add($"dateFrom={dateFrom.Value:yyyy-MM-ddTHH:mm:ss.fffZ}");
            }
            if (dateTo.HasValue)
            {
                queryParams.Add($"dateTo={dateTo.Value:yyyy-MM-ddTHH:mm:ss.fffZ}");
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                queryParams.Add($"CompanyId={Uri.EscapeDataString(companyId)}");
            }

            if (queryParams.Any())
            {
                httpRequest.RequestUri = new Uri($"{_options.BaseUrl}/api/ExternalDailyInvoice/get-all-toll?{string.Join("&", queryParams)}");
            }

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var getAllTollResponse = JsonConvert.DeserializeObject<GetAllExternalTollDailyInvoicesResponse>(responseContent);

            return getAllTollResponse ?? new GetAllExternalTollDailyInvoicesResponse
            {
                Reason = -1,
                Message = "Failed to deserialize all toll invoices response",
                Result = new List<ExternalTollDailyInvoice>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all external toll daily invoices");
            throw;
        }
    }

    /// <summary>
    /// Gets company information for employees. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Company info response</returns>
    public async Task<GetCompanyInfoResponse> GetCompanyInfoAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/Employees/companyinfo");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var companyInfoResponse = JsonConvert.DeserializeObject<GetCompanyInfoResponse>(responseContent);

            return companyInfoResponse ?? new GetCompanyInfoResponse
            {
                Reason = -1,
                Message = "Failed to deserialize company info response",
                Result = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company info");
            throw;
        }
    }

    /// <summary>
    /// Gets all external vehicles. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External vehicles response</returns>
    public async Task<GetExternalVehiclesResponse> GetExternalVehiclesAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new ArgumentException("Bearer token is required", nameof(bearerToken));
        }

        // Create a temporary HttpClient with 3-minute timeout for this specific request
        using var tempHttpClient = new HttpClient
        {
            BaseAddress = new Uri(_options.BaseUrl),
            Timeout = TimeSpan.FromMinutes(3)
        };

        // Copy headers from the main HttpClient (skip restricted headers)
        foreach (var header in _httpClient.DefaultRequestHeaders)
        {
            try
            {
                if (!tempHttpClient.DefaultRequestHeaders.Contains(header.Key))
                {
                    tempHttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            catch
            {
                // Skip headers that can't be added (e.g., restricted headers)
            }
        }

        // Copy API key if present
        if (!string.IsNullOrEmpty(_options.ApiKey) && !tempHttpClient.DefaultRequestHeaders.Contains("X-API-Key"))
        {
            tempHttpClient.DefaultRequestHeaders.Add("X-API-Key", _options.ApiKey);
        }

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_options.BaseUrl}/ExternalVehicles");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await tempHttpClient.SendAsync(httpRequest, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP error getting external vehicles. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseContent);
                
                // Try to deserialize error response
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<GetExternalVehiclesResponse>(responseContent);
                    if (errorResponse != null)
                    {
                        return errorResponse;
                    }
                }
                catch
                {
                    // If deserialization fails, return error response with raw content
                }
                
                return new GetExternalVehiclesResponse
                {
                    Reason = (int)response.StatusCode,
                    Message = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}. Response: {responseContent}",
                    Result = new List<ExternalVehicle>()
                };
            }

            var externalVehiclesResponse = JsonConvert.DeserializeObject<GetExternalVehiclesResponse>(responseContent);

            return externalVehiclesResponse ?? new GetExternalVehiclesResponse
            {
                Reason = -1,
                Message = "Failed to deserialize external vehicles response",
                Result = new List<ExternalVehicle>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting external vehicles");
            throw;
        }
    }
}
