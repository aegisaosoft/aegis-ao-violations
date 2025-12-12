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
using System.Text.Json.Serialization;

namespace HuurApi.Models;

/// <summary>
/// Response model for parking violations
/// </summary>
public class ParkingViolationsResponse
{
    /// <summary>
    /// Array of parking violation records
    /// </summary>
    [JsonProperty("result")]
    public List<ParkingViolation> Result { get; set; } = new();

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
/// Individual parking violation record
/// </summary>
public class ParkingViolation
{
    /// <summary>
    /// Violation ID
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Citation number
    /// </summary>
    [JsonProperty("citationNumber")]
    public string? CitationNumber { get; set; }

    /// <summary>
    /// Notice number
    /// </summary>
    [JsonProperty("noticeNumber")]
    public string? NoticeNumber { get; set; }

    /// <summary>
    /// Provider
    /// </summary>
    [JsonProperty("provider")]
    public int Provider { get; set; }

    /// <summary>
    /// Agency
    /// </summary>
    [JsonProperty("agency")]
    public string? Agency { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [JsonProperty("address")]
    public string? Address { get; set; }

    /// <summary>
    /// Tag
    /// </summary>
    [JsonProperty("tag")]
    public string? Tag { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string? State { get; set; }

    /// <summary>
    /// Issue date
    /// </summary>
    [JsonProperty("issueDate")]
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Start date
    /// </summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date
    /// </summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency
    /// </summary>
    [JsonProperty("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Fine type
    /// </summary>
    [JsonProperty("fineType")]
    public int FineType { get; set; }

    /// <summary>
    /// Note
    /// </summary>
    [JsonProperty("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Link
    /// </summary>
    [JsonProperty("link")]
    public string? Link { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Response model for creating parking violations
/// </summary>
public class CreateParkingViolationResponse
{
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

    /// <summary>
    /// Created parking violation data (if successful)
    /// </summary>
    [JsonProperty("result")]
    public ParkingViolation? Result { get; set; }
}

/// <summary>
/// Request model for updating a parking violation
/// </summary>
public class UpdateParkingViolationRequest
{
    /// <summary>
    /// Citation number
    /// </summary>
    [JsonProperty("citationNumber")]
    public string? CitationNumber { get; set; }

    /// <summary>
    /// Notice number
    /// </summary>
    [JsonProperty("noticeNumber")]
    public string? NoticeNumber { get; set; }

    /// <summary>
    /// Provider
    /// </summary>
    [JsonProperty("provider")]
    public int Provider { get; set; }

    /// <summary>
    /// Agency
    /// </summary>
    [JsonProperty("agency")]
    public string? Agency { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [JsonProperty("address")]
    public string? Address { get; set; }

    /// <summary>
    /// Tag
    /// </summary>
    [JsonProperty("tag")]
    public string? Tag { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string? State { get; set; }

    /// <summary>
    /// Issue date
    /// </summary>
    [JsonProperty("issueDate")]
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Start date
    /// </summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date
    /// </summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency
    /// </summary>
    [JsonProperty("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Fine type
    /// </summary>
    [JsonProperty("fineType")]
    public int FineType { get; set; }

    /// <summary>
    /// Note
    /// </summary>
    [JsonProperty("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Link
    /// </summary>
    [JsonProperty("link")]
    public string? Link { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Request model for creating a parking violation
/// </summary>
public class CreateParkingViolationRequest
{
    /// <summary>
    /// Citation number
    /// </summary>
    [JsonProperty("citationNumber")]
    public string? CitationNumber { get; set; }

    /// <summary>
    /// Notice number
    /// </summary>
    [JsonProperty("noticeNumber")]
    public string? NoticeNumber { get; set; }

    /// <summary>
    /// Provider
    /// </summary>
    [JsonProperty("provider")]
    public int Provider { get; set; }

    /// <summary>
    /// Agency
    /// </summary>
    [JsonProperty("agency")]
    public string? Agency { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [JsonProperty("address")]
    public string? Address { get; set; }

    /// <summary>
    /// Tag
    /// </summary>
    [JsonProperty("tag")]
    public string? Tag { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string? State { get; set; }

    /// <summary>
    /// Issue date
    /// </summary>
    [JsonProperty("issueDate")]
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Start date
    /// </summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date
    /// </summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency
    /// </summary>
    [JsonProperty("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Fine type
    /// </summary>
    [JsonProperty("fineType")]
    public int FineType { get; set; }

    /// <summary>
    /// Note
    /// </summary>
    [JsonProperty("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Link
    /// </summary>
    [JsonProperty("link")]
    public string? Link { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Response model for updating a parking violation
/// </summary>
public class UpdateParkingViolationResponse
{
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

    /// <summary>
    /// Updated parking violation data (if successful)
    /// </summary>
    [JsonProperty("result")]
    public ParkingViolation? Result { get; set; }
}

/// <summary>
/// Response model for getting a single parking violation
/// </summary>
public class GetParkingViolationResponse
{
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

    /// <summary>
    /// Parking violation data (if successful)
    /// </summary>
    [JsonProperty("result")]
    public ParkingViolation? Result { get; set; }
}

/// <summary>
/// Response model for getting parking violations by agency and notice number
/// </summary>
public class GetParkingViolationByAgencyResponse
{
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

    /// <summary>
    /// Parking violation data (if successful)
    /// </summary>
    [JsonProperty("result")]
    public ParkingViolation? Result { get; set; }
}

/// <summary>
/// Request model for updating payment status of parking violations
/// </summary>
public class UpdateViolationPaymentStatusRequest
{
    /// <summary>
    /// List of violation IDs to update
    /// </summary>
    [JsonProperty("ids")]
    public List<string> Ids { get; set; } = new List<string>();

    /// <summary>
    /// New payment status to set
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Company ID associated with the update
    /// </summary>
    [JsonProperty("companyId")]
    public string CompanyId { get; set; } = string.Empty;
}

/// <summary>
/// Response model for updating payment status
/// </summary>
public class UpdateViolationPaymentStatusResponse
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
    /// Update result
    /// </summary>
    [JsonProperty("result")]
    public bool Result { get; set; }
}

/// <summary>
/// Response model for deleting a parking violation
/// </summary>
public class DeleteParkingViolationResponse
{
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

    /// <summary>
    /// Deletion result
    /// </summary>
    [JsonProperty("result")]
    public bool Result { get; set; }
}

