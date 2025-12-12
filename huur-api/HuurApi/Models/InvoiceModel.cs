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
/// Response model for parking violations
/// </summary>
public class InvoiceResponse
{
    /// <summary>
    /// Array of parking violation records
    /// </summary>
    [JsonProperty("result")]
    public Invoice Result { get; set; } = new();

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
/// Represents an invoice with company and payment information
/// </summary>
public class Invoice
{
    /// <summary>
    /// Invice ID
    /// </summary>
    [JsonProperty("id")]
    public long Id { get; set; }

    /// <summary>
    /// Company Name
    /// </summary>
    [JsonProperty("companyName")]
    public string CompantName { get; set; } = string.Empty;

    /// <summary>
    /// Company IG
    /// </summary>
    [JsonProperty("companyId")]
    public string CompanyId { get; set; } = string.Empty;

    /// <summary>
    /// Invoise Date
    /// </summary>
    [JsonProperty("invoiseDate")]
    public DateTime? InvoiseDate { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [JsonProperty("status")]
    public int Status { get; set; }

    /// <summary>
    /// Company IG
    /// </summary>
    [JsonProperty("payments")]
    public List<InvoicePaymnent>? Payments { get; set; }

}

/// <summary>
/// Individual parking violation record
/// </summary>
public class InvoicePaymnent
{
    /// <summary>
    /// Table
    /// </summary>
    [JsonProperty("table")]
    public string Table { get; set; } = string.Empty; //for now Talls or Fees see InvoiceDetailsModel

    /// <summary>
    /// Payment Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ampunt
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

}

