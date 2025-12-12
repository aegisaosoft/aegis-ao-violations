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

namespace HuurApi.Services;

/// <summary>
/// Main interface for interacting with the Huur API
/// </summary>
public interface IHuurApiClient
{
    /// <summary>
    /// Signs in a user with email and password
    /// </summary>
    /// <param name="signinRequest">User sign-in credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sign-in response with token and user details</returns>
    Task<SigninResponse> SigninAsync(SigninRequest signinRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets EZ-Pass charges between two dates. Requires Authorization.
    /// </summary>
    /// <param name="dateFrom">Start date (as string, exactly as API expects in path)</param>
    /// <param name="dateTo">End date (as string, exactly as API expects in path)</param>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raw JSON response string</returns>
    Task<EzpassChargesResponse> GetEzpassChargesAsync(string dateFrom, string dateTo, string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets Parking Violations between two dates. Requires Authorization.
    /// </summary>
    /// <param name="dateFrom">Start date (as string, exactly as API expects in path)</param>
    /// <param name="dateTo">End date (as string, exactly as API expects in path)</param>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raw JSON response string</returns>
    Task<ParkingViolationsResponse> GetParkingViolationsAsync(string dateFrom, string dateTo, string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed payments. Requires Authorization with Owner or Employee role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="ownerId">Optional owner ID to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Failed payments response with detailed records</returns>
    Task<FailedPaymentsResponse> GetFailedPaymentsAsync(string bearerToken, string? ownerId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets complete payments. Requires Authorization with Owner or Employee role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="ownerId">Optional owner ID to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete payments response indicating operation success</returns>
    Task<CompletePaymentsResponse> GetCompletePaymentsAsync(string bearerToken, string? ownerId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new parking violation. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="parkingViolation">Parking violation data to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response indicating success or failure of the creation operation</returns>
    Task<CreateParkingViolationResponse> CreateParkingViolationAsync(string bearerToken, ParkingViolation parkingViolation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a parking violation by its ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Violation ID to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parking violation response with the requested violation</returns>
    Task<CreateParkingViolationResponse> GetViolationByIdAsync(string bearerToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a parking violation by its ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Violation ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response indicating success or failure of the deletion operation</returns>
    Task<CreateParkingViolationResponse> DeleteViolationAsync(string bearerToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets car list by license plate. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cars list response</returns>
    Task<CarListByLicensePlateResponse> GetCarListByLicensePlateAsync(string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new car. Requires Authorization with Owner or Employee role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Car creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Car creation response</returns>
    Task<CreateCarResponse> CreateCarAsync(string bearerToken, CreateCarRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all cars. Requires Authorization with Owner or Employee role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Request with role parameter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All cars response</returns>
    Task<GetCarsAllResponse> GetCarsAllAsync(string bearerToken, GetCarsAllRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all license plates. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>License plates response</returns>
    Task<GetLicensePlatesResponse> GetLicensePlatesAsync(string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all external vehicle license plates. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>License plates response</returns>
    Task<GetLicensePlatesResponse> GetExternalVehicleLicensePlatesAsync(string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users list response</returns>
    Task<GetUsersResponse> GetUsersAsync(string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all external toll daily invoices. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All external toll daily invoices response</returns>
    Task<GetAllExternalTollDailyInvoicesResponse> GetAllExternalTollDailyInvoicesAsync(string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets external toll daily invoice by ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Invoice ID to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External toll daily invoice response</returns>
    Task<GetExternalTollDailyInvoiceByIdResponse> GetExternalTollDailyInvoiceByIdAsync(string bearerToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets external toll daily invoices by toll ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Request with toll ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External toll daily invoices by toll ID response</returns>
    Task<GetExternalTollDailyInvoiceByTollIdResponse> GetExternalTollDailyInvoiceByTollIdAsync(string bearerToken, GetInvoiceByTollIdRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets external toll daily invoices by plate number. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Request with plate number and state</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External toll daily invoices by plate response</returns>
    Task<GetExternalTollDailyInvoiceByPlateResponse> GetExternalTollDailyInvoiceByPlateAsync(string bearerToken, GetInvoiceByPlateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new external toll daily invoice. Requires Authorization with Admin role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Invoice creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External toll daily invoice creation response</returns>
    Task<CreateExternalTollDailyInvoiceResponse> CreateExternalTollDailyInvoiceAsync(string bearerToken, CreateExternalTollDailyInvoiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing external toll daily invoice. Requires Authorization with Admin role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Invoice ID to update</param>
    /// <param name="request">Invoice update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External toll daily invoice update response</returns>
    Task<UpdateExternalTollDailyInvoiceResponse> UpdateExternalTollDailyInvoiceAsync(string bearerToken, string id, UpdateExternalTollDailyInvoiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an external toll daily invoice by ID. Requires Authorization with Admin role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Invoice ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External toll daily invoice deletion response</returns>
    Task<DeleteExternalTollDailyInvoiceResponse> DeleteExternalTollDailyInvoiceAsync(string bearerToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates payment status for multiple external toll daily invoices. Requires Authorization and admin role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Update payment status request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update payment status response</returns>
    Task<UpdatePaymentStatusResponse> UpdatePaymentStatusAsync(string bearerToken, UpdatePaymentStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a parking violation by ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Violation ID to update</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update response</returns>
    Task<UpdateParkingViolationResponse> UpdateParkingViolationAsync(string bearerToken, string id, UpdateParkingViolationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a parking violation by agency and notice number. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="agency">Agency name</param>
    /// <param name="noticeNumber">Notice number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parking violation response</returns>
    Task<GetParkingViolationByAgencyResponse> GetParkingViolationByAgencyAsync(string bearerToken, string agency, string noticeNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all parking violations with optional filters. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="dateFrom">Start date filter</param>
    /// <param name="dateTo">End date filter</param>
    /// <param name="companyId">Company ID filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parking violations response</returns>
    Task<ParkingViolationsResponse> GetAllParkingViolationsAsync(string bearerToken, DateTime? dateFrom = null, DateTime? dateTo = null, string? companyId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a parking violation by ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Violation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parking violation response</returns>
    Task<GetParkingViolationResponse> GetParkingViolationByIdAsync(string bearerToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new parking violation. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Create response</returns>
    Task<CreateParkingViolationResponse> CreateParkingViolationAsync(string bearerToken, CreateParkingViolationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a parking violation by ID. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="id">Violation ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Delete response</returns>
    Task<DeleteParkingViolationResponse> DeleteParkingViolationAsync(string bearerToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates payment status for multiple parking violations. Requires Authorization and admin role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="request">Update payment status request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update payment status response</returns>
    Task<UpdateViolationPaymentStatusResponse> UpdateViolationPaymentStatusAsync(string bearerToken, UpdateViolationPaymentStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all companies. Requires Authorization with Admin role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All companies response</returns>
    Task<GetCompaniesAllResponse> GetCompaniesAllAsync(string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active companies. Requires Authorization with Admin role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active companies response</returns>
    Task<GetCompaniesActiveResponse> GetCompaniesActiveAsync(string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets external toll daily invoices by company ID. Requires Authorization with Admin role.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="companyId">Company ID to filter by</param>
    /// <param name="dateFrom">Start date filter (optional)</param>
    /// <param name="dateTo">End date filter (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External toll daily invoices by company response</returns>
    Task<GetExternalTollDailyInvoiceByCompanyResponse> GetExternalTollDailyInvoiceByCompanyAsync(string bearerToken, string companyId, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all external toll daily invoices with optional filters. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="dateFrom">Start date filter (optional)</param>
    /// <param name="dateTo">End date filter (optional)</param>
    /// <param name="companyId">Company ID filter (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All external toll daily invoices response</returns>
    Task<GetAllExternalTollDailyInvoicesResponse> GetExternalTollDailyInvoiceAllTollAsync(string bearerToken, DateTime? dateFrom = null, DateTime? dateTo = null, string? companyId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets company information for employees. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Company info response</returns>
    Task<GetCompanyInfoResponse> GetCompanyInfoAsync(string bearerToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all external vehicles. Requires Authorization.
    /// </summary>
    /// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External vehicles response</returns>
    Task<GetExternalVehiclesResponse> GetExternalVehiclesAsync(string bearerToken, CancellationToken cancellationToken = default);
}
