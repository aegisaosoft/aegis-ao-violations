# HuurApi - .NET Library for Huur API

A .NET 8.0 library for interacting with the Huur API, designed to be easily integrated into your existing projects.

## Features

- **Property Management**: Get all properties, search by ID, city, price range, and more
- **Advanced Search**: Search with multiple criteria including location, amenities, and availability
- **User Authentication**: Complete user authentication system with 6 core commands
  - Register new users
  - Login/logout functionality
  - Password management (change/reset)
  - User profile management
- **Async Operations**: All operations are asynchronous for better performance
- **Dependency Injection**: Built-in support for Microsoft.Extensions.DependencyInjection
- **Configurable**: Customizable timeout, retry policies, and API endpoints
- **Logging**: Integrated logging support
- **Error Handling**: Comprehensive error handling with detailed logging
- **Token Management**: Automatic JWT token handling for authenticated requests
- **Configuration-Based Tokens**: Store and retrieve tokens from configuration files
- **Persistent Authentication**: Tokens are automatically saved and restored between sessions

## Installation

### Option 1: Reference the Projects (Recommended for Development)

1. Add the projects to your solution:
   ```bash
   # Add the main library
   dotnet sln add HuurApi/HuurApi.csproj
   
   # Add the models project
   dotnet sln add HuurApi.Models/HuurApi.Models.csproj
   
   # Add the services project
   dotnet sln add HuurApi.Services/HuurApi.Services.csproj
   ```

2. Add project references to your project:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\HuurApi\HuurApi.csproj" />
   </ItemGroup>
   ```

### Option 2: Build and Reference the DLL

1. Build the solution:
   ```bash
   dotnet build --configuration Release
   ```

2. Reference the generated DLL from `HuurApi/bin/Release/net8.0/HuurApi.dll`

## Quick Start

### 1. Register Services

```csharp
using HuurApi;
using HuurApi.Models;

var services = new ServiceCollection();

// Add the Huur API client with default configuration
services.AddHuurApi();

// Or with custom configuration
services.AddHuurApi(options =>
{
    options.BaseUrl = "https://agsm-huur-production-api.azurewebsites.net";
    options.TimeoutSeconds = 30;
    options.MaxRetries = 3;
    // options.ApiKey = "your-api-key-here"; // If required
});
```

### 2. Use the Client

```csharp
public class PropertyService
{
    private readonly IHuurApiClient _huurApiClient;

    public PropertyService(IHuurApiClient huurApiClient)
    {
        _huurApiClient = huurApiClient;
    }

    public async Task<IEnumerable<Property>> GetPropertiesInCityAsync(string city)
    {
        return await _huurApiClient.GetPropertiesByCityAsync(city);
    }

    public async Task<IEnumerable<Property>> SearchPropertiesAsync(decimal minPrice, decimal maxPrice)
    {
        return await _huurApiClient.GetPropertiesByPriceRangeAsync(minPrice, maxPrice);
    }
}
```

## API Methods

### Property Operations

- `GetPropertiesAsync()` - Get all available properties
- `GetPropertyByIdAsync(string id)` - Get a specific property by ID
- `GetPropertiesByCityAsync(string city)` - Get properties in a specific city
- `GetPropertiesByPriceRangeAsync(decimal minPrice, decimal maxPrice)` - Get properties within a price range

### Authentication Operations

- `RegisterUserAsync(RegisterRequest request)` - Register a new user
- `LoginUserAsync(LoginRequest request)` - Authenticate user and get token
- `LogoutUserAsync(LogoutRequest request)` - Logout user and invalidate token
- `ChangePasswordAsync(ChangePasswordRequest request)` - Change user password
- `ResetPasswordAsync(ResetPasswordRequest request)` - Initiate password reset
- `GetUserProfileAsync()` - Get current user's profile information

### Token Management Operations

- `HasValidTokensAsync()` - Check if valid tokens are available
- `GetCurrentTokenAsync()` - Get the current valid access token

### Advanced Search

```csharp
var searchCriteria = new PropertySearchCriteria
{
    City = "Amsterdam",
    MinPrice = 800,
    MaxPrice = 2000,
    MinBedrooms = 2,
    Furnished = true,
    PetsAllowed = true,
    AvailableFrom = DateTime.Now.AddDays(30)
};

var results = await _huurApiClient.SearchPropertiesAsync(searchCriteria);
```

### Authentication Examples

```csharp
// Register a new user
var registerRequest = new RegisterRequest
{
    Username = "john.doe",
    Email = "john.doe@example.com",
    Password = "SecurePassword123!",
    ConfirmPassword = "SecurePassword123!",
    FirstName = "John",
    LastName = "Doe"
};
var registerResponse = await _huurApiClient.RegisterUserAsync(registerRequest);

// Login user
var loginRequest = new LoginRequest
{
    Username = "john.doe",
    Password = "SecurePassword123!"
};
var loginResponse = await _huurApiClient.LoginUserAsync(loginRequest);

// Get user profile (requires authentication)
var userProfile = await _huurApiClient.GetUserProfileAsync();

// Change password
var changePasswordRequest = new ChangePasswordRequest
{
    CurrentPassword = "SecurePassword123!",
    NewPassword = "NewSecurePassword456!",
    ConfirmNewPassword = "NewSecurePassword456!"
};
var changeResult = await _huurApiClient.ChangePasswordAsync(changePasswordRequest);

// Logout user
var logoutRequest = new LogoutRequest
{
    LogoutFromAllDevices = false
};
var logoutResult = await _huurApiClient.LogoutUserAsync(logoutRequest);

// Token Management
var hasValidTokens = await _huurApiClient.HasValidTokensAsync();
var currentToken = await _huurApiClient.GetCurrentTokenAsync();

## Configuration Options

The `HuurApiOptions` class provides the following configuration options:

```csharp
public class HuurApiOptions
{
    public string BaseUrl { get; set; } = "https://agsm-huur-production-api.azurewebsites.net";
    public string DocumentationUrl { get; set; } = "https://agsm-huur-production-api.azurewebsites.net/";
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
    public string TokenConfigPath { get; set; } = "huur-api-config.json";
    public bool PersistTokens { get; set; } = true;
}
```

### Configuration Properties

- **BaseUrl**: The base URL for the Huur API (default: `https://agsm-huur-production-api.azurewebsites.net`)
- **DocumentationUrl**: The documentation URL for the Huur API (default: `https://agsm-huur-production-api.azurewebsites.net/`)
- **ApiKey**: Optional API key for authentication
- **TimeoutSeconds**: HTTP request timeout in seconds (default: 30)
- **MaxRetries**: Maximum number of retry attempts for failed requests (default: 3)
- **RetryDelayMs**: Delay between retry attempts in milliseconds (default: 1000)
- **TokenConfigPath**: Path to the token configuration file (default: `huur-api-config.json`)
- **PersistTokens**: Whether to persist authentication tokens (default: `true`)

## Token Configuration

The library automatically manages authentication tokens and stores them in a configuration file. By default, tokens are stored in the user's AppData folder for security.

### Token Storage Features

- **Automatic Persistence**: Tokens are automatically saved after login and restored on startup
- **Secure Storage**: Tokens are stored in the user's AppData folder by default
- **Expiration Handling**: Expired tokens are automatically detected and cleared
- **Configurable Path**: Custom token file paths can be specified

### Configuration File Location

- **Default**: `%APPDATA%\HuurApi\huur-api-config.json`
- **Custom**: Set `TokenConfigPath` in options to specify a custom location
- **Security**: Tokens are stored in JSON format with proper encryption considerations

### Token Configuration Structure

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "tokenExpiresAt": "2024-12-31T23:59:59Z",
  "username": "john.doe",
  "lastUpdated": "2024-01-01T00:00:00Z",
  "isValid": true
}
```

## Integration with ASP.NET Core

```csharp
// Program.cs or Startup.cs
builder.Services.AddHuurApi(options =>
{
    options.BaseUrl = builder.Configuration["HuurApi:BaseUrl"];
    options.ApiKey = builder.Configuration["HuurApi:ApiKey"];
    options.TimeoutSeconds = int.Parse(builder.Configuration["HuurApi:TimeoutSeconds"] ?? "30");
    options.PersistTokens = bool.Parse(builder.Configuration["HuurApi:PersistTokens"] ?? "true");
    options.TokenConfigPath = builder.Configuration["HuurApi:TokenConfigPath"] ?? "huur-api-tokens.json";
});

// appsettings.json
{
  "HuurApi": {
    "BaseUrl": "https://agsm-huur-production-api.azurewebsites.net",
    "ApiKey": "your-api-key",
    "TimeoutSeconds": 30,
    "PersistTokens": true,
    "TokenConfigPath": "huur-api-tokens.json"
  }
}
```

## Error Handling

The library includes comprehensive error handling:

```csharp
try
{
    var properties = await _huurApiClient.GetPropertiesAsync();
    // Process properties
}
catch (HttpRequestException ex)
{
    // Handle HTTP errors (network issues, timeouts, etc.)
    _logger.LogError(ex, "HTTP error occurred while calling Huur API");
}
catch (JsonException ex)
{
    // Handle JSON parsing errors
    _logger.LogError(ex, "Failed to parse response from Huur API");
}
catch (Exception ex)
{
    // Handle other errors
    _logger.LogError(ex, "Unexpected error occurred");
}
```

## Logging

The library integrates with Microsoft.Extensions.Logging:

```csharp
// Configure logging in your application
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Information);
});
```

## Building and Testing

1. **Build the solution:**
   ```bash
   dotnet build
   ```

2. **Run the example:**
   ```bash
   cd HuurApi.Example
   dotnet run
   ```

3. **Run tests (if available):**
   ```bash
   dotnet test
   ```

## Project Structure

```
HuurApi/
├── HuurApi.sln                 # Solution file
├── HuurApi/                    # Main library project
│   ├── HuurApi.csproj         # Main project file
│   └── IHuurApiClient.cs      # Main interface
├── HuurApi.Models/             # Data models
│   ├── HuurApi.Models.csproj  # Models project file
│   └── Models/                 # Model classes
│       ├── Property.cs        # Property model
│       ├── PropertySearchCriteria.cs # Search criteria
│       └── HuurApiOptions.cs  # Configuration options
├── HuurApi.Services/           # Service implementations
│   ├── HuurApi.Services.csproj # Services project file
│   ├── HuurApiClient.cs       # Main implementation
│   └── ServiceCollectionExtensions.cs # DI extensions
└── HuurApi.Example/            # Example usage
    ├── HuurApi.Example.csproj  # Example project file
    ├── Program.cs              # Main program
    └── ExampleService.cs       # Example service
```

## Dependencies

- **.NET 8.0** - Target framework
- **Microsoft.Extensions.Http** - HTTP client factory
- **Microsoft.Extensions.Logging.Abstractions** - Logging abstractions
- **Microsoft.Extensions.Options** - Configuration options
- **Newtonsoft.Json** - JSON serialization

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions, please create an issue in the repository or contact the development team.
