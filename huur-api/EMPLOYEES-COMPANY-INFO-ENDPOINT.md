# Employees Company Info Endpoint

## Overview
This document describes the new `GET /Employees/companyinfo` endpoint implementation in the HuurAPI client library, based on the [Swagger API documentation](https://agsm-back.azurewebsites.net/swagger/index.html).

## Endpoint Details

**URL:** `GET /Employees/companyinfo`  
**Authentication:** Required (Bearer Token)  
**Parameters:** None

## Response Model

```json
{
  "reason": 0,
  "message": "string",
  "stackTrace": "string",
  "result": {
    "companyInfo": {
      "id": "string",
      "email": "string",
      "phone": "string",
      "firstName": "string",
      "middleName": "string",
      "lastName": "string",
      "nickName": "string",
      "company": "string",
      "isCompany": true,
      "cityId": "string",
      "city": "string",
      "stateId": "string",
      "addressLine1": "string",
      "addressLine2": "string",
      "zipCode": "string",
      "birthDate": "2025-10-24T21:02:34.499Z",
      "employerId": "string",
      "imageURL": "string",
      "receiveNotification": true,
      "type": 0,
      "aboutMe": "string",
      "languages": "string",
      "rating": 0,
      "frequency": 0,
      "referralCode": "string",
      "requestDeleteAccount": true
    },
    "documents": [
      {
        "id": "string",
        "userId": "string",
        "role": 0,
        "dtd": 0,
        "value": "string",
        "description": "string",
        "isEmpty": true,
        "deleted": true,
        "createdBy": "2025-10-24T21:02:34.499Z"
      }
    ]
  }
}
```

## Implementation Files

### 1. HuurApi Project

#### Models (EmployeeModels.cs)
- `CompanyInfo` class - represents company information
- `EmployeeDocument` class - represents employee documents
- `CompanyInfoResult` class - wraps the result data
- `GetCompanyInfoResponse` class - response wrapper

#### Interface (IHuurApiClient.cs)
```csharp
/// <summary>
/// Gets company information for employees. Requires Authorization.
/// </summary>
/// <param name="bearerToken">Bearer access token (without the "Bearer " prefix)</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Company info response</returns>
Task<GetCompanyInfoResponse> GetCompanyInfoAsync(string bearerToken, CancellationToken cancellationToken = default);
```

#### Implementation (HuurApiClient.cs)
Located at line 1362-1392, implements the HTTP GET request to the Employees endpoint with proper authorization headers and error handling.

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

// Get company info
var companyInfoResponse = await apiClient.GetCompanyInfoAsync(token);
if (companyInfoResponse.Reason == 0 && companyInfoResponse.Result != null)
{
    var companyInfo = companyInfoResponse.Result.CompanyInfo;
    Console.WriteLine($"Company: {companyInfo.Company}");
    Console.WriteLine($"Contact: {companyInfo.FirstName} {companyInfo.LastName}");
    Console.WriteLine($"Email: {companyInfo.Email}");
    Console.WriteLine($"Location: {companyInfo.City}, {companyInfo.StateId}");
    Console.WriteLine($"Documents: {companyInfoResponse.Result.Documents.Count}");
}
```

### Example 2: Using in WPF Application

```csharp
private async void GetCompanyInfoButton_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var companyInfoResponse = await _huurApiClient.GetCompanyInfoAsync(_bearerToken!);
        
        if (companyInfoResponse.Reason == 0 && companyInfoResponse.Result != null)
        {
            var companyInfo = companyInfoResponse.Result.CompanyInfo;
            var documents = companyInfoResponse.Result.Documents;
            
            // Display company information
            CompanyNameLabel.Content = companyInfo.Company;
            ContactNameLabel.Content = $"{companyInfo.FirstName} {companyInfo.LastName}";
            EmailLabel.Content = companyInfo.Email;
            LocationLabel.Content = $"{companyInfo.City}, {companyInfo.StateId} {companyInfo.ZipCode}";
            DocumentsCountLabel.Content = $"Documents: {documents.Count}";
            
            MessageBox.Show($"Company info loaded successfully!\n" +
                          $"Company: {companyInfo.Company}\n" +
                          $"Contact: {companyInfo.FirstName} {companyInfo.LastName}\n" +
                          $"Documents: {documents.Count}", 
                          "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show($"Failed to get company info: {companyInfoResponse.Message}", 
                          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error getting company info: {ex.Message}", 
                      "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

### Example 3: Processing Documents

```csharp
var companyInfoResponse = await apiClient.GetCompanyInfoAsync(token);
if (companyInfoResponse.Reason == 0 && companyInfoResponse.Result != null)
{
    var documents = companyInfoResponse.Result.Documents;
    
    foreach (var document in documents)
    {
        if (!document.IsEmpty && !document.Deleted)
        {
            Console.WriteLine($"Document: {document.Description}");
            Console.WriteLine($"  - ID: {document.Id}");
            Console.WriteLine($"  - Role: {document.Role}");
            Console.WriteLine($"  - Value: {document.Value}");
            Console.WriteLine($"  - Created: {document.CreatedBy}");
        }
    }
}
```

## Data Models

### CompanyInfo Properties
- **Basic Info**: `Id`, `Email`, `Phone`, `FirstName`, `MiddleName`, `LastName`, `NickName`
- **Company Details**: `Company`, `IsCompany`, `EmployerId`
- **Location**: `CityId`, `City`, `StateId`, `AddressLine1`, `AddressLine2`, `ZipCode`
- **Personal**: `BirthDate`, `ImageURL`, `AboutMe`, `Languages`
- **Settings**: `ReceiveNotification`, `Type`, `Rating`, `Frequency`, `ReferralCode`, `RequestDeleteAccount`

### EmployeeDocument Properties
- **Identification**: `Id`, `UserId`, `Role`, `Dtd`
- **Content**: `Value`, `Description`
- **Status**: `IsEmpty`, `Deleted`, `CreatedBy`

## Testing

The implementation has been tested and verified:
- ✅ HuurApi project builds successfully
- ✅ HuurApi.Example project builds successfully
- ✅ Example project demonstrates usage
- ✅ All models properly initialized with default values

## Files Modified

1. `C:\Huur\huur-api\HuurApi\Models\EmployeeModels.cs` - **NEW** - Employee-related models
2. `C:\Huur\huur-api\HuurApi\Services\IHuurApiClient.cs` - Added interface method
3. `C:\Huur\huur-api\HuurApi\Services\HuurApiClient.cs` - Implemented HTTP call
4. `C:\Huur\huur-api\HuurApi.Example\Program.cs` - Added usage example

## Notes

- The endpoint requires authentication via Bearer token
- Returns comprehensive company and employee information
- Includes associated documents with the company info
- All string properties are initialized with `string.Empty` to avoid null reference warnings
- Lists are initialized with empty collections
- Includes proper error handling for authentication failures and HTTP errors
- No parameters required - returns all available company information for the authenticated user

## API Reference

This implementation is based on the official Swagger documentation available at:
[https://agsm-back.azurewebsites.net/swagger/index.html](https://agsm-back.azurewebsites.net/swagger/index.html)

The endpoint provides access to company information and associated documents for employees, making it useful for:
- Employee profile management
- Company information display
- Document management
- Contact information retrieval
- Location and address details
