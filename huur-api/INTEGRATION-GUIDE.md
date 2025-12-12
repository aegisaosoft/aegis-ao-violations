# Integration Guide for huur-us Project

This guide shows you how to integrate the HuurApi library into your existing `huur-us` project.

## Prerequisites

- Your `huur-us` project is already using .NET 8.0 ✅
- The HuurApi library is built and ready to use

## Step 1: Add Projects to Your Solution

From your `huur-us` directory, run these commands:

```bash
# Navigate to your huur-us solution directory
cd C:\Users\Alexander\OneDrive\Documents\GitHub\huur-us

# Add the HuurApi projects to your solution
dotnet sln add ..\huur-api\HuurApi\HuurApi.csproj
dotnet sln add ..\huur-api\HuurApi.Models\HuurApi.Models.csproj
dotnet sln add ..\huur-api\HuurApi.Services\HuurApi.Services.csproj
```

## Step 2: Add Project References

Add these references to your `HuurUS.Web.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.StaticFiles" Version="2.2.0" />
  </ItemGroup>

  <!-- Add these project references -->
  <ItemGroup>
    <ProjectReference Include="..\..\huur-api\HuurApi\HuurApi.csproj" />
  </ItemGroup>

</Project>
```

## Step 3: Configure Services

In your `Program.cs` or `Startup.cs`, add the HuurApi services:

```csharp
using HuurApi;
using HuurApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HuurApi services
builder.Services.AddHuurApi(options =>
{
    options.BaseUrl = "https://agsm-huur-production-api.azurewebsites.net";
    options.DocumentationUrl = "https://agsm-huur-production-api.azurewebsites.net/";
    options.TimeoutSeconds = 30;
    options.MaxRetries = 3;
    options.PersistTokens = true;
    options.TokenConfigPath = "huur-api-tokens.json";
});

var app = builder.Build();
```

**Important**: The application now uses **full URLs** for all API calls, combining the base URL with the specific endpoint paths. This ensures proper authentication and routing.

## Step 4: Create a Property Service

Create a new service file `Services/PropertyService.cs`:

```csharp
using HuurApi;
using HuurApi.Models;

namespace HuurUS.Web.Services;

public interface IPropertyService
{
    Task<IEnumerable<Property>> GetAllPropertiesAsync();
    Task<IEnumerable<Property>> GetPropertiesByCityAsync(string city);
    Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchCriteria criteria);
    Task<Property?> GetPropertyByIdAsync(string id);
}

public interface IUserService
{
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request);
    Task<LoginResponse> LoginUserAsync(LoginRequest request);
    Task<AuthResponse> LogoutUserAsync(LogoutRequest request);
    Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request);
    Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);
    Task<User?> GetUserProfileAsync();
}

public class PropertyService : IPropertyService
{
    private readonly IHuurApiClient _huurApiClient;
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(IHuurApiClient huurApiClient, ILogger<PropertyService> logger)
    {
        _huurApiClient = huurApiClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
    {
        try
        {
            return await _huurApiClient.GetPropertiesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all properties");
            return Enumerable.Empty<Property>();
        }
    }

    public async Task<IEnumerable<Property>> GetPropertiesByCityAsync(string city)
    {
        try
        {
            return await _huurApiClient.GetPropertiesByCityAsync(city);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting properties for city: {City}", city);
            return Enumerable.Empty<Property>();
        }
    }

    public async Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchCriteria criteria)
    {
        try
        {
            return await _huurApiClient.SearchPropertiesAsync(criteria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching properties with criteria: {@Criteria}", criteria);
            return Enumerable.Empty<Property>();
        }
    }

    public async Task<Property?> GetPropertyByIdAsync(string id)
    {
        try
        {
            return await _huurApiClient.GetPropertyByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting property by ID: {Id}", id);
            return null;
        }
    }
}

public class UserService : IUserService
{
    private readonly IHuurApiClient _huurApiClient;
    private readonly ILogger<UserService> _logger;

    public UserService(IHuurApiClient huurApiClient, ILogger<UserService> logger)
    {
        _huurApiClient = huurApiClient;
        _logger = logger;
    }

    public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request)
    {
        try
        {
            return await _huurApiClient.RegisterUserAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Username}", request.Username);
            return new RegisterResponse 
            { 
                Message = "Registration failed",
                User = new User()
            };
        }
    }

    public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
    {
        try
        {
            return await _huurApiClient.LoginUserAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user: {Username}", request.Username);
            return new LoginResponse 
            { 
                Token = "",
                User = new User()
            };
        }
    }

    public async Task<AuthResponse> LogoutUserAsync(LogoutRequest request)
    {
        try
        {
            return await _huurApiClient.LogoutUserAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging out user");
            return new AuthResponse 
            { 
                Success = false,
                Message = "Logout failed"
            };
        }
    }

    public async Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            return await _huurApiClient.ChangePasswordAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return new AuthResponse 
            { 
                Success = false,
                Message = "Password change failed"
            };
        }
    }

    public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            return await _huurApiClient.ResetPasswordAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for: {Email}", request.Email);
            return new AuthResponse 
            { 
                Success = false,
                Message = "Password reset failed"
            };
        }
    }

    public async Task<User?> GetUserProfileAsync()
    {
        try
        {
            return await _huurApiClient.GetUserProfileAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return null;
        }
    }
}
```

## Step 5: Register the Property Service

Add this to your `Program.cs`:

```csharp
// Add the property service
builder.Services.AddScoped<IPropertyService, PropertyService>();

// Add the user service  
builder.Services.AddScoped<IUserService, UserService>();
```

## Step 6: Use in Controllers

Create or update your controller to use the property service:

```csharp
using HuurUS.Web.Services;
using HuurApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HuurUS.Web.Controllers;

public class PropertiesController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyService propertyService, ILogger<PropertiesController> logger)
    {
        _propertyService = propertyService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return View(properties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Properties Index");
            return View("Error");
        }
    }

    public async Task<IActionResult> Search(string city, decimal? minPrice, decimal? maxPrice)
    {
        try
        {
            var criteria = new PropertySearchCriteria
            {
                City = city,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            var properties = await _propertyService.SearchPropertiesAsync(criteria);
            return View("Index", properties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Properties Search");
            return View("Error");
        }
    }

    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Properties Details for ID: {Id}", id);
            return View("Error");
        }
    }
}
```

## Step 7: Create Views

Create a view for displaying properties (`Views/Properties/Index.cshtml`):

```html
@model IEnumerable<HuurApi.Models.Property>

<h2>Available Properties</h2>

<div class="row">
    @foreach (var property in Model)
    {
        <div class="col-md-4 mb-4">
            <div class="card">
                @if (property.Images.Any())
                {
                    <img src="@property.Images.First()" class="card-img-top" alt="@property.Title">
                }
                <div class="card-body">
                    <h5 class="card-title">@property.Title</h5>
                    <p class="card-text">@property.Description</p>
                    <p class="card-text">
                        <strong>Price:</strong> @property.Price @property.Currency<br>
                        <strong>Location:</strong> @property.Location.City, @property.Location.Country<br>
                        <strong>Bedrooms:</strong> @property.Bedrooms | <strong>Bathrooms:</strong> @property.Bathrooms
                    </p>
                    <a href="@Url.Action("Details", new { id = property.Id })" class="btn btn-primary">View Details</a>
                </div>
            </div>
        </div>
    }
</div>
```

## Step 8: Add Configuration (Optional)

Add configuration to your `appsettings.json`:

```json
{
  "HuurApi": {
    "BaseUrl": "https://agsm-huur-production-api.azurewebsites.net",
    "TimeoutSeconds": 30,
    "MaxRetries": 3
  }
}
```

## Step 9: Test the Integration

1. Build your solution:
   ```bash
   dotnet build
   ```

2. Run your application:
   ```bash
   dotnet run
   ```

3. Navigate to `/Properties` to see the properties from the Huur API

## Troubleshooting

### Common Issues

1. **Build Errors**: Make sure all project references are correct
2. **Runtime Errors**: Check that services are properly registered
3. **API Errors**: Verify the API endpoint is accessible and returns expected data

### Debug Tips

- Use the browser's developer tools to see network requests
- Check the application logs for detailed error information
- Verify the API endpoint in your browser: https://agsm-huur-production-api.azurewebsites.net/swagger/index.html

## Next Steps

- Add caching for better performance
- Implement pagination for large result sets
- Add filtering and sorting capabilities
- Create a search form for user input
- Add property favorites functionality

## Support

If you encounter any issues during integration, check the main README.md file or create an issue in the repository.
