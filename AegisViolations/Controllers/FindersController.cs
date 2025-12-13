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

namespace AegisViolations.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FindersController : ControllerBase
{
    private readonly ILogger<FindersController> _logger;

    public FindersController(ILogger<FindersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetFinders()
    {
        try
        {
            // Use the reflection-based method that handles configuration errors gracefully
            var result = LoadFindersInfoFromDlls();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading finders");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{state}")]
    public IActionResult GetFindersByState(string state)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(state))
            {
                return BadRequest(new { error = "State parameter is required" });
            }

            var allFinders = LoadFindersInfoFromDlls();
            
            _logger.LogInformation($"Loaded {allFinders.Count} total finders. Requested state: {state}");
            foreach (var finder in allFinders)
            {
                _logger.LogDebug($"Finder: {finder.ClassName}, State: '{finder.State}'");
            }
            
            // Filter finders: return finders matching the requested state OR "USA"
            var filteredFinders = allFinders
                .Where(f => string.Equals(f.State, state, StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(f.State, "USA", StringComparison.OrdinalIgnoreCase))
                .ToList();

            _logger.LogInformation($"Filtered to {filteredFinders.Count} finders for state {state}");
            
            // Always return a list, even if empty
            return Ok(filteredFinders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading finders by state");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private List<IAegisAPIFinder> LoadFindersFromDlls()
    {
        var finders = new List<IAegisAPIFinder>();
        
        // Get the Finders folder path relative to the application base directory
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
                        if (Activator.CreateInstance(type) is IAegisAPIFinder finder)
                        {
                            finders.Add(finder);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to create instance of {type.Name} from {Path.GetFileName(dll)}. Attempting to read properties via reflection.");
                        // Try to get Name and Link via reflection if instantiation fails
                        // This won't add to the list, but we'll handle it in GetFinders
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

    private List<FinderInfo> LoadFindersInfoFromDlls()
    {
        var findersInfo = new List<FinderInfo>();
        
        var findersDir = Path.Combine(AppContext.BaseDirectory, "Finders");
        
        if (!Directory.Exists(findersDir))
        {
            _logger.LogWarning($"Finders directory not found: {findersDir}");
            return findersInfo;
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
                    var finderInfo = new FinderInfo
                    {
                        ClassName = type.Name
                    };

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
                            finderInfo.Name = finder.Name;
                            finderInfo.Link = finder.Link;
                            
                            // Try to get State property via reflection if it exists
                            // Use BindingFlags to ensure we find public instance properties
                            var stateProperty = type.GetProperty("State", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                            if (stateProperty != null && stateProperty.CanRead)
                            {
                                try
                                {
                                    var stateValue = stateProperty.GetValue(finder);
                                    finderInfo.State = stateValue?.ToString() ?? string.Empty;
                                    _logger.LogInformation($"Found finder {type.Name} with State: '{finderInfo.State}'");
                                }
                                catch (Exception stateEx)
                                {
                                    _logger.LogWarning(stateEx, $"Failed to read State property for {type.Name}");
                                    finderInfo.State = string.Empty;
                                }
                            }
                            else
                            {
                                _logger.LogWarning($"Finder {type.Name} does not have a State property");
                                finderInfo.State = string.Empty;
                            }
                            
                            findersInfo.Add(finderInfo);
                            
                            // Dispose if it implements IDisposable
                            if (finder is IDisposable disposable)
                            {
                                disposable.Dispose();
                            }
                        }
                        else if (instance == null)
                        {
                            throw new MissingMethodException($"No suitable constructor found for {type.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to create instance of {type.Name} from {Path.GetFileName(dll)}. Configuration may be missing.");
                        
                        // Since Name and Link are simple properties, try to read them from the type definition
                        // For expression-bodied properties like "public string Name => \"Broward Clerk\";"
                        // we can't easily extract the value without instantiating, but we can provide fallback
                        finderInfo.Name = type.Name.Replace("Finder", "").Replace("Aegis", "");
                        finderInfo.Link = "Configuration required";
                        finderInfo.State = string.Empty;
                        findersInfo.Add(finderInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to load assembly: {Path.GetFileName(dll)}");
            }
        }

        return findersInfo;
    }
}

public class FinderInfo
{
    public string ClassName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}

