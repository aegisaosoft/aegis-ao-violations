using Microsoft.AspNetCore.Mvc;
using HuurApi.Services;
using Microsoft.Extensions.Options;
using HuurApi.Models;
using AegisViolations.Data;
using AegisViolations.Services;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AegisViolations.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HuurController : ControllerBase
{
    private readonly ILogger<HuurController> _logger;
    private readonly IHuurApiClient _huurApiClient;
    private readonly HuurApiOptions _options;
    private readonly IHuurApiAuthService _authService;
    private readonly ViolationsDbContext _context;

    public HuurController(
        ILogger<HuurController> logger,
        IHuurApiClient huurApiClient,
        IOptions<HuurApiOptions> options,
        IHuurApiAuthService authService,
        ViolationsDbContext context)
    {
        _logger = logger;
        _huurApiClient = huurApiClient;
        _options = options.Value;
        _authService = authService;
        _context = context;
    }

    /// <summary>
    /// Health check endpoint to verify Huur API connection
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "connected",
            baseUrl = _options.BaseUrl,
            documentationUrl = _options.DocumentationUrl,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Synchronize companies from Huur API to PostgreSQL database
    /// POST /api/Huur/synchronize-companies
    /// Uses credentials from settings table to authenticate automatically
    /// </summary>
    [HttpPost("synchronize-companies")]
    public async Task<IActionResult> SynchronizeCompanies()
    {
        try
        {
            _logger.LogInformation("Starting companies synchronization from Huur API...");

            // Get authenticated bearer token
            var bearerToken = await _authService.GetBearerTokenAsync();

            // Call Huur API to get active companies
            var companiesResponse = await _huurApiClient.GetCompaniesActiveAsync(bearerToken);

            if (companiesResponse.Reason != 0)
            {
                _logger.LogError("Failed to get companies from Huur API. Reason: {Reason}, Message: {Message}", 
                    companiesResponse.Reason, companiesResponse.Message);
                return BadRequest(new 
                { 
                    error = "Failed to get companies from Huur API",
                    reason = companiesResponse.Reason,
                    message = companiesResponse.Message
                });
            }

            if (companiesResponse.Result == null || companiesResponse.Result.Count == 0)
            {
                _logger.LogWarning("No companies returned from Huur API");
                return Ok(new
                {
                    message = "No companies found in Huur API",
                    companiesProcessed = 0,
                    companiesCreated = 0,
                    companiesUpdated = 0
                });
            }

            _logger.LogInformation("Retrieved {Count} companies from Huur API", companiesResponse.Result.Count);

            var createdCount = 0;
            var updatedCount = 0;
            var errors = new List<string>();

            // Process each company
            foreach (var huurCompany in companiesResponse.Result)
            {
                try
                {
                    // Convert Huur company ID (string) to UUID
                    if (!Guid.TryParse(huurCompany.Id, out var companyId))
                    {
                        _logger.LogWarning("Invalid company ID format from Huur API: {Id}", huurCompany.Id);
                        errors.Add($"Invalid company ID format: {huurCompany.Id}");
                        continue;
                    }

                    // Check if company exists in database using raw SQL
                    var companyExists = false;
                    Guid? existingCompanyId = null;
                    
                    try
                    {
                        await _context.Database.OpenConnectionAsync();
                        using var command = _context.Database.GetDbConnection().CreateCommand();
                        command.CommandText = "SELECT id FROM companies WHERE id = @companyId";
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@companyId";
                        parameter.Value = companyId;
                        command.Parameters.Add(parameter);

                        using var reader = await command.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            companyExists = true;
                            existingCompanyId = reader.GetGuid(0);
                        }
                    }
                    finally
                    {
                        await _context.Database.CloseConnectionAsync();
                    }

                    if (companyExists)
                    {
                        // Update existing company
                        await _context.Database.ExecuteSqlRawAsync(
                            @"UPDATE companies 
                              SET company_name = {0}, 
                                  is_active = {1},
                                  updated_at = CURRENT_TIMESTAMP
                              WHERE id = {2}",
                            huurCompany.Name ?? string.Empty,
                            huurCompany.IsActive,
                            companyId);

                        updatedCount++;
                        _logger.LogInformation("Updated company: {Id} - {Name}", companyId, huurCompany.Name);
                    }
                    else
                    {
                        // Create new company
                        // Generate email using company ID to ensure uniqueness
                        var companyEmail = $"company-{companyId}@huur-sync.local";

                        await _context.Database.ExecuteSqlRawAsync(
                            @"INSERT INTO companies (id, company_name, email, is_active, created_at, updated_at)
                              VALUES ({0}, {1}, {2}, {3}, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)",
                            companyId,
                            huurCompany.Name ?? string.Empty,
                            companyEmail,
                            huurCompany.IsActive);

                        createdCount++;
                        _logger.LogInformation("Created company: {Id} - {Name} with email {Email}", companyId, huurCompany.Name, companyEmail);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing company {Id} - {Name}", huurCompany.Id, huurCompany.Name);
                    errors.Add($"Error processing company {huurCompany.Id}: {ex.Message}");
                }
            }

            _logger.LogInformation("Companies synchronization completed. Created: {Created}, Updated: {Updated}", 
                createdCount, updatedCount);

            return Ok(new
            {
                message = "Companies synchronized successfully",
                companiesProcessed = companiesResponse.Result.Count,
                companiesCreated = createdCount,
                companiesUpdated = updatedCount,
                errors = errors.Count > 0 ? errors : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error synchronizing companies");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Synchronize vehicles from Huur API to PostgreSQL database
    /// POST /api/Huur/synchronize-vehicles
    /// Uses credentials from settings table to authenticate automatically
    /// Maps vehicles to companies based on ownerId
    /// </summary>
    [HttpPost("synchronize-vehicles")]
    public async Task<IActionResult> SynchronizeVehicles()
    {
        try
        {
            _logger.LogInformation("Starting vehicles synchronization from Huur API...");

            // Get authenticated bearer token
            var bearerToken = await _authService.GetBearerTokenAsync();

            // Call Huur API to get external vehicles
            GetExternalVehiclesResponse? vehiclesResponse = null;
            try
            {
                vehiclesResponse = await _huurApiClient.GetExternalVehiclesAsync(bearerToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling GetExternalVehiclesAsync. Status: {StatusCode}", 
                    ex.Data.Contains("StatusCode") ? ex.Data["StatusCode"] : "Unknown");
                return StatusCode(500, new 
                { 
                    error = "Failed to retrieve vehicles from Huur API",
                    details = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling GetExternalVehiclesAsync");
                return StatusCode(500, new 
                { 
                    error = "Failed to retrieve vehicles from Huur API",
                    details = ex.Message
                });
            }

            if (vehiclesResponse == null || vehiclesResponse.Reason != 0)
            {
                _logger.LogError("Failed to get vehicles from Huur API. Reason: {Reason}, Message: {Message}", 
                    vehiclesResponse.Reason, vehiclesResponse.Message);
                return BadRequest(new 
                { 
                    error = "Failed to get vehicles from Huur API",
                    reason = vehiclesResponse.Reason,
                    message = vehiclesResponse.Message
                });
            }

            if (vehiclesResponse.Result == null || vehiclesResponse.Result.Count == 0)
            {
                _logger.LogWarning("No vehicles returned from Huur API");
                return Ok(new
                {
                    message = "No vehicles found in Huur API",
                    vehiclesProcessed = 0,
                    vehiclesCreated = 0,
                    vehiclesUpdated = 0
                });
            }

            _logger.LogInformation("Retrieved {Count} vehicles from Huur API", vehiclesResponse.Result.Count);

            var createdCount = 0;
            var updatedCount = 0;
            var skippedCount = 0;
            var errors = new List<string>();

            // Process each vehicle
            foreach (var huurVehicle in vehiclesResponse.Result)
            {
                try
                {
                    // Convert ownerId (company ID) to UUID
                    if (string.IsNullOrEmpty(huurVehicle.OwnerId) || !Guid.TryParse(huurVehicle.OwnerId, out var companyId))
                    {
                        _logger.LogWarning("Invalid or missing ownerId for vehicle {Id}: {OwnerId}", huurVehicle.Id, huurVehicle.OwnerId);
                        skippedCount++;
                        errors.Add($"Vehicle {huurVehicle.Id}: Invalid or missing ownerId (company ID)");
                        continue;
                    }

                    // Verify company exists
                    var companyExists = false;
                    try
                    {
                        await _context.Database.OpenConnectionAsync();
                        using var companyCheckCommand = _context.Database.GetDbConnection().CreateCommand();
                        companyCheckCommand.CommandText = "SELECT COUNT(*) FROM companies WHERE id = @companyId";
                        var companyParam = companyCheckCommand.CreateParameter();
                        companyParam.ParameterName = "@companyId";
                        companyParam.Value = companyId;
                        companyCheckCommand.Parameters.Add(companyParam);

                        var companyCount = await companyCheckCommand.ExecuteScalarAsync();
                        companyExists = Convert.ToInt32(companyCount) > 0;
                    }
                    finally
                    {
                        await _context.Database.CloseConnectionAsync();
                    }

                    if (!companyExists)
                    {
                        _logger.LogWarning("Company {CompanyId} not found for vehicle {VehicleId}. Skipping vehicle.", companyId, huurVehicle.Id);
                        skippedCount++;
                        errors.Add($"Vehicle {huurVehicle.Id}: Company {companyId} not found. Please synchronize companies first.");
                        continue;
                    }

                    // Use license plate or tag as the identifier
                    var licensePlate = !string.IsNullOrEmpty(huurVehicle.LicensePlate) 
                        ? huurVehicle.LicensePlate 
                        : huurVehicle.Tag ?? string.Empty;

                    if (string.IsNullOrEmpty(licensePlate))
                    {
                        _logger.LogWarning("Vehicle {Id} has no license plate or tag. Skipping.", huurVehicle.Id);
                        skippedCount++;
                        errors.Add($"Vehicle {huurVehicle.Id}: No license plate or tag");
                        continue;
                    }

                    // Check if vehicle exists (by license_plate and company_id)
                    var vehicleExists = false;
                    Guid? existingVehicleId = null;
                    
                    try
                    {
                        await _context.Database.OpenConnectionAsync();
                        using var vehicleCheckCommand = _context.Database.GetDbConnection().CreateCommand();
                        vehicleCheckCommand.CommandText = "SELECT id FROM vehicles WHERE license_plate = @licensePlate AND company_id = @companyId";
                        var licenseParam = vehicleCheckCommand.CreateParameter();
                        licenseParam.ParameterName = "@licensePlate";
                        licenseParam.Value = licensePlate;
                        vehicleCheckCommand.Parameters.Add(licenseParam);

                        var companyParam = vehicleCheckCommand.CreateParameter();
                        companyParam.ParameterName = "@companyId";
                        companyParam.Value = companyId;
                        vehicleCheckCommand.Parameters.Add(companyParam);

                        using var reader = await vehicleCheckCommand.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            vehicleExists = true;
                            existingVehicleId = reader.GetGuid(0);
                        }
                    }
                    finally
                    {
                        await _context.Database.CloseConnectionAsync();
                    }

                    if (vehicleExists && existingVehicleId.HasValue)
                    {
                        // Update existing vehicle
                        await _context.Database.ExecuteSqlRawAsync(
                            @"UPDATE vehicles 
                              SET state = {0},
                                  updated_at = CURRENT_TIMESTAMP
                              WHERE id = {1}",
                            huurVehicle.State ?? string.Empty,
                            existingVehicleId.Value);

                        updatedCount++;
                        _logger.LogInformation("Updated vehicle: {LicensePlate} (Company: {CompanyId})", licensePlate, companyId);
                    }
                    else
                    {
                        // Create new vehicle
                        var vehicleId = Guid.NewGuid();
                        await _context.Database.ExecuteSqlRawAsync(
                            @"INSERT INTO vehicles (id, company_id, license_plate, state, created_at, updated_at)
                              VALUES ({0}, {1}, {2}, {3}, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)",
                            vehicleId,
                            companyId,
                            licensePlate,
                            huurVehicle.State ?? string.Empty);

                        createdCount++;
                        _logger.LogInformation("Created vehicle: {LicensePlate} (Company: {CompanyId})", licensePlate, companyId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing vehicle {Id} - {LicensePlate}", huurVehicle.Id, huurVehicle.LicensePlate ?? huurVehicle.Tag);
                    errors.Add($"Error processing vehicle {huurVehicle.Id}: {ex.Message}");
                }
            }

            _logger.LogInformation("Vehicles synchronization completed. Created: {Created}, Updated: {Updated}, Skipped: {Skipped}", 
                createdCount, updatedCount, skippedCount);

            return Ok(new
            {
                message = "Vehicles synchronized successfully",
                vehiclesProcessed = vehiclesResponse.Result.Count,
                vehiclesCreated = createdCount,
                vehiclesUpdated = updatedCount,
                vehiclesSkipped = skippedCount,
                errors = errors.Count > 0 ? errors : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error synchronizing vehicles");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

