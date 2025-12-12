# HuurApi Example

This console application demonstrates all the available API calls in the HuurApi library.

## Features

The example demonstrates the following API operations:

1. **Authentication** - Sign in to get a bearer token
2. **User Management** - Get user information
3. **Car Management** - Get cars by license plate, get all cars, create cars
4. **Parking Violations** - Get parking violations for a date range
5. **EZ Pass Charges** - Get EZ Pass charges for a date range
6. **Payment Management** - Get failed payments and complete payments
7. **External Toll Daily Invoices** - Get all invoices, get by toll ID, get by plate, get by company (requires admin role)
8. **Company Management** - Get all companies and active companies (requires admin role)

## Usage

1. **Update Credentials**: Edit `Program.cs` and replace the test credentials with valid ones:
   ```csharp
   var signinRequest = new SigninRequest
   {
       Email = "your-email@example.com",  // Replace with your email
       Password = "your-password"         // Replace with your password
   };
   ```

2. **Run the Application**:
   ```bash
   dotnet run --project HuurApi.Example
   ```

## Expected Behavior

- The application will attempt to authenticate with the provided credentials
- If authentication succeeds, it will demonstrate all available API calls
- If authentication fails (like with the default test credentials), it will show an error message
- Some API calls require admin roles and may fail with regular user credentials

## API Endpoints Demonstrated

- `POST /UserAuth/signin` - User authentication
- `GET /Users` - Get user information
- `GET /Cars/CarListByLicensePlate` - Get cars by license plate
- `POST /Cars/all` - Get all cars with role filter
- `POST /Cars` - Create a new car
- `GET /ParkingViolations/{dateFrom}/{dateTo}` - Get parking violations
- `GET /EzpassCharges/{dateFrom}/{dateTo}` - Get EZ Pass charges
- `GET /FailedPayments` - Get failed payments
- `GET /CompletePayments` - Get complete payments
- `GET /api/ExternalDailyInvoice/get-all` - Get all external toll daily invoices (admin)
- `GET /api/ExternalDailyInvoice/get-all-toll` - Get all external toll daily invoices with filters (auth)
- `GET /api/ExternalDailyInvoice/get-by-toll-id` - Get external toll daily invoices by toll ID (admin)
- `GET /api/ExternalDailyInvoice/get-by-plate` - Get external toll daily invoices by plate (admin)
- `GET /api/ExternalDailyInvoice/get-by-company/{companyId}` - Get external toll daily invoices by company (admin)
- `GET /Companies/active` - Get active companies (admin)
- `GET /Companies/all` - Get all companies (admin)

## Error Handling

The application includes comprehensive error handling and will log:
- ✅ Success messages for successful operations
- ❌ Error messages for failed operations
- ⚠️ Warning messages for operations that might fail due to permissions

## Dependencies

- .NET 9.0
- HuurApi library
- Microsoft.Extensions.Logging.Console
