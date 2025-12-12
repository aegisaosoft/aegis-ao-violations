using Microsoft.AspNetCore.Mvc;
using AegisViolationsAPI;
using System.Reflection;
using HuurApi.Models;
using System.Collections.Concurrent;

namespace AegisViolations.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ViolationsController : ControllerBase
{
    private readonly ILogger<ViolationsController> _logger;

    public ViolationsController(ILogger<ViolationsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> GetViolations([FromBody] ViolationsRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request body is required" });
            }

            if (request.Cars == null || request.Cars.Count == 0)
            {
                return BadRequest(new { error = "At least one car is required" });
            }

            // Combine states from request and always include USA
            var statesToSearch = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (request.States != null)
            {
                foreach (var state in request.States)
                {
                    if (!string.IsNullOrWhiteSpace(state))
                    {
                        statesToSearch.Add(state.Trim().ToUpperInvariant());
                    }
                }
            }
            // Always include USA
            statesToSearch.Add("USA");

            _logger.LogInformation($"Searching violations for {request.Cars.Count} cars in states: {string.Join(", ", statesToSearch)}");

            // Load all finders
            var allFinders = LoadFindersFromDlls();
            
            // Filter finders by states (including USA)
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

            // Use thread-safe collection for violations
            var allViolations = new ConcurrentBag<ParkingViolation>();

            // Process all cars in parallel
            var carTasks = request.Cars
                .Where(car => !string.IsNullOrWhiteSpace(car.LicensePlate))
                .Select(async car =>
                {
                    var carState = string.IsNullOrWhiteSpace(car.State) ? string.Empty : car.State.Trim().ToUpperInvariant();
                    
                    // Get finders for this car's state + USA finders
                    var carFinders = relevantFinders
                        .Where(f =>
                        {
                            var stateProperty = f.GetType().GetProperty("State", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                            if (stateProperty != null && stateProperty.CanRead)
                            {
                                var finderState = stateProperty.GetValue(f)?.ToString() ?? string.Empty;
                                return string.Equals(finderState, carState, StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(finderState, "USA", StringComparison.OrdinalIgnoreCase);
                            }
                            return false;
                        })
                        .ToList();

                    _logger.LogInformation($"Searching for plate {car.LicensePlate} (state: {carState}) using {carFinders.Count} finders");

                    // Search with all finders in parallel for this car
                    var finderTasks = carFinders.Select(async finder =>
                    {
                        try
                        {
                            var violations = await finder.Find(car.LicensePlate, carState);
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
                });

            // Wait for all cars to be processed
            await Task.WhenAll(carTasks);

            var violationsList = allViolations.ToList();
            _logger.LogInformation($"Total violations found: {violationsList.Count}");

            return Ok(new ViolationsResponse
            {
                Violations = violationsList,
                TotalCount = violationsList.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting violations");
            return StatusCode(500, new { error = ex.Message });
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
}

