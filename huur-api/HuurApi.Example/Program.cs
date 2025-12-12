using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HuurApi.Services;
using HuurApi.Models;

namespace HuurApi.Example;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup dependency injection
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddHuurApi(options =>
        {
            options.BaseUrl = "https://agsm-back.azurewebsites.net";
        });

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var apiClient = serviceProvider.GetRequiredService<IHuurApiClient>();

        logger.LogInformation("=== HuurApi Example - All API Calls Demo ===");

        try
        {
            // Step 1: Sign in to get authentication token
            logger.LogInformation("Step 1: Authenticating...");
            var signinRequest = new SigninRequest
            {
                Email = "mirzoev.siyovush@outlook.com",
                Password = "Iroc@2020"
            };

            var signinResponse = await apiClient.SigninAsync(signinRequest);
            if (signinResponse.Reason != 0)
            {
                logger.LogError("❌ Authentication failed: {Message}", signinResponse.Message);
                return;
            }

            var token = signinResponse.Result.Token;
            logger.LogInformation("✅ Authentication successful! Token: {Token}", token.Substring(0, Math.Min(20, token.Length)) + "...");

            // Step 1a: Get license plates from Cars endpoint
            logger.LogInformation("\nStep 1a: Getting license plates from /Cars/GetLicensePlates...");
            var licensePlatesResponse = await apiClient.GetLicensePlatesAsync(token);
            if (licensePlatesResponse.Reason == 0 && licensePlatesResponse.Result != null)
            {
                logger.LogInformation("✅ License plates retrieved: {Count} plates found", licensePlatesResponse.Result.Count);
                foreach (var plate in licensePlatesResponse.Result.Take(5)) // Show first 5 plates
                {
                    logger.LogInformation("  - {PlateNumber} ({PlateState})", plate.PlateNumber, plate.PlateState);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get license plates: {Message}", licensePlatesResponse.Message);
            }

            // Step 1b: Get license plates from ExternalVehicles endpoint (NEW)
            logger.LogInformation("\nStep 1b: Getting license plates from /ExternalVehicles/GetLicensePlates...");
            var externalLicensePlatesResponse = await apiClient.GetExternalVehicleLicensePlatesAsync(token);
            if (externalLicensePlatesResponse.Reason == 0 && externalLicensePlatesResponse.Result != null)
            {
                logger.LogInformation("✅ External vehicle license plates retrieved: {Count} plates found", externalLicensePlatesResponse.Result.Count);
                foreach (var plate in externalLicensePlatesResponse.Result.Take(5)) // Show first 5 plates
                {
                    logger.LogInformation("  - {PlateNumber} ({PlateState})", plate.PlateNumber, plate.PlateState);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get external vehicle license plates: {Message}", externalLicensePlatesResponse.Message);
            }

            // Step 1c: Get company info from Employees endpoint (NEW)
            logger.LogInformation("\nStep 1c: Getting company info from /Employees/companyinfo...");
            var companyInfoResponse = await apiClient.GetCompaniesActiveAsync(token);
            if (companyInfoResponse.Reason == 0 && companyInfoResponse.Result != null)
            {
                logger.LogInformation("✅ Company info retrieved successfully");
                //var companyInfo = companyInfoResponse.Result.CompanyInfo;
                //logger.LogInformation("  - Company: {Company} ({IsCompany})", companyInfo.Company, companyInfo.IsCompany ? "Yes" : "No");
                //logger.LogInformation("  - Contact: {FirstName} {LastName} ({Email})", companyInfo.FirstName, companyInfo.LastName, companyInfo.Email);
                //logger.LogInformation("  - Location: {City}, {StateId} {ZipCode}", companyInfo.City, companyInfo.StateId, companyInfo.ZipCode);
                //logger.LogInformation("  - Documents: {Count} documents found", companyInfoResponse.Result.Documents.Count);
            }
            else
            {
                logger.LogWarning("❌ Failed to get company info: {Message}", companyInfoResponse.Message);
            }

            // Get external vehicles
            logger.LogInformation("\nGetting external vehicles...");
            var vehiclesResponse = await apiClient.GetExternalVehiclesAsync(token);
            if (vehiclesResponse.Reason == 0 && vehiclesResponse.Result != null)
            {

            }
            else
            {
                logger.LogWarning("❌ Failed to get external vehicles: {Message}", vehiclesResponse.Message);
            }

            // Step 2: Get user information
            logger.LogInformation("\nStep 2: Getting user information...");
            var usersResponse = await apiClient.GetUsersAsync(token);
            if (usersResponse.Reason == 0 && usersResponse.Result != null)
            {
                logger.LogInformation("✅ User info retrieved: {FirstName} {LastName} ({Email})", 
                    usersResponse.Result.FirstName, usersResponse.Result.LastName, usersResponse.Result.Email);
            }
            else
            {
                logger.LogWarning("❌ Failed to get user info: {Message}", usersResponse.Message);
            }

            /*
            // Step 3: Get cars by license plate
            logger.LogInformation("\nStep 3: Getting cars by license plate...");
            var carsResponse = await apiClient.GetCarListByLicensePlateAsync(token);
            if (carsResponse.Reason == 0 && carsResponse.Result != null)
            {
                logger.LogInformation("✅ Cars retrieved: {Count} cars found", carsResponse.Result.Count);
                foreach (var car in carsResponse.Result.Take(3)) // Show first 3 cars
                {
                    logger.LogInformation("  - {PlateNumber} {PlateState} ({Year} {Brand} {Model})", 
                        car.CarPlateNumber, car.CarPlateNumberState, car.Year, car.CarBrand, car.CarModel);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get cars: {Message}", carsResponse.Message);
            }

            // Step 4: Get all cars
            logger.LogInformation("\nStep 4: Getting all cars...");
            var carsAllRequest = new GetCarsAllRequest { Role = 0 };
            var carsAllResponse = await apiClient.GetCarsAllAsync(token, carsAllRequest);
            if (carsAllResponse.Reason == 0 && carsAllResponse.Result != null)
            {
                logger.LogInformation("✅ All cars retrieved: {Count} cars found", carsAllResponse.Result.Count);
                foreach (var car in carsAllResponse.Result.Take(3)) // Show first 3 cars
                {
                    logger.LogInformation("  - {Title} ({Brand} {Model}) - ${PricePerWeek}/week", 
                        car.Title, car.Brand, car.Model, car.PricePerWeek);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get all cars: {Message}", carsAllResponse.Message);
            }
            

            // Step 5: Get parking violations
            logger.LogInformation("\nStep 5: Getting parking violations...");
            var dateFrom = DateTime.Now.AddDays(-365).ToString("yyyy-MM-dd");
            var dateTo = DateTime.Now.ToString("yyyy-MM-dd");
            var violationsResponse = await apiClient.GetParkingViolationsAsync(dateFrom, dateTo, token);
            if (violationsResponse.Reason == 0 && violationsResponse.Result != null)
            {
                logger.LogInformation("✅ Parking violations retrieved: {Count} violations found", violationsResponse.Result.Count);
                foreach (var violation in violationsResponse.Result.Take(3)) // Show first 3 violations
                {
                    logger.LogInformation("  - {Tag} {State} - ${Amount} ({PaymentStatus})", 
                        violation.Tag, violation.State, violation.Amount, violation.PaymentStatus);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get parking violations: {Message}", violationsResponse.Message);
            }

            // Step 6: Get EZ Pass charges
            logger.LogInformation("\nStep 6: Getting EZ Pass charges...");
            var ezpassResponse = await apiClient.GetEzpassChargesAsync(dateFrom, dateTo, token);
            if (ezpassResponse.Reason == 0 && ezpassResponse.Result != null)
            {
                logger.LogInformation("✅ EZ Pass charges retrieved: {Count} charges found", ezpassResponse.Result.Count);
                foreach (var charge in ezpassResponse.Result.Take(3)) // Show first 3 charges
                {
                    logger.LogInformation("  - {PlateNumber} {State} - ${Amount} ({Date})", 
                        charge.PlateNumber, charge.State, charge.Amount, charge.TollDate);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get EZ Pass charges: {Message}", ezpassResponse.Message);
            }

            // Step 7: Get failed payments
            logger.LogInformation("\nStep 7: Getting failed payments...");
            var failedPaymentsResponse = await apiClient.GetFailedPaymentsAsync(token);
            if (failedPaymentsResponse.Reason == 0 && failedPaymentsResponse.Result != null)
            {
                logger.LogInformation("✅ Failed payments retrieved: {Count} payments found", failedPaymentsResponse.Result.Count);
                foreach (var payment in failedPaymentsResponse.Result.Take(3)) // Show first 3 payments
                {
                    logger.LogInformation("  - {PlateNumber} - ${Amount} ({Date})", 
                        payment.CarPlateNumber, payment.Total, payment.InvoiceDateDeployed);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get failed payments: {Message}", failedPaymentsResponse.Message);
            }

            // Step 8: Get complete payments
            logger.LogInformation("\nStep 8: Getting complete payments...");
            var completePaymentsResponse = await apiClient.GetCompletePaymentsAsync(token);
            if (completePaymentsResponse.Reason == 0 && completePaymentsResponse.Result != null)
            {
                logger.LogInformation("✅ Complete payments retrieved: {Count} payments found", completePaymentsResponse.Result.Count);
                foreach (var payment in completePaymentsResponse.Result.Take(3)) // Show first 3 payments
                {
                    logger.LogInformation("  - {PlateNumber} - ${Amount} ({Date})", 
                        payment.CarPlateNumber, payment.Total, payment.InvoiceDateDeployed);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get complete payments: {Message}", completePaymentsResponse.Message);
            }

            // Step 9: Get external toll daily invoices (requires admin role)
            logger.LogInformation("\nStep 9: Getting external toll daily invoices...");
            var invoicesResponse = await apiClient.GetAllExternalTollDailyInvoicesAsync(token);
            if (invoicesResponse.Reason == 0 && invoicesResponse.Result != null)
            {
                logger.LogInformation("✅ External toll daily invoices retrieved: {Count} invoices found", invoicesResponse.Result.Count);
                foreach (var invoice in invoicesResponse.Result.Take(3)) // Show first 3 invoices
                {
                    logger.LogInformation("  - {PlateNumber} {PlateState} - ${Amount} ({Date})", 
                        invoice.PlateNumber, invoice.PlateState, invoice.Amount, invoice.DateCreated);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get external toll daily invoices: {Message}", invoicesResponse.Message);
                logger.LogInformation("This might be expected if you don't have admin permissions.");
            }

            // Step 9c: Get all external toll daily invoices with filters
            logger.LogInformation("\nStep 9c: Getting all external toll daily invoices with filters...");
            var tollDateFromFilter = DateTime.Now.AddDays(-30);
            var tollDateToFilter = DateTime.Now;
            var allTollInvoicesResponse = await apiClient.GetExternalTollDailyInvoiceAllTollAsync(token, tollDateFromFilter, tollDateToFilter);
            if (allTollInvoicesResponse.Reason == 0 && allTollInvoicesResponse.Result != null)
            {
                logger.LogInformation("✅ All external toll daily invoices retrieved: {Count} invoices found", allTollInvoicesResponse.Result.Count);
                foreach (var invoice in allTollInvoicesResponse.Result.Take(3)) // Show first 3 invoices
                {
                    logger.LogInformation("  - {PlateNumber} {PlateState} - ${Amount} (Toll ID: {TollId}, Agency: {Agency})", 
                        invoice.PlateNumber, invoice.PlateState, invoice.Amount, invoice.TollId, invoice.Agency);
                    if (!string.IsNullOrEmpty(invoice.EntryPlaza))
                    {
                        logger.LogInformation("    Entry: {EntryPlaza} Lane {EntryLane}, Exit: {ExitPlaza} Lane {ExitLane}", 
                            invoice.EntryPlaza, invoice.EntryLane, invoice.ExitPlaza, invoice.ExitLane);
                    }
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get all external toll daily invoices: {Message}", allTollInvoicesResponse.Message);
            }

            // Step 9d: Get external toll daily invoices by company (requires admin role)
            logger.LogInformation("\nStep 9d: Getting external toll daily invoices by company...");
            var demoCompanyId = "f3be5e13-c6d2-4ce0-a564-b71459c38158"; // Use a demo company ID
            var companyDateFrom = DateTime.Now.AddDays(-30);
            var companyDateTo = DateTime.Now;
            var companyInvoicesResponse = await apiClient.GetExternalTollDailyInvoiceByCompanyAsync(token, demoCompanyId, companyDateFrom, companyDateTo);
            if (companyInvoicesResponse.Reason == 0 && companyInvoicesResponse.Result != null)
            {
                logger.LogInformation("✅ External toll daily invoices by company retrieved: Company ID {CompanyId}", companyInvoicesResponse.Result.CompanyId);
                logger.LogInformation("  - Invoice ID: {Id}, Date: {InvoiceDate}, Total Amount: ${TotalAmount}, Payment Status: {PaymentStatus}", 
                    companyInvoicesResponse.Result.Id, companyInvoicesResponse.Result.InvoiceDate, 
                    companyInvoicesResponse.Result.TotalAmount, companyInvoicesResponse.Result.PaymentStatus);
                logger.LogInformation("  - Payments: {Count} payments found", companyInvoicesResponse.Result.Payments.Count);
                foreach (var payment in companyInvoicesResponse.Result.Payments.Take(3)) // Show first 3 payments
                {
                    logger.LogInformation("    - {Name} (Table: {Table}) - ${Amount} (Status: {OriginalPaymentStatus})", 
                        payment.Name, payment.Table, payment.Amount, payment.OriginalPaymentStatus);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get external toll daily invoices by company: {Message}", companyInvoicesResponse.Message);
                logger.LogInformation("This might be expected if you don't have admin permissions or the company ID doesn't exist.");
            }

            // Step 9a: Get external toll daily invoices by toll ID (requires admin role)
            logger.LogInformation("\nStep 9a: Getting external toll daily invoices by toll ID...");
            var tollIdRequest = new GetInvoiceByTollIdRequest 
            { 
                SourceTable = "ExternalTollDailyInvoice",
                SourceId = Guid.NewGuid(),
                TollId = 12345,
                PlateNumber = "DEMO123",
                PlateState = "NY",
                PlateTag = "DEMO",
                Agency = "NYC DOT",
                Amount = 5.50m,
                AmountWithFee = 6.00m,
                NewAmount = 5.50m,
                PaymentStatus = 1,
                PostingDate = DateTime.Now,
                TransactionDate = DateTime.Now,
                TransactionDateTime = DateTime.Now,
                ExitLane = "Lane 1",
                PlazaDescription = "Demo Plaza",
                Axle = "2",
                VehicleTypeCode = "2",
                VehicleClass = "Passenger",
                Description = "Demo toll transaction",
                Prepaid = "no",
                PlanRate = "Standard",
                FareType = "Cash",
                BalanceText = "Balance: $0.00",
                DebitText = "Debit: $5.50",
                CreditText = "Credit: $0.00",
                Note = "Demo transaction",
                Exception = "",
                IsException = false,
                Completed = true,
                DateCompleted = DateTime.Now,
                TollPlanType = 7137,
                TollPlanDescription = "Standard Plan",
                IsActive = true
            };
            var invoicesByTollIdResponse = await apiClient.GetExternalTollDailyInvoiceByTollIdAsync(token, tollIdRequest);
            if (invoicesByTollIdResponse.Reason == 0 && invoicesByTollIdResponse.Result != null)
            {
                logger.LogInformation("✅ External toll daily invoices by toll ID retrieved: {Count} invoices found", invoicesByTollIdResponse.Result.Count);
                foreach (var invoice in invoicesByTollIdResponse.Result.Take(3)) // Show first 3 invoices
                {
                    logger.LogInformation("  - {PlateNumber} {PlateState} - ${Amount} (Toll ID: {TollId})", 
                        invoice.PlateNumber, invoice.PlateState, invoice.Amount, invoice.TollId);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get external toll daily invoices by toll ID: {Message}", invoicesByTollIdResponse.Message);
                logger.LogInformation("This might be expected if you don't have admin permissions or the toll ID doesn't exist.");
            }

            // Step 9b: Get external toll daily invoices by plate (requires admin role)
            logger.LogInformation("\nStep 9b: Getting external toll daily invoices by plate...");
            var plateRequest = new GetInvoiceByPlateRequest 
            { 
                SourceTable = "ExternalTollDailyInvoice",
                SourceId = Guid.NewGuid(),
                TollId = 12346,
                PlateNumber = "DEMO123", 
                PlateState = "NY",
                PlateTag = "DEMO",
                Agency = "NYC DOT",
                Amount = 5.50m,
                AmountWithFee = 6.00m,
                NewAmount = 5.50m,
                PaymentStatus = 1,
                PostingDate = DateTime.Now,
                TransactionDate = DateTime.Now,
                TransactionDateTime = DateTime.Now,
                ExitLane = "Lane 1",
                PlazaDescription = "Demo Plaza",
                Axle = "2",
                VehicleTypeCode = "2",
                VehicleClass = "Passenger",
                Description = "Demo toll transaction",
                Prepaid = "no",
                PlanRate = "Standard",
                FareType = "Cash",
                BalanceText = "Balance: $0.00",
                DebitText = "Debit: $5.50",
                CreditText = "Credit: $0.00",
                Note = "Demo transaction",
                Exception = "",
                IsException = false,
                Completed = true,
                DateCompleted = DateTime.Now,
                TollPlanType = 7137,
                TollPlanDescription = "Standard Plan",
                IsActive = true
            };
            var invoicesByPlateResponse = await apiClient.GetExternalTollDailyInvoiceByPlateAsync(token, plateRequest);
            if (invoicesByPlateResponse.Reason == 0 && invoicesByPlateResponse.Result != null)
            {
                logger.LogInformation("✅ External toll daily invoices by plate retrieved: {Count} invoices found", invoicesByPlateResponse.Result.Count);
                foreach (var invoice in invoicesByPlateResponse.Result.Take(3)) // Show first 3 invoices
                {
                    logger.LogInformation("  - {PlateNumber} {PlateState} - ${Amount} ({Date})", 
                        invoice.PlateNumber, invoice.PlateState, invoice.Amount, invoice.DateCreated);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get external toll daily invoices by plate: {Message}", invoicesByPlateResponse.Message);
                logger.LogInformation("This might be expected if you don't have admin permissions or the plate doesn't exist.");
            }

            // Step 10: Update payment status for external toll daily invoices (requires admin role)
            logger.LogInformation("\nStep 10: Updating payment status for external toll daily invoices...");
            var updatePaymentStatusRequest = new UpdatePaymentStatusRequest
            {
                Ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }, // Demo invoice IDs
                PaymentStatus = 1, // Demo payment status
                CompanyId = "demo-company-id"
            };
            var updatePaymentStatusResponse = await apiClient.UpdatePaymentStatusAsync(token, updatePaymentStatusRequest);
            if (updatePaymentStatusResponse.Reason == 0 && updatePaymentStatusResponse.Result)
            {
                logger.LogInformation("✅ Payment status updated successfully for {Count} invoices", updatePaymentStatusRequest.Ids.Count);
            }
            else
            {
                logger.LogWarning("❌ Failed to update payment status: {Message}", updatePaymentStatusResponse.Message);
                logger.LogInformation("This might be expected if you don't have admin permissions or the invoice IDs don't exist.");
            }

            // Step 11: Get companies (requires admin role)
            logger.LogInformation("\nStep 11: Getting companies...");
            var companiesAllResponse = await apiClient.GetCompaniesAllAsync(token);
            if (companiesAllResponse.Reason == 0 && companiesAllResponse.Result != null)
            {
                logger.LogInformation("✅ All companies retrieved: {Count} companies found", companiesAllResponse.Result.Count);
                foreach (var company in companiesAllResponse.Result.Take(3)) // Show first 3 companies
                {
                    logger.LogInformation("  - {Name} (ID: {Id}) - Active: {IsActive}, State: {StateId}", 
                        company.Name, company.Id, company.IsActive, company.StateId);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get all companies: {Message}", companiesAllResponse.Message);
                logger.LogInformation("This might be expected if you don't have admin permissions.");
            }

            // Step 12: Get active companies (requires admin role)
            logger.LogInformation("\nStep 12: Getting active companies...");
            var companiesActiveResponse = await apiClient.GetCompaniesActiveAsync(token);
            if (companiesActiveResponse.Reason == 0 && companiesActiveResponse.Result != null)
            {
                logger.LogInformation("✅ Active companies retrieved: {Count} companies found", companiesActiveResponse.Result.Count);
                foreach (var company in companiesActiveResponse.Result.Take(3)) // Show first 3 companies
                {
                    logger.LogInformation("  - {Name} (ID: {Id}) - Active: {IsActive}, State: {StateId}", 
                        company.Name, company.Id, company.IsActive, company.StateId);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get active companies: {Message}", companiesActiveResponse.Message);
                logger.LogInformation("This might be expected if you don't have admin permissions.");
            }

            // Step 12a: Get external toll daily invoices by company (requires admin role)
            logger.LogInformation("\nStep 12a: Getting external toll daily invoices by company...");
            var companyId = "f3be5e13-c6d2-4ce0-a564-b71459c38158"; // Use a demo company ID
            var dateFromFilter = DateTime.Now.AddDays(-30);
            var dateToFilter = DateTime.Now;
            var invoicesByCompanyResponse = await apiClient.GetExternalTollDailyInvoiceByCompanyAsync(token, companyId, dateFromFilter, dateToFilter);
            if (invoicesByCompanyResponse.Reason == 0 && invoicesByCompanyResponse.Result != null)
            {
                logger.LogInformation("✅ External toll daily invoices by company retrieved: Company ID {CompanyId}", invoicesByCompanyResponse.Result.CompanyId);
                logger.LogInformation("  - Invoice ID: {Id}, Date: {InvoiceDate}, Total Amount: ${TotalAmount}, Payment Status: {PaymentStatus}", 
                    invoicesByCompanyResponse.Result.Id, invoicesByCompanyResponse.Result.InvoiceDate, 
                    invoicesByCompanyResponse.Result.TotalAmount, invoicesByCompanyResponse.Result.PaymentStatus);
                logger.LogInformation("  - Payments: {Count} payments found", invoicesByCompanyResponse.Result.Payments.Count);
                foreach (var payment in invoicesByCompanyResponse.Result.Payments.Take(3)) // Show first 3 payments
                {
                    logger.LogInformation("    - {Name} (Table: {Table}) - ${Amount} (Status: {OriginalPaymentStatus})", 
                        payment.Name, payment.Table, payment.Amount, payment.OriginalPaymentStatus);
                }
            }
            else
            {
                logger.LogWarning("❌ Failed to get external toll daily invoices by company: {Message}", invoicesByCompanyResponse.Message);
                logger.LogInformation("This might be expected if you don't have admin permissions or the company ID doesn't exist.");
            }

            // Step 12: Create a car (example)
            logger.LogInformation("\nStep 12: Creating a car...");
            var createCarRequest = new CreateCarRequest
            {
                Role = 0,
                Id = Guid.NewGuid().ToString(),
                Title = "Demo Car",
                Description = "Demo car created by API example",
                Published = true,
                PricePerWeek = 150.00m,
                MinimumReservationDays = 1,
                CarBrand = "Toyota",
                Color = "Blue",
                InteriorColor = "Black",
                CarModel = "Camry",
                Year = 2023,
                CarTypeId = "sedan",
                LiabilityInsuranceId = "liability-1",
                NewLiabilityInsurance = "State Farm",
                FullCoverageId = "full-1",
                NewFullCoverage = "State Farm Full",
                CityId = "city-1",
                City = "New York",
                StateId = "NY",
                AddressLine1 = "123 Main St",
                AddressLine2 = "",
                ZipCode = "10001",
                Latitude = 40.7128,
                Longitude = -74.0060,
                PhoneNumber = "(555) 123-4567",
                CarPlateNumber = "DEMO123",
                CarPlateNumberState = "NY",
                TermsConditions = true,
                PriceFullTimePerWeek = 150.00m,
                PriceDayPerShiftWeek = 50.00m,
                PriceNightPerShiftWeek = 30.00m,
                PriceSecurityDeposit = 200.00m,
                AllowNegotiatingPrice = true,
                RentalType = 0,
                Tlc = false,
                IsMonthlyCar = false,
                IsPedicabCar = false,
                SeaterId = "seater-1",
                EngineId = "engine-1",
                Vin = "1HGBH41JXMN109186",
                KillSwitchId = "kill-1",
                VerraTollTagAttached = false,
                IsExternal = false,
                RentersTag = false,
                RequestShipTag = false
            };

            var createCarResponse = await apiClient.CreateCarAsync(token, createCarRequest);
            if (createCarResponse.Reason == 0 && createCarResponse.Result != null)
            {
                logger.LogInformation("✅ Car created successfully!");
                logger.LogInformation("  - ID: {Id}, Title: {Title}, Plate: {PlateNumber} {PlateState}", 
                    createCarResponse.Result.Id, createCarResponse.Result.Title, 
                    createCarResponse.Result.CarPlateNumber, createCarResponse.Result.CarPlateNumberState);
            }
            else
            {
                logger.LogWarning("❌ Failed to create car: {Message}", createCarResponse.Message);
            }
            */
            logger.LogInformation("\n=== All API calls completed successfully! ===");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error occurred during API calls demo");
        }
    }
}