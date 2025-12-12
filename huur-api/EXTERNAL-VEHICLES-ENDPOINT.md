# External Vehicles License Plates Endpoint

## Overview
This document describes the new `GET /ExternalVehicles/GetLicensePlates` endpoint implementation in the HuurAPI client library.

## Endpoint Details

**URL:** `GET /ExternalVehicles/GetLicensePlates`  
**Authentication:** Required (Bearer Token)  
**Parameters:** None

## Response Model

```json
{
  "result": [
    {
      "plateNumber": "1234567",
      "plateState": "AZ"
    },
    {
      "plateNumber": "01ELGG",
      "plateState": "FL"
    }
  ],
  "reason": 0,
  "message": null,
  "stackTrace": null
}
```

## Implementation Files

### 1. HuurApi Project

#### Models (CarModels.cs)
- `LicensePlate` class - represents a single license plate
- `GetLicensePlatesResponse` class - response wrapper with result list

#### Interface (IHuurApiClient.cs)
```csharp
/// <summary>
/// Gets all external vehicle license plates. Requires Authorization.
/// </summary>
/// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>License plates response</returns>
Task<GetLicensePlatesResponse> GetExternalVehicleLicensePlatesAsync(string bearerToken, CancellationToken cancellationToken = default);
```

#### Implementation (HuurApiClient.cs)
Located at line 582-612, implements the HTTP GET request to the ExternalVehicles endpoint with proper authorization headers and error handling.

### 2. HuurLoader Project

#### HuurApiService.cs
```csharp
public async Task<List<LicensePlate>> GetExternalVehicleLicensePlatesAsync()
{
    await EnsureSignedInAsync();

    var platesResponse = await _huurApiClient.GetExternalVehicleLicensePlatesAsync(_bearerToken!);
    
    if (platesResponse?.Result != null && platesResponse.Result.Any())
    {
        return platesResponse.Result;
    }

    return new List<LicensePlate>();
}
```

## Usage Examples

### Example 1: Using IHuurApiClient directly

```csharp
using HuurApi.Services;
using HuurApi.Models;

// Get the client from DI
var apiClient = serviceProvider.GetRequiredService<IHuurApiClient>();

// Sign in first
var signinRequest = new SigninRequest
{
    Email = "your-email@example.com",
    Password = "your-password"
};
var signinResponse = await apiClient.SigninAsync(signinRequest);
var token = signinResponse.Result.Token;

// Get external vehicle license plates
var platesResponse = await apiClient.GetExternalVehicleLicensePlatesAsync(token);
if (platesResponse.Reason == 0 && platesResponse.Result != null)
{
    Console.WriteLine($"Found {platesResponse.Result.Count} plates:");
    foreach (var plate in platesResponse.Result)
    {
        Console.WriteLine($"  - {plate.PlateNumber} ({plate.PlateState})");
    }
}
```

### Example 2: Using HuurApiService (HuurLoader)

```csharp
using HuurLoader.Services;

// Get the service from DI
var huurApiService = serviceProvider.GetRequiredService<HuurApiService>();

// Get external vehicle license plates (handles authentication automatically)
var plates = await huurApiService.GetExternalVehicleLicensePlatesAsync();
Console.WriteLine($"Found {plates.Count} plates:");
foreach (var plate in plates)
{
    Console.WriteLine($"  - {plate.PlateNumber} ({plate.PlateState})");
}
```

### Example 3: Using in WPF Application (HuurFinderTest)

```csharp
private async void GetExternalPlatesButton_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var plates = await _huurApiService.GetExternalVehicleLicensePlatesAsync();
        
        // Filter for Florida plates only
        var floridaPlates = plates.Where(p => p.PlateState == "FL").ToList();
        
        _loadedPlates.Clear();
        foreach (var plate in floridaPlates)
        {
            _loadedPlates.Add(plate);
        }
        
        PlatesCountLabel.Content = $"{floridaPlates.Count} Florida plates loaded";
        MessageBox.Show($"Loaded {floridaPlates.Count} Florida plates from external vehicles.", 
            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error getting external vehicle plates: {ex.Message}", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

## Differences from `/Cars/GetLicensePlates`

| Feature | `/Cars/GetLicensePlates` | `/ExternalVehicles/GetLicensePlates` |
|---------|-------------------------|-------------------------------------|
| Endpoint | `/Cars/GetLicensePlates` | `/ExternalVehicles/GetLicensePlates` |
| Data Source | Internal car database | External vehicle database |
| Response Model | `GetLicensePlatesResponse` | `GetLicensePlatesResponse` (same) |
| Method Name | `GetLicensePlatesAsync()` | `GetExternalVehicleLicensePlatesAsync()` |

Both endpoints return the same model structure, making them interchangeable in code that processes license plate data.

## Testing

The implementation has been tested and verified:
- ✅ HuurApi project builds successfully
- ✅ HuurLoader project builds successfully
- ✅ HuurFinderTest project builds successfully
- ✅ Example project demonstrates usage

## Files Modified

1. `C:\Huur\huur-api\HuurApi\Services\IHuurApiClient.cs` - Added interface method
2. `C:\Huur\huur-api\HuurApi\Services\HuurApiClient.cs` - Implemented HTTP call
3. `C:\Huur\huur-broward\HuurLoader\Services\HuurApiService.cs` - Added service wrapper
4. `C:\Huur\huur-api\HuurApi.Example\Program.cs` - Added usage example

## Notes

- The endpoint requires authentication via Bearer token
- Returns the same `GetLicensePlatesResponse` model as the Cars endpoint
- Includes proper error handling for authentication failures and HTTP errors
- No parameters required - returns all available external vehicle plates
- Integrated with existing HuurApiService for automatic authentication management




