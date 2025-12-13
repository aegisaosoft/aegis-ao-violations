/*
 *
 * Copyright (c) 2025 Alexander Orlov.
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
/// Company model
/// </summary>
public class Company
{
    /// <summary>
    /// Company ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Company name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// State ID
    /// </summary>
    [JsonProperty("stateId")]
    public string? StateId { get; set; }

    /// <summary>
    /// Created by timestamp
    /// </summary>
    [JsonProperty("createdBy")]
    public DateTime CreatedBy { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    [JsonProperty("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// Charge policy
    /// </summary>
    [JsonProperty("chargePolicy")]
    public int ChargePolicy { get; set; }

    /// <summary>
    /// HQ token
    /// </summary>
    [JsonProperty("hqToken")]
    public string? HqToken { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Renteon ID
    /// </summary>
    [JsonProperty("renteonId")]
    public string? RenteonId { get; set; }
}

/// <summary>
/// Response model for GET /Companies/all
/// </summary>
public class GetCompaniesAllResponse
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
    /// List of companies
    /// </summary>
    [JsonProperty("result")]
    public List<Company> Result { get; set; } = new List<Company>();
}

/// <summary>
/// Response model for GET /Companies/active
/// </summary>
public class GetCompaniesActiveResponse
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
    /// List of active companies
    /// </summary>
    [JsonProperty("result")]
    public List<Company> Result { get; set; } = new List<Company>();
}

