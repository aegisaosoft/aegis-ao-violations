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
/// Response model for user sign-in
/// </summary>
public class SigninResponse
{
    /// <summary>
    /// The result containing user data and token
    /// </summary>
    [JsonProperty("result")]
    public SigninResult Result { get; set; } = new();

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
/// Result data from sign-in response
/// </summary>
public class SigninResult
{
    /// <summary>
    /// Authentication token
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration date and time
    /// </summary>
    [JsonProperty("tokenExpired")]
    public DateTime TokenExpired { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's middle name
    /// </summary>
    [JsonProperty("middleName")]
    public string? MiddleName { get; set; }

    /// <summary>
    /// User's last name
    /// </summary>
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's nickname/display name
    /// </summary>
    [JsonProperty("nickName")]
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number
    /// </summary>
    [JsonProperty("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Employer ID
    /// </summary>
    [JsonProperty("employerId")]
    public string EmployerId { get; set; } = string.Empty;

    /// <summary>
    /// First address line
    /// </summary>
    [JsonProperty("addressLine1")]
    public string AddressLine1 { get; set; } = string.Empty;

    /// <summary>
    /// Second address line
    /// </summary>
    [JsonProperty("addressLine2")]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// User's birth date
    /// </summary>
    [JsonProperty("birthDate")]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// URL to user's profile image
    /// </summary>
    [JsonProperty("imageURL")]
    public string ImageURL { get; set; } = string.Empty;

    /// <summary>
    /// City ID
    /// </summary>
    [JsonProperty("cityId")]
    public string CityId { get; set; } = string.Empty;

    /// <summary>
    /// City name
    /// </summary>
    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State ID
    /// </summary>
    [JsonProperty("stateId")]
    public string StateId { get; set; } = string.Empty;

    /// <summary>
    /// Whether user is an owner
    /// </summary>
    [JsonProperty("isOwner")]
    public bool IsOwner { get; set; }

    /// <summary>
    /// Whether user is a renter
    /// </summary>
    [JsonProperty("isRenter")]
    public bool IsRenter { get; set; }

    /// <summary>
    /// Whether user is an employee
    /// </summary>
    [JsonProperty("isEmployee")]
    public bool IsEmployee { get; set; }

    /// <summary>
    /// Whether user is an admin
    /// </summary>
    [JsonProperty("isAdmin")]
    public bool? IsAdmin { get; set; }

    /// <summary>
    /// Whether employee request is accepted
    /// </summary>
    [JsonProperty("isEmployeeRequestAccepted")]
    public bool IsEmployeeRequestAccepted { get; set; }

    /// <summary>
    /// Whether user is a company
    /// </summary>
    [JsonProperty("isCompany")]
    public bool IsCompany { get; set; }
}
