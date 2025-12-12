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
/// Response model for failed payments
/// </summary>
public class FailedPaymentsResponse
{
    /// <summary>
    /// Array of failed payment records
    /// </summary>
    [JsonProperty("result")]
    public List<FailedPayment> Result { get; set; } = new();

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
/// Individual failed payment record
/// </summary>
public class FailedPayment
{
    /// <summary>
    /// Car license plate number
    /// </summary>
    [JsonProperty("carPlateNumber")]
    public string CarPlateNumber { get; set; } = string.Empty;

    /// <summary>
    /// Number of tolls in this payment
    /// </summary>
    [JsonProperty("numberOfTolls")]
    public int NumberOfTolls { get; set; }

    /// <summary>
    /// Total amount for this payment
    /// </summary>
    [JsonProperty("total")]
    public decimal Total { get; set; }

    /// <summary>
    /// Stripe adjustment amount (if any)
    /// </summary>
    [JsonProperty("stripeAdjusted")]
    public string StripeAdjusted { get; set; } = string.Empty;

    /// <summary>
    /// Daily payment ID
    /// </summary>
    [JsonProperty("dailyPaymentId")]
    public string DailyPaymentId { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Owner's first name
    /// </summary>
    [JsonProperty("ownerFirstName")]
    public string OwnerFirstName { get; set; } = string.Empty;

    /// <summary>
    /// Owner's last name
    /// </summary>
    [JsonProperty("ownerLastName")]
    public string OwnerLastName { get; set; } = string.Empty;

    /// <summary>
    /// Kill switch ID
    /// </summary>
    [JsonProperty("killSwitchId")]
    public string KillSwitchId { get; set; } = string.Empty;

    /// <summary>
    /// Whether tag is present
    /// </summary>
    [JsonProperty("tag")]
    public bool Tag { get; set; }

    /// <summary>
    /// Payment flow ID
    /// </summary>
    [JsonProperty("paymentFlowId")]
    public string PaymentFlowId { get; set; } = string.Empty;

    /// <summary>
    /// Invoice date deployed
    /// </summary>
    [JsonProperty("invoiceDateDeployed")]
    public DateTime InvoiceDateDeployed { get; set; }

    /// <summary>
    /// Vehicle Identification Number
    /// </summary>
    [JsonProperty("vin")]
    public string Vin { get; set; } = string.Empty;

    /// <summary>
    /// Booking ID
    /// </summary>
    [JsonProperty("bookingId")]
    public string BookingId { get; set; } = string.Empty;
}
