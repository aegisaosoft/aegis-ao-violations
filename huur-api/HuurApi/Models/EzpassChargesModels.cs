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
/// Response model for EZ-Pass charges
/// </summary>
public class EzpassChargesResponse
{
    /// <summary>
    /// Collection of EZ-Pass charge items
    /// </summary>
    [JsonProperty("result")]
    public List<EzpassCharge> Result { get; set; } = new();

    /// <summary>
    /// Reason code for the response
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace (if any error occurred)
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }
}

/// <summary>
/// Single EZ-Pass charge item
/// </summary>
public class EzpassCharge
{
    /// <summary>
    /// Unique identifier for the EZ-Pass charge
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// External fleet code identifier
    /// </summary>
    [JsonProperty("externalFleetCode")]
    public string? ExternalFleetCode { get; set; }

    /// <summary>
    /// Name of the fleet
    /// </summary>
    [JsonProperty("fleetName")]
    public string? FleetName { get; set; }

    /// <summary>
    /// Vehicle identifier
    /// </summary>
    [JsonProperty("vehicleID")]
    public string? VehicleId { get; set; }

    /// <summary>
    /// Vehicle Identification Number
    /// </summary>
    [JsonProperty("vin")]
    public string? Vin { get; set; }

    /// <summary>
    /// Vehicle license plate number
    /// </summary>
    [JsonProperty("plateNumber")]
    public string? PlateNumber { get; set; }

    /// <summary>
    /// Driver's first name
    /// </summary>
    [JsonProperty("driverFirstName")]
    public string? DriverFirstName { get; set; }

    /// <summary>
    /// Driver's last name
    /// </summary>
    [JsonProperty("driverLastName")]
    public string? DriverLastName { get; set; }

    /// <summary>
    /// Primary address line
    /// </summary>
    [JsonProperty("address1")]
    public string? Address1 { get; set; }

    /// <summary>
    /// Secondary address line
    /// </summary>
    [JsonProperty("address2")]
    public string? Address2 { get; set; }

    /// <summary>
    /// City name
    /// </summary>
    [JsonProperty("city")]
    public string? City { get; set; }

    /// <summary>
    /// State or province
    /// </summary>
    [JsonProperty("state")]
    public string? State { get; set; }

    /// <summary>
    /// ZIP or postal code
    /// </summary>
    [JsonProperty("zip")]
    public string? Zip { get; set; }

    /// <summary>
    /// Driver's email address
    /// </summary>
    [JsonProperty("driverEmailAddress")]
    public string? DriverEmailAddress { get; set; }

    /// <summary>
    /// Unique toll transaction identifier
    /// </summary>
    [JsonProperty("tollID")]
    public long TollId { get; set; }

    /// <summary>
    /// Date of the toll transaction
    /// </summary>
    [JsonProperty("tollDate")]
    public DateTime TollDate { get; set; }

    /// <summary>
    /// Time of the toll transaction (HH:mm:ss format)
    /// </summary>
    [JsonProperty("tollTime")]
    public string? TollTime { get; set; }

    /// <summary>
    /// Exit date for the toll transaction
    /// </summary>
    [JsonProperty("tollExitDate")]
    public DateTime? TollExitDate { get; set; }

    /// <summary>
    /// Toll authority code
    /// </summary>
    [JsonProperty("tollAuthority")]
    public string? TollAuthority { get; set; }

    /// <summary>
    /// Description of the toll authority
    /// </summary>
    [JsonProperty("tollAuthorityDescription")]
    public string? TollAuthorityDescription { get; set; }

    /// <summary>
    /// Type of toll transaction
    /// </summary>
    [JsonProperty("transactionType")]
    public string? TransactionType { get; set; }

    /// <summary>
    /// Entry point for the toll
    /// </summary>
    [JsonProperty("entry")]
    public string? Entry { get; set; }

    /// <summary>
    /// Exit point for the toll
    /// </summary>
    [JsonProperty("exit")]
    public string? Exit { get; set; }

    /// <summary>
    /// Toll amount charged
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency for the amount
    /// </summary>
    [JsonProperty("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Date when the invoice was deployed
    /// </summary>
    [JsonProperty("dateInvoiceDeployed")]
    public DateTime DateInvoiceDeployed { get; set; }

    /// <summary>
    /// Information header for the transaction
    /// </summary>
    [JsonProperty("infoHeader")]
    public string? InfoHeader { get; set; }

    /// <summary>
    /// Daily payment identifier
    /// </summary>
    [JsonProperty("dailyPaymentId")]
    public string? DailyPaymentId { get; set; }

    /// <summary>
    /// Adjusted amount after any modifications
    /// </summary>
    [JsonProperty("adjustedAmount")]
    public decimal AdjustedAmount { get; set; }

    /// <summary>
    /// Original payer identifier
    /// </summary>
    [JsonProperty("originalPayerId")]
    public string? OriginalPayerId { get; set; }

    /// <summary>
    /// EZ-Pass transponder number
    /// </summary>
    [JsonProperty("transponderNumber")]
    public string? TransponderNumber { get; set; }
}



