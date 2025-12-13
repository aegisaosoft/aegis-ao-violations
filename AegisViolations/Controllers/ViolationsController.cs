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

using Microsoft.AspNetCore.Mvc;
using AegisViolationsAPI;
using System.Reflection;
using HuurApi.Models;
using System.Collections.Concurrent;
using AegisViolations.Data;
using AegisViolations.Models;
using AegisViolations.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using AegisViolations.Services;

namespace AegisViolations.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ViolationsController : ControllerBase
{
    private readonly ILogger<ViolationsController> _logger;
    private readonly ViolationsDbContext _context;
    private readonly IProgressTrackingService _progressTracking;

    public ViolationsController(ILogger<ViolationsController> logger, ViolationsDbContext context, IProgressTrackingService progressTracking)
    {
        _logger = logger;
        _context = context;
        _progressTracking = progressTracking;
    }

    /// <summary>
    /// Get progress status for a violation search request
    /// GET /api/Violations/progress/{requestId}
    /// </summary>
    [HttpGet("progress/{requestId}")]
    public IActionResult GetProgress([FromRoute] string requestId)
    {
        var progress = _progressTracking.GetProgress(requestId);
        if (progress == null)
        {
            return NotFound(new { error = "Progress tracker not found for the given request ID" });
        }

        return Ok(progress);
    }

    /// <summary>
    /// Get violations for a company within a date range
    /// GET /api/Violations?companyId={uuid}&dateFrom={YYYY-MM-DD}&dateTo={YYYY-MM-DD}
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetViolationsByCompany(
        [FromQuery] Guid companyId,
        [FromQuery] string dateFrom,
        [FromQuery] string dateTo)
    {
        try
        {
            // Validate date format
            if (!DateTime.TryParseExact(dateFrom, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var startDate))
            {
                return BadRequest(new { error = "Invalid dateFrom format. Expected YYYY-MM-DD" });
            }

            if (!DateTime.TryParseExact(dateTo, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var endDate))
            {
                return BadRequest(new { error = "Invalid dateTo format. Expected YYYY-MM-DD" });
            }

            if (startDate > endDate)
            {
                return BadRequest(new { error = "dateFrom must be before or equal to dateTo" });
            }

            _logger.LogInformation($"Retrieving violations for company {companyId} from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

            // Query violations from database
            var violations = await _context.Violations
                .Where(v => v.CompanyId == companyId &&
                           v.IsActive &&
                           ((v.IssueDate.HasValue && v.IssueDate.Value.Date >= startDate.Date && v.IssueDate.Value.Date <= endDate.Date) ||
                            (v.StartDate.HasValue && v.StartDate.Value.Date >= startDate.Date && v.StartDate.Value.Date <= endDate.Date) ||
                            (!v.IssueDate.HasValue && !v.StartDate.HasValue && v.CreatedAt.Date >= startDate.Date && v.CreatedAt.Date <= endDate.Date)))
                .OrderByDescending(v => v.IssueDate ?? v.StartDate ?? v.CreatedAt)
                .ToListAsync();

            _logger.LogInformation($"Found {violations.Count} violations for company {companyId}");

            return Ok(new
            {
                companyId = companyId,
                dateFrom = dateFrom,
                dateTo = dateTo,
                violations = violations.Select(v => new
                {
                    id = v.Id,
                    citationNumber = v.CitationNumber,
                    noticeNumber = v.NoticeNumber,
                    provider = v.Provider,
                    agency = v.Agency,
                    address = v.Address,
                    tag = v.Tag,
                    state = v.State,
                    issueDate = v.IssueDate,
                    startDate = v.StartDate,
                    endDate = v.EndDate,
                    amount = v.Amount,
                    currency = v.Currency,
                    paymentStatus = v.PaymentStatus,
                    fineType = v.FineType,
                    note = v.Note,
                    link = v.Link,
                    isActive = v.IsActive,
                    createdAt = v.CreatedAt,
                    updatedAt = v.UpdatedAt
                }),
                totalCount = violations.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting violations for company {companyId}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Search for violations using finders (POST endpoint)
    /// POST /api/Violations
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GetViolations([FromBody] ViolationsRequest request)
    {
        // Create progress tracker
        var requestId = _progressTracking.CreateProgressTracker();

        try
        {
            _progressTracking.UpdateProgress(requestId, 0, "Validating request");

            if (request == null)
            {
                _progressTracking.UpdateProgress(requestId, 0, "Error: Request body is required");
                return BadRequest(new { error = "Request body is required", requestId });
            }

            if (request.Cars == null || request.Cars.Count == 0)
            {
                _progressTracking.UpdateProgress(requestId, 0, "Error: At least one car is required");
                return BadRequest(new { error = "At least one car is required", requestId });
            }

            // Combine states from request
            var statesToSearch = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (request.States != null && request.States.Count > 0)
            {
                foreach (var state in request.States)
                {
                    if (!string.IsNullOrWhiteSpace(state))
                    {
                        statesToSearch.Add(state.Trim().ToUpperInvariant());
                    }
                }
            }

            if (statesToSearch.Count == 0)
            {
                _progressTracking.UpdateProgress(requestId, 0, "Error: At least one state must be specified");
                return BadRequest(new { error = "At least one state must be specified", requestId });
            }

            _logger.LogInformation($"Searching violations for {request.Cars.Count} cars in states: {string.Join(", ", statesToSearch)}");

            _progressTracking.UpdateProgress(requestId, 5, "Loading finders");

            // Load all finders
            var allFinders = LoadFindersFromDlls();
            
            // Filter finders by states from the request
            var relevantFinders = allFinders
                .Where(f => 
                {
                    var stateProperty = f.GetType().GetProperty("State", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if (stateProperty != null && stateProperty.CanRead)
                    {
                        var finderState = stateProperty.GetValue(f)?.ToString() ?? string.Empty;
                        return statesToSearch.Contains(finderState, StringComparer.OrdinalIgnoreCase);
                    }
                    return false;
                })
                .ToList();

            _logger.LogInformation($"Found {relevantFinders.Count} relevant finders for states: {string.Join(", ", statesToSearch)}");

            _progressTracking.UpdateProgress(requestId, 10, "Starting violation search");

            // Use thread-safe collection for violations
            var allViolations = new ConcurrentBag<ParkingViolation>();
            var validCars = request.Cars.Where(car => !string.IsNullOrWhiteSpace(car.LicensePlate)).ToList();
            var totalCars = validCars.Count;
            var processedCars = 0;

            // Process all cars in parallel
            var carTasks = validCars
                .Select(async car =>
                {
                    // Use the states list from request for all cars
                    _logger.LogInformation($"Searching for plate {car.LicensePlate} using {relevantFinders.Count} finders for states: {string.Join(", ", statesToSearch)}");

                    // Search with all relevant finders in parallel for this car
                    var finderTasks = relevantFinders.Select(async finder =>
                    {
                        try
                        {
                            // Get the finder's state to pass to Find method
                            var stateProperty = finder.GetType().GetProperty("State", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                            var finderState = stateProperty != null && stateProperty.CanRead 
                                ? stateProperty.GetValue(finder)?.ToString() ?? string.Empty 
                                : string.Empty;
                            
                            var violations = await finder.Find(car.LicensePlate, finderState);
                            if (violations != null && violations.Count > 0)
                            {
                                foreach (var violation in violations)
                                {
                                    allViolations.Add(violation);
                                }
                                _logger.LogInformation($"Found {violations.Count} violations for {car.LicensePlate} using {finder.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Error searching with finder {finder.Name} for plate {car.LicensePlate}");
                        }
                    });

                    await Task.WhenAll(finderTasks);

                    // Update progress after each car is processed (10% to 90%)
                    var newProcessed = Interlocked.Increment(ref processedCars);
                    var progress = 10 + (int)((double)newProcessed / totalCars * 80);
                    _progressTracking.UpdateProgress(requestId, progress, $"Processed {newProcessed}/{totalCars} vehicles");
                });

            // Wait for all cars to be processed
            await Task.WhenAll(carTasks);

            var violationsList = allViolations.ToList();
            _logger.LogInformation($"Total violations found: {violationsList.Count}");

            _progressTracking.UpdateProgress(requestId, 95, "Saving request record");

            // Track request in violations_requests table
            try
            {
                var requestRecord = new AegisViolations.Models.ViolationsRequest
                {
                    Id = Guid.NewGuid(),
                    CompanyId = null, // No company for this endpoint
                    VehicleCount = request.Cars?.Count ?? 0,
                    RequestsCount = (request.Cars?.Count ?? 0) * relevantFinders.Count, // Total finder calls
                    FindersCount = relevantFinders.Count,
                    RequestDateTime = DateTime.UtcNow,
                    ViolationsFound = violationsList.Count,
                    Requestor = GetRequestor()
                };
                _context.ViolationsRequests.Add(requestRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to save violation request record");
                // Don't fail the request if tracking fails
            }

            _progressTracking.UpdateProgress(requestId, 100, "Completed");

            return Ok(new ViolationsResponse
            {
                Violations = violationsList,
                TotalCount = violationsList.Count,
                RequestId = requestId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting violations");
            _progressTracking.UpdateProgress(requestId, 0, $"Error: {ex.Message}");
            return StatusCode(500, new { error = ex.Message, requestId });
        }
    }

    private List<IAegisAPIFinder> LoadFindersFromDlls()
    {
        var finders = new List<IAegisAPIFinder>();
        
        var findersDir = Path.Combine(AppContext.BaseDirectory, "Finders");
        
        if (!Directory.Exists(findersDir))
        {
            _logger.LogWarning($"Finders directory not found: {findersDir}");
            return finders;
        }

        foreach (var dll in Directory.EnumerateFiles(findersDir, "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(dll);
                var types = assembly.GetTypes()
                    .Where(t => typeof(IAegisAPIFinder).IsAssignableFrom(t) && 
                               t.IsClass && 
                               !t.IsAbstract);

                foreach (var type in types)
                {
                    try
                    {
                        // Try to instantiate - handle both parameterless and constructors with optional parameters
                        object? instance = null;
                        
                        // First, try parameterless constructor
                        try
                        {
                            instance = Activator.CreateInstance(type);
                        }
                        catch (MissingMethodException)
                        {
                            // If no parameterless constructor, try to find a constructor and invoke it with null/default values
                            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                            foreach (var constructor in constructors.OrderBy(c => c.GetParameters().Length))
                            {
                                var parameters = constructor.GetParameters();
                                // Build arguments: use default value for optional params, null for nullable/reference types
                                var args = new object?[parameters.Length];
                                for (int i = 0; i < parameters.Length; i++)
                                {
                                    var param = parameters[i];
                                    if (param.IsOptional)
                                    {
                                        args[i] = param.DefaultValue;
                                    }
                                    else if (param.ParameterType.IsClass || Nullable.GetUnderlyingType(param.ParameterType) != null)
                                    {
                                        args[i] = null;
                                    }
                                    else
                                    {
                                        // Value type - try to create default instance
                                        args[i] = Activator.CreateInstance(param.ParameterType);
                                    }
                                }
                                
                                try
                                {
                                    instance = constructor.Invoke(args);
                                    break;
                                }
                                catch
                                {
                                    // Try next constructor
                                    continue;
                                }
                            }
                        }
                        
                        if (instance is IAegisAPIFinder finder)
                        {
                            finders.Add(finder);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to create instance of {type.Name} from {Path.GetFileName(dll)}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to load assembly: {Path.GetFileName(dll)}");
            }
        }

        return finders;
    }

    /// <summary>
    /// Gets the requestor identifier from the HTTP request (Authorization header, user claim, or IP address)
    /// </summary>
    private string? GetRequestor()
    {
        try
        {
            // Try to get from Authorization header (email or user identifier)
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                // Extract user info from token if available
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return "Bearer Token";
                }
                return authHeader.Length > 50 ? authHeader.Substring(0, 50) : authHeader;
            }

            // Try to get from User claims
            if (User?.Identity?.IsAuthenticated == true && User.Identity.Name != null)
            {
                return User.Identity.Name;
            }

            // Fall back to IP address
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return $"IP: {ipAddress}";
            }

            return "Unknown";
        }
        catch
        {
            return "System";
        }
    }

    /// <summary>
    /// Search and save violations for all vehicles of a company within a date range
    /// POST /api/Violations/{company_id}
    /// </summary>
    [HttpPost("{companyId}")]
    public async Task<IActionResult> GetAndSaveCompanyViolations(
        [FromRoute] Guid companyId,
        [FromBody] CompanyViolationsRequest request)
    {
        // Create progress tracker
        var requestId = _progressTracking.CreateProgressTracker();

        try
        {
            _progressTracking.UpdateProgress(requestId, 0, "Validating request");

            if (request == null)
            {
                _progressTracking.UpdateProgress(requestId, 0, "Error: Request body is required");
                return BadRequest(new { error = "Request body is required", requestId });
            }

            // Validate date format
            if (!DateTime.TryParseExact(request.StartDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var startDate))
            {
                _progressTracking.UpdateProgress(requestId, 0, "Error: Invalid StartDate format");
                return BadRequest(new { error = "Invalid StartDate format. Expected YYYY-MM-DD", requestId });
            }

            if (!DateTime.TryParseExact(request.EndDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var endDate))
            {
                _progressTracking.UpdateProgress(requestId, 0, "Error: Invalid EndDate format");
                return BadRequest(new { error = "Invalid EndDate format. Expected YYYY-MM-DD", requestId });
            }

            if (startDate > endDate)
            {
                _progressTracking.UpdateProgress(requestId, 0, "Error: StartDate must be before or equal to EndDate");
                return BadRequest(new { error = "StartDate must be before or equal to EndDate", requestId });
            }

            // Prepare states to search
            var statesToSearch = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (request.States != null && request.States.Count > 0)
            {
                foreach (var state in request.States)
                {
                    if (!string.IsNullOrWhiteSpace(state))
                    {
                        statesToSearch.Add(state.Trim().ToUpperInvariant());
                    }
                }
            }
            
            if (statesToSearch.Count == 0)
            {
                _progressTracking.UpdateProgress(requestId, 0, "Error: At least one state must be specified");
                return BadRequest(new { error = "At least one state must be specified", requestId });
            }

            _logger.LogInformation($"Searching violations for company {companyId} from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd} in states: {string.Join(", ", statesToSearch)}");

            _progressTracking.UpdateProgress(requestId, 5, "Loading vehicles from database");

            // Get all vehicles for the company from the database using raw SQL
            var vehicles = new List<Vehicle>();
            try
            {
                await _context.Database.OpenConnectionAsync();
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SELECT id, company_id, license_plate, state FROM vehicles WHERE company_id = @companyId AND license_plate IS NOT NULL AND license_plate != ''";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@companyId";
                parameter.Value = companyId;
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    vehicles.Add(new Vehicle
                    {
                        Id = reader.GetGuid(0),
                        CompanyId = reader.GetGuid(1),
                        LicensePlate = reader.IsDBNull(2) ? null : reader.GetString(2),
                        State = reader.IsDBNull(3) ? null : reader.GetString(3)
                    });
                }
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }

            if (vehicles.Count == 0)
            {
                _logger.LogWarning($"No vehicles found for company {companyId}");
                _progressTracking.UpdateProgress(requestId, 100, "Completed: No vehicles found");
                return Ok(new CompanyViolationsResponse
                {
                    CompanyId = companyId,
                    VehiclesProcessed = 0,
                    ViolationsFound = 0,
                    ViolationsSaved = 0,
                    ViolationsUpdated = 0,
                    Message = "No vehicles found for this company",
                    RequestId = requestId
                });
            }

            _logger.LogInformation($"Found {vehicles.Count} vehicles for company {companyId}");

            _progressTracking.UpdateProgress(requestId, 10, "Loading finders");

            // Load all finders
            var allFinders = LoadFindersFromDlls();

            // Filter finders by states
            var relevantFinders = allFinders
                .Where(f =>
                {
                    var stateProperty = f.GetType().GetProperty("State", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if (stateProperty != null && stateProperty.CanRead)
                    {
                        var finderState = stateProperty.GetValue(f)?.ToString() ?? string.Empty;
                        return statesToSearch.Contains(finderState, StringComparer.OrdinalIgnoreCase);
                    }
                    return false;
                })
                .ToList();

            _logger.LogInformation($"Found {relevantFinders.Count} relevant finders for states: {string.Join(", ", statesToSearch)}");

            _progressTracking.UpdateProgress(requestId, 15, "Starting violation search");

            // Use thread-safe collections
            var allViolations = new ConcurrentBag<ParkingViolation>();
            var savedCount = 0;
            var updatedCount = 0;

            var validVehicles = vehicles.Where(v => !string.IsNullOrWhiteSpace(v.LicensePlate)).ToList();
            var totalVehicles = validVehicles.Count;
            var processedVehicles = 0;

            // Process all vehicles in parallel
            var vehicleTasks = validVehicles
                .Select(async vehicle =>
                {
                    var vehicleState = string.IsNullOrWhiteSpace(vehicle.State) ? string.Empty : vehicle.State.Trim().ToUpperInvariant();

                    // Get finders for this vehicle's state + USA finders
                    var vehicleFinders = relevantFinders
                        .Where(f =>
                        {
                            var stateProperty = f.GetType().GetProperty("State", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                            if (stateProperty != null && stateProperty.CanRead)
                            {
                                var finderState = stateProperty.GetValue(f)?.ToString() ?? string.Empty;
                                return string.Equals(finderState, vehicleState, StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(finderState, "USA", StringComparison.OrdinalIgnoreCase);
                            }
                            return false;
                        })
                        .ToList();

                    _logger.LogInformation($"Searching for plate {vehicle.LicensePlate} (state: {vehicleState}) using {vehicleFinders.Count} finders");

                    // Search with all finders in parallel for this vehicle
                    var finderTasks = vehicleFinders.Select(async finder =>
                    {
                        try
                        {
                            var violations = await finder.Find(vehicle.LicensePlate!, vehicleState);
                            if (violations != null && violations.Count > 0)
                            {
                                foreach (var violation in violations)
                                {
                                    // Filter violations by date range
                                    if (violation.IssueDate.HasValue)
                                    {
                                        var issueDate = violation.IssueDate.Value.Date;
                                        if (issueDate >= startDate.Date && issueDate <= endDate.Date)
                                        {
                                            allViolations.Add(violation);
                                        }
                                    }
                                    else if (violation.StartDate.HasValue)
                                    {
                                        var startDateValue = violation.StartDate.Value.Date;
                                        if (startDateValue >= startDate.Date && startDateValue <= endDate.Date)
                                        {
                                            allViolations.Add(violation);
                                        }
                                    }
                                    else
                                    {
                                        // If no date, include it (might be a violation without date)
                                        allViolations.Add(violation);
                                    }
                                }
                                _logger.LogInformation($"Found {violations.Count} violations for {vehicle.LicensePlate} using {finder.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Error searching with finder {finder.Name} for plate {vehicle.LicensePlate}");
                        }
                    });

                    await Task.WhenAll(finderTasks);

                    // Update progress after each vehicle is processed (15% to 70%)
                    var newProcessed = Interlocked.Increment(ref processedVehicles);
                    var progress = 15 + (int)((double)newProcessed / totalVehicles * 55);
                    _progressTracking.UpdateProgress(requestId, progress, $"Processed {newProcessed}/{totalVehicles} vehicles");
                });

            // Wait for all vehicles to be processed
            await Task.WhenAll(vehicleTasks);

            var violationsList = allViolations.ToList();
            _logger.LogInformation($"Total violations found: {violationsList.Count}");

            _progressTracking.UpdateProgress(requestId, 75, $"Saving {violationsList.Count} violations to database");

            var totalViolations = violationsList.Count;
            var savedViolations = 0;

            // Save or update violations in the database
            foreach (var violation in violationsList)
            {
                try
                {
                    // Convert ParkingViolation to Violation entity
                    var violationEntity = new Violation
                    {
                        CompanyId = companyId,
                        CitationNumber = violation.CitationNumber,
                        NoticeNumber = violation.NoticeNumber,
                        Provider = violation.Provider,
                        Agency = violation.Agency,
                        Address = violation.Address,
                        Tag = violation.Tag,
                        State = violation.State,
                        IssueDate = violation.IssueDate,
                        StartDate = violation.StartDate,
                        EndDate = violation.EndDate,
                        Amount = violation.Amount,
                        Currency = violation.Currency,
                        PaymentStatus = violation.PaymentStatus,
                        FineType = violation.FineType,
                        Note = violation.Note,
                        Link = violation.Link,
                        IsActive = violation.IsActive,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Check if violation already exists (by notice number or citation number)
                    Violation? existingViolation = null;
                    if (!string.IsNullOrWhiteSpace(violationEntity.NoticeNumber))
                    {
                        existingViolation = await _context.Violations
                            .FirstOrDefaultAsync(v => v.CompanyId == companyId && 
                                                     v.NoticeNumber == violationEntity.NoticeNumber);
                    }
                    else if (!string.IsNullOrWhiteSpace(violationEntity.CitationNumber))
                    {
                        existingViolation = await _context.Violations
                            .FirstOrDefaultAsync(v => v.CompanyId == companyId && 
                                                     v.CitationNumber == violationEntity.CitationNumber);
                    }

                    if (existingViolation != null)
                    {
                        // Update existing violation
                        existingViolation.CitationNumber = violationEntity.CitationNumber;
                        existingViolation.NoticeNumber = violationEntity.NoticeNumber;
                        existingViolation.Provider = violationEntity.Provider;
                        existingViolation.Agency = violationEntity.Agency;
                        existingViolation.Address = violationEntity.Address;
                        existingViolation.Tag = violationEntity.Tag;
                        existingViolation.State = violationEntity.State;
                        existingViolation.IssueDate = violationEntity.IssueDate;
                        existingViolation.StartDate = violationEntity.StartDate;
                        existingViolation.EndDate = violationEntity.EndDate;
                        existingViolation.Amount = violationEntity.Amount;
                        existingViolation.Currency = violationEntity.Currency;
                        existingViolation.PaymentStatus = violationEntity.PaymentStatus;
                        existingViolation.FineType = violationEntity.FineType;
                        existingViolation.Note = violationEntity.Note;
                        existingViolation.Link = violationEntity.Link;
                        existingViolation.IsActive = violationEntity.IsActive;
                        existingViolation.UpdatedAt = DateTime.UtcNow;

                        _context.Violations.Update(existingViolation);
                        Interlocked.Increment(ref updatedCount);
                    }
                    else
                    {
                        // Add new violation
                        _context.Violations.Add(violationEntity);
                        Interlocked.Increment(ref savedCount);
                    }

                    // Update progress for saving violations (75% to 90%)
                    var newSaved = Interlocked.Increment(ref savedViolations);
                    if (totalViolations > 0)
                    {
                        var progress = 75 + (int)((double)newSaved / totalViolations * 15);
                        _progressTracking.UpdateProgress(requestId, progress, $"Saved {newSaved}/{totalViolations} violations");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error saving violation for company {companyId}");
                }
            }

            // Save all changes to database
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Saved {savedCount} new violations and updated {updatedCount} existing violations for company {companyId}");

            _progressTracking.UpdateProgress(requestId, 95, "Saving request record");

            // Track request in violations_requests table
            try
            {
                var totalFinderCalls = vehicles.Count * relevantFinders.Count; // Estimate total finder calls
                var requestRecord = new AegisViolations.Models.ViolationsRequest
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    VehicleCount = vehicles.Count,
                    RequestsCount = totalFinderCalls,
                    FindersCount = relevantFinders.Count,
                    RequestDateTime = DateTime.UtcNow,
                    ViolationsFound = violationsList.Count,
                    Requestor = GetRequestor()
                };
                _context.ViolationsRequests.Add(requestRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to save violation request record");
                // Don't fail the request if tracking fails
            }

            _progressTracking.UpdateProgress(requestId, 100, "Completed");

            return Ok(new CompanyViolationsResponse
            {
                CompanyId = companyId,
                VehiclesProcessed = vehicles.Count,
                ViolationsFound = violationsList.Count,
                ViolationsSaved = savedCount,
                ViolationsUpdated = updatedCount,
                Message = $"Successfully processed {vehicles.Count} vehicles. Found {violationsList.Count} violations, saved {savedCount} new, updated {updatedCount} existing.",
                RequestId = requestId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting and saving violations for company {companyId}");
            _progressTracking.UpdateProgress(requestId, 0, $"Error: {ex.Message}");
            return StatusCode(500, new { error = ex.Message, requestId });
        }
    }
}

public class ViolationsRequest
{
    public List<CarInfo> Cars { get; set; } = new();
    public List<string>? States { get; set; }
}

public class CarInfo
{
    public string LicensePlate { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}

public class ViolationsResponse
{
    public List<ParkingViolation> Violations { get; set; } = new();
    public int TotalCount { get; set; }
    public string? RequestId { get; set; }
}

public class CompanyViolationsResponse
{
    public Guid CompanyId { get; set; }
    public int VehiclesProcessed { get; set; }
    public int ViolationsFound { get; set; }
    public int ViolationsSaved { get; set; }
    public int ViolationsUpdated { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? RequestId { get; set; }
}

