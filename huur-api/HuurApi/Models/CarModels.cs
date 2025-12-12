/*
 *
 * Copyright (c) 2024 Alexander Orlov.
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

using Newtonsoft.Json;

namespace HuurApi.Models;

/// <summary>
/// Request model for Cars/CarListByLicensePlate - simple GET with no parameters
/// </summary>
public class CarListByLicensePlateRequest
{
	// No parameters needed for this endpoint
}

/// <summary>
/// Request model for getting all cars via POST /Cars/all
/// </summary>
public class GetCarsAllRequest
{
    /// <summary>
    /// Requesting user's role
    /// </summary>
	[JsonProperty("role")]
	public int Role { get; set; }
}

/// <summary>
/// Simplified car model for Cars/all endpoint response
/// </summary>
public class CarAll
{
	/// <summary>
	/// Car identifier
	/// </summary>
	[JsonProperty("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// Car title/name
	/// </summary>
	[JsonProperty("title")]
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// Car description
	/// </summary>
	[JsonProperty("description")]
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Primary image URL
	/// </summary>
	[JsonProperty("imageURL")]
	public string ImageURL { get; set; } = string.Empty;

	/// <summary>
	/// Brand/manufacturer
	/// </summary>
	[JsonProperty("brand")]
	public string Brand { get; set; } = string.Empty;

	/// <summary>
	/// Owner identifier
	/// </summary>
	[JsonProperty("ownerId")]
	public string OwnerId { get; set; } = string.Empty;

	/// <summary>
	/// Type color
	/// </summary>
	[JsonProperty("typeColor")]
	public string TypeColor { get; set; } = string.Empty;

	/// <summary>
	/// Type name
	/// </summary>
	[JsonProperty("typeName")]
	public string TypeName { get; set; } = string.Empty;

	/// <summary>
	/// Car type identifier
	/// </summary>
	[JsonProperty("typeId")]
	public string TypeId { get; set; } = string.Empty;

	/// <summary>
	/// Publication flag
	/// </summary>
	[JsonProperty("published")]
	public bool Published { get; set; }

	/// <summary>
	/// Minimum reservation days
	/// </summary>
	[JsonProperty("minimumReservationDays")]
	public int MinimumReservationDays { get; set; }

	/// <summary>
	/// Weekly price
	/// </summary>
	[JsonProperty("pricePerWeek")]
	public decimal PricePerWeek { get; set; }

	/// <summary>
	/// Latitude coordinate
	/// </summary>
	[JsonProperty("latitude")]
	public double Latitude { get; set; }

	/// <summary>
	/// Longitude coordinate
	/// </summary>
	[JsonProperty("longitude")]
	public double Longitude { get; set; }

	/// <summary>
	/// Full-time weekly price
	/// </summary>
	[JsonProperty("priceFullTimePerWeek")]
	public decimal PriceFullTimePerWeek { get; set; }

	/// <summary>
	/// Day shift weekly price
	/// </summary>
	[JsonProperty("priceDayPerShiftWeek")]
	public decimal PriceDayPerShiftWeek { get; set; }

	/// <summary>
	/// Night shift weekly price
	/// </summary>
	[JsonProperty("priceNightPerShiftWeek")]
	public decimal PriceNightPerShiftWeek { get; set; }

	/// <summary>
	/// Security deposit amount
	/// </summary>
	[JsonProperty("priceSecurityDeposit")]
	public decimal PriceSecurityDeposit { get; set; }

	/// <summary>
	/// Indicates whether price negotiating is allowed
	/// </summary>
	[JsonProperty("allowNegotiatingPrice")]
	public bool AllowNegotiatingPrice { get; set; }

	/// <summary>
	/// Rental type
	/// </summary>
	[JsonProperty("rentalType")]
	public int RentalType { get; set; }

	/// <summary>
	/// Aggregated rating
	/// </summary>
	[JsonProperty("rating")]
	public int Rating { get; set; }

	/// <summary>
	/// City name
	/// </summary>
	[JsonProperty("city")]
	public string City { get; set; } = string.Empty;

	/// <summary>
	/// State code
	/// </summary>
	[JsonProperty("state")]
	public string State { get; set; } = string.Empty;

	/// <summary>
	/// Model name
	/// </summary>
	[JsonProperty("model")]
	public string Model { get; set; } = string.Empty;

	/// <summary>
	/// Model year
	/// </summary>
	[JsonProperty("year")]
	public int Year { get; set; }

	/// <summary>
	/// Last update timestamp
	/// </summary>
	[JsonProperty("updated")]
	public DateTime Updated { get; set; }

	/// <summary>
	/// Image URLs
	/// </summary>
	[JsonProperty("images")]
	public List<string> Images { get; set; } = new List<string>();

	/// <summary>
	/// Monthly car flag
	/// </summary>
	[JsonProperty("isMonthlyCar")]
	public bool IsMonthlyCar { get; set; }

	/// <summary>
	/// Pedicab car flag
	/// </summary>
	[JsonProperty("isPedicabCar")]
	public bool IsPedicabCar { get; set; }

	/// <summary>
	/// Seater type
	/// </summary>
	[JsonProperty("seaterType")]
	public string SeaterType { get; set; } = string.Empty;

	/// <summary>
	/// Engine type
	/// </summary>
	[JsonProperty("engineType")]
	public string EngineType { get; set; } = string.Empty;

	/// <summary>
	/// Liability insurance description
	/// </summary>
	[JsonProperty("liabilityInsurance")]
	public string LiabilityInsurance { get; set; } = string.Empty;

	/// <summary>
	/// Full coverage description
	/// </summary>
	[JsonProperty("fullCoverage")]
	public string FullCoverage { get; set; } = string.Empty;
}

/// <summary>
/// Response model for getting all cars via POST /Cars/all
/// </summary>
public class GetCarsAllResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
	[JsonProperty("reason")]
	public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
	[JsonProperty("message")]
	public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
	[JsonProperty("stackTrace")]
	public string? StackTrace { get; set; }

    /// <summary>
    /// List of cars
    /// </summary>
	[JsonProperty("result")]
	public List<CarAll> Result { get; set; } = new List<CarAll>();
}

/// <summary>
/// Request model for creating a new car via POST /Cars
/// </summary>
public class CreateCarRequest
{
	/// <summary>Requesting user's role</summary>
	[JsonProperty("role")]
	public int Role { get; set; }

	/// <summary>Car identifier</summary>
	[JsonProperty("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>Car title/name</summary>
	[JsonProperty("title")]
	public string Title { get; set; } = string.Empty;

	/// <summary>Car description</summary>
	[JsonProperty("description")]
	public string Description { get; set; } = string.Empty;

	/// <summary>Publication flag</summary>
	[JsonProperty("published")]
	public bool Published { get; set; }

	/// <summary>Weekly price</summary>
	[JsonProperty("pricePerWeek")]
	public decimal PricePerWeek { get; set; }

	/// <summary>Minimum reservation days</summary>
	[JsonProperty("minimumReservationDays")]
	public int MinimumReservationDays { get; set; }

	/// <summary>Brand/manufacturer</summary>
	[JsonProperty("carBrand")]
	public string CarBrand { get; set; } = string.Empty;

	/// <summary>Color</summary>
	[JsonProperty("color")]
	public string Color { get; set; } = string.Empty;

	/// <summary>Interior color</summary>
	[JsonProperty("interiorColor")]
	public string InteriorColor { get; set; } = string.Empty;

	/// <summary>Model</summary>
	[JsonProperty("carModel")]
	public string CarModel { get; set; } = string.Empty;

	/// <summary>Model year</summary>
	[JsonProperty("year")]
	public int Year { get; set; }

	/// <summary>Car type identifier</summary>
	[JsonProperty("carTypeId")]
	public string CarTypeId { get; set; } = string.Empty;

	/// <summary>Liability insurance identifier</summary>
	[JsonProperty("liabilityInsuranceId")]
	public string LiabilityInsuranceId { get; set; } = string.Empty;

	/// <summary>New liability insurance description</summary>
	[JsonProperty("newLiabilityInsurance")]
	public string NewLiabilityInsurance { get; set; } = string.Empty;

	/// <summary>Full coverage identifier</summary>
	[JsonProperty("fullCoverageId")]
	public string FullCoverageId { get; set; } = string.Empty;

	/// <summary>New full coverage description</summary>
	[JsonProperty("newFullCoverage")]
	public string NewFullCoverage { get; set; } = string.Empty;

	/// <summary>City identifier</summary>
	[JsonProperty("cityId")]
	public string CityId { get; set; } = string.Empty;

	/// <summary>City name</summary>
	[JsonProperty("city")]
	public string City { get; set; } = string.Empty;

	/// <summary>State identifier</summary>
	[JsonProperty("stateId")]
	public string StateId { get; set; } = string.Empty;

	/// <summary>Address line 1</summary>
	[JsonProperty("addressLine1")]
	public string AddressLine1 { get; set; } = string.Empty;

	/// <summary>Address line 2</summary>
	[JsonProperty("addressLine2")]
	public string AddressLine2 { get; set; } = string.Empty;

	/// <summary>ZIP code</summary>
	[JsonProperty("zipCode")]
	public string ZipCode { get; set; } = string.Empty;

	/// <summary>Latitude</summary>
	[JsonProperty("latitude")]
	public double Latitude { get; set; }

	/// <summary>Longitude</summary>
	[JsonProperty("longitude")]
	public double Longitude { get; set; }

	/// <summary>Contact phone number</summary>
	[JsonProperty("phoneNumber")]
	public string PhoneNumber { get; set; } = string.Empty;

	/// <summary>License plate number</summary>
	[JsonProperty("carPlateNumber")]
	public string CarPlateNumber { get; set; } = string.Empty;

	/// <summary>License plate state</summary>
	[JsonProperty("carPlateNumberState")]
	public string CarPlateNumberState { get; set; } = string.Empty;

	/// <summary>Terms and conditions accepted flag</summary>
	[JsonProperty("termsConditions")]
	public bool TermsConditions { get; set; }

	/// <summary>Full-time weekly price</summary>
	[JsonProperty("priceFullTimePerWeek")]
	public decimal PriceFullTimePerWeek { get; set; }

	/// <summary>Day shift weekly price</summary>
	[JsonProperty("priceDayPerShiftWeek")]
	public decimal PriceDayPerShiftWeek { get; set; }

	/// <summary>Night shift weekly price</summary>
	[JsonProperty("priceNightPerShiftWeek")]
	public decimal PriceNightPerShiftWeek { get; set; }

	/// <summary>Security deposit amount</summary>
	[JsonProperty("priceSecurityDeposit")]
	public decimal PriceSecurityDeposit { get; set; }

	/// <summary>Allow price negotiating</summary>
	[JsonProperty("allowNegotiatingPrice")]
	public bool AllowNegotiatingPrice { get; set; }

	/// <summary>Rental type</summary>
	[JsonProperty("rentalType")]
	public int RentalType { get; set; }

	/// <summary>TLC license flag</summary>
	[JsonProperty("tlc")]
	public bool Tlc { get; set; }

	/// <summary>Monthly car flag</summary>
	[JsonProperty("isMonthlyCar")]
	public bool IsMonthlyCar { get; set; }

	/// <summary>Pedicab car flag</summary>
	[JsonProperty("isPedicabCar")]
	public bool IsPedicabCar { get; set; }

	/// <summary>Seater identifier</summary>
	[JsonProperty("seaterId")]
	public string SeaterId { get; set; } = string.Empty;

	/// <summary>Engine identifier</summary>
	[JsonProperty("engineId")]
	public string EngineId { get; set; } = string.Empty;

	/// <summary>Vehicle identification number</summary>
	[JsonProperty("vin")]
	public string Vin { get; set; } = string.Empty;

	/// <summary>Kill switch identifier</summary>
	[JsonProperty("killSwitchId")]
	public string KillSwitchId { get; set; } = string.Empty;

	/// <summary>Verra toll tag attached</summary>
	[JsonProperty("verraTollTagAttached")]
	public bool VerraTollTagAttached { get; set; }

	/// <summary>External car flag</summary>
	[JsonProperty("isExternal")]
	public bool IsExternal { get; set; }

	/// <summary>Renter's tag flag</summary>
	[JsonProperty("rentersTag")]
	public bool RentersTag { get; set; }

	/// <summary>Request shipping tag</summary>
	[JsonProperty("requestShipTag")]
	public bool RequestShipTag { get; set; }
}

/// <summary>
/// Individual car record returned by the API
/// </summary>
public class Car
{
	/// <summary>Car identifier</summary>
	[JsonProperty("id")] public string? Id { get; set; }
	/// <summary>Owner identifier</summary>
	[JsonProperty("ownerId")] public string? OwnerId { get; set; }
	/// <summary>Title/name</summary>
	[JsonProperty("title")] public string? Title { get; set; }
	/// <summary>Description</summary>
	[JsonProperty("description")] public string? Description { get; set; }
	/// <summary>Publication flag</summary>
	[JsonProperty("published")] public bool Published { get; set; }
	/// <summary>Weekly price</summary>
	[JsonProperty("pricePerWeek")] public decimal PricePerWeek { get; set; }
	/// <summary>Minimum reservation days</summary>
	[JsonProperty("minimumReservationDays")] public int MinimumReservationDays { get; set; }
	/// <summary>Brand/manufacturer</summary>
	[JsonProperty("carBrand")] public string? CarBrand { get; set; }
	/// <summary>Car type identifier</summary>
	[JsonProperty("carTypeId")] public string? CarTypeId { get; set; }
	/// <summary>Car type</summary>
	[JsonProperty("carType")] public string? CarType { get; set; }
	/// <summary>Type color</summary>
	[JsonProperty("carTypeColor")] public string? CarTypeColor { get; set; }
	/// <summary>Model</summary>
	[JsonProperty("carModel")] public string? CarModel { get; set; }
	/// <summary>Model year</summary>
	[JsonProperty("year")] public int Year { get; set; }
	/// <summary>Color</summary>
	[JsonProperty("color")] public string? Color { get; set; }
	/// <summary>Interior color</summary>
	[JsonProperty("interiorColor")] public string? InteriorColor { get; set; }
	/// <summary>Liability insurance description</summary>
	[JsonProperty("liabilityInsurance")] public string? LiabilityInsurance { get; set; }
	/// <summary>Full coverage description</summary>
	[JsonProperty("fullCoverage")] public string? FullCoverage { get; set; }
	/// <summary>Latitude</summary>
	[JsonProperty("latitude")] public double Latitude { get; set; }
	/// <summary>Longitude</summary>
	[JsonProperty("longitude")] public double Longitude { get; set; }
	/// <summary>Contact phone number</summary>
	[JsonProperty("phoneNumber")] public string? PhoneNumber { get; set; }
	/// <summary>License plate number</summary>
	[JsonProperty("carPlateNumber")] public string? CarPlateNumber { get; set; }
	/// <summary>License plate state</summary>
	[JsonProperty("carPlateNumberState")] public string? CarPlateNumberState { get; set; }
	/// <summary>Terms and conditions accepted</summary>
	[JsonProperty("termsConditions")] public bool TermsConditions { get; set; }
	/// <summary>Creator identifier</summary>
	[JsonProperty("createdBy")] public string? CreatedBy { get; set; }
	/// <summary>Last update timestamp</summary>
	[JsonProperty("updated")] public DateTimeOffset? Updated { get; set; }
	/// <summary>City identifier</summary>
	[JsonProperty("cityId")] public string? CityId { get; set; }
	/// <summary>City name</summary>
	[JsonProperty("city")] public string? City { get; set; }
	/// <summary>State code</summary>
	[JsonProperty("state")] public string? State { get; set; }
	/// <summary>Address line 1</summary>
	[JsonProperty("addressLine1")] public string? AddressLine1 { get; set; }
	/// <summary>Address line 2</summary>
	[JsonProperty("addressLine2")] public string? AddressLine2 { get; set; }
	/// <summary>ZIP code</summary>
	[JsonProperty("zipCode")] public string? ZipCode { get; set; }
	/// <summary>Primary image URL</summary>
	[JsonProperty("imageUrl")] public string? ImageUrl { get; set; }
	/// <summary>Image URLs</summary>
	[JsonProperty("images")] public List<string> Images { get; set; } = new();
	/// <summary>Location name</summary>
	[JsonProperty("location")] public string? Location { get; set; }
	/// <summary>Full-time weekly price</summary>
	[JsonProperty("priceFullTimePerWeek")] public decimal PriceFullTimePerWeek { get; set; }
	/// <summary>Day shift weekly price</summary>
	[JsonProperty("priceDayPerShiftWeek")] public decimal PriceDayPerShiftWeek { get; set; }
	/// <summary>Night shift weekly price</summary>
	[JsonProperty("priceNightPerShiftWeek")] public decimal PriceNightPerShiftWeek { get; set; }
	/// <summary>Security deposit amount</summary>
	[JsonProperty("priceSecurityDeposit")] public decimal PriceSecurityDeposit { get; set; }
	/// <summary>Allow price negotiating</summary>
	[JsonProperty("allowNegotiatingPrice")] public bool AllowNegotiatingPrice { get; set; }
	/// <summary>TLC license flag</summary>
	[JsonProperty("tlc")] public bool Tlc { get; set; }
	/// <summary>Rental type</summary>
	[JsonProperty("rentalType")] public int RentalType { get; set; }
	/// <summary>Aggregated rating</summary>
	[JsonProperty("rating")] public int Rating { get; set; }
	/// <summary>Usage frequency</summary>
	[JsonProperty("frequency")] public int Frequency { get; set; }
	/// <summary>Monthly car flag</summary>
	[JsonProperty("isMonthlyCar")] public bool IsMonthlyCar { get; set; }
	/// <summary>Pedicab car flag</summary>
	[JsonProperty("isPedicabCar")] public bool IsPedicabCar { get; set; }
	/// <summary>Seater type</summary>
	[JsonProperty("seaterType")] public string? SeaterType { get; set; }
	/// <summary>Engine type</summary>
	[JsonProperty("engineType")] public string? EngineType { get; set; }
	/// <summary>Vehicle identification number</summary>
	[JsonProperty("vin")] public string? Vin { get; set; }
	/// <summary>Kill switch identifier</summary>
	[JsonProperty("killSwitchId")] public string? KillSwitchId { get; set; }
	/// <summary>Verra toll tag attached</summary>
	[JsonProperty("verraTollTagAttached")] public bool VerraTollTagAttached { get; set; }
	/// <summary>External car flag</summary>
	[JsonProperty("isExternal")] public bool IsExternal { get; set; }
	/// <summary>Renter's tag flag</summary>
	[JsonProperty("rentersTag")] public bool RentersTag { get; set; }
	/// <summary>Request shipping tag</summary>
	[JsonProperty("requestShipTag")] public bool RequestShipTag { get; set; }
	/// <summary>Deleted flag</summary>
	[JsonProperty("deleted")] public bool Deleted { get; set; }
}

/// <summary>
/// Response model for Cars/CarListByLicensePlate
/// </summary>
public class CarListByLicensePlateResponse
{
	/// <summary>
	/// Strongly-typed list of cars
	/// </summary>
	[JsonProperty("result")]
	public List<Car> Result { get; set; } = new List<Car>();

	/// <summary>Response reason code</summary>
	[JsonProperty("reason")]
	public int Reason { get; set; }

	/// <summary>Response message</summary>
	[JsonProperty("message")]
	public string? Message { get; set; }

	/// <summary>Stack trace for errors</summary>
	[JsonProperty("stackTrace")]
	public string? StackTrace { get; set; }
}

/// <summary>
/// Response model for creating a new car via POST /Cars
/// </summary>
public class CreateCarResponse
{
	/// <summary>Response reason code</summary>
	[JsonProperty("reason")]
	public int Reason { get; set; }

	/// <summary>Response message</summary>
	[JsonProperty("message")]
	public string? Message { get; set; }

	/// <summary>Stack trace for errors</summary>
	[JsonProperty("stackTrace")]
	public string? StackTrace { get; set; }

	/// <summary>Created car</summary>
	[JsonProperty("result")]
	public Car? Result { get; set; }
}

/// <summary>
/// License plate information
/// </summary>
public class LicensePlate
{
	/// <summary>License plate number</summary>
	[JsonProperty("plateNumber")]
	public string PlateNumber { get; set; } = string.Empty;

	/// <summary>License plate state</summary>
	[JsonProperty("plateState")]
	public string PlateState { get; set; } = string.Empty;
}

/// <summary>
/// Response model for GET /Cars/GetLicensePlates
/// </summary>
public class GetLicensePlatesResponse
{
	/// <summary>List of license plates</summary>
	[JsonProperty("result")]
	public List<LicensePlate> Result { get; set; } = new List<LicensePlate>();

	/// <summary>Response reason code</summary>
	[JsonProperty("reason")]
	public int Reason { get; set; }

	/// <summary>Response message</summary>
	[JsonProperty("message")]
	public string? Message { get; set; }

	/// <summary>Stack trace for errors</summary>
	[JsonProperty("stackTrace")]
	public string? StackTrace { get; set; }
}

/// <summary>
/// External vehicle information
/// </summary>
public class ExternalVehicle
{
	/// <summary>Vehicle identifier</summary>
	[JsonProperty("id")]
	public int Id { get; set; }

	/// <summary>Vehicle label</summary>
	[JsonProperty("label")]
	public string? Label { get; set; }

	/// <summary>Vehicle Identification Number</summary>
	[JsonProperty("vin")]
	public string? Vin { get; set; }

	/// <summary>Vehicle tag</summary>
	[JsonProperty("tag")]
	public string? Tag { get; set; }

	/// <summary>State code</summary>
	[JsonProperty("state")]
	public string? State { get; set; }

	/// <summary>Owner identifier</summary>
	[JsonProperty("ownerId")]
	public string? OwnerId { get; set; }

	/// <summary>License plate number</summary>
	[JsonProperty("licensePlate")]
	public string? LicensePlate { get; set; }

	/// <summary>Provider vehicle identifier</summary>
	[JsonProperty("providerVehicleId")]
	public string? ProviderVehicleId { get; set; }

	/// <summary>Provider identifier</summary>
	[JsonProperty("provider")]
	public int Provider { get; set; }

	/// <summary>Country</summary>
	[JsonProperty("country")]
	public string? Country { get; set; }

	/// <summary>Model year</summary>
	[JsonProperty("year")]
	public int? Year { get; set; }

	/// <summary>Vehicle make</summary>
	[JsonProperty("make")]
	public string? Make { get; set; }

	/// <summary>Vehicle model</summary>
	[JsonProperty("model")]
	public string? Model { get; set; }

	/// <summary>Vehicle color</summary>
	[JsonProperty("color")]
	public string? Color { get; set; }

	/// <summary>Active status flag</summary>
	[JsonProperty("isActive")]
	public bool IsActive { get; set; }

	/// <summary>Creation timestamp</summary>
	[JsonProperty("createdAt")]
	public DateTime? CreatedAt { get; set; }

	/// <summary>Last update timestamp</summary>
	[JsonProperty("updatedAt")]
	public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Response model for GET /ExternalVehicles
/// </summary>
public class GetExternalVehiclesResponse
{
	/// <summary>List of external vehicles</summary>
	[JsonProperty("result")]
	public List<ExternalVehicle> Result { get; set; } = new List<ExternalVehicle>();

	/// <summary>Response reason code</summary>
	[JsonProperty("reason")]
	public int Reason { get; set; }

	/// <summary>Response message</summary>
	[JsonProperty("message")]
	public string? Message { get; set; }

	/// <summary>Stack trace for errors</summary>
	[JsonProperty("stackTrace")]
	public string? StackTrace { get; set; }
}
