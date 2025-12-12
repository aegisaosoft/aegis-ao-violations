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
/// User profile information
/// </summary>
public class User
{
    /// <summary>
    /// Unique user identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number
    /// </summary>
    [JsonProperty("phone")]
    public string? Phone { get; set; }

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
    /// User's company name
    /// </summary>
    [JsonProperty("company")]
    public string? Company { get; set; }

    /// <summary>
    /// Whether this is a company account
    /// </summary>
    [JsonProperty("isCompany")]
    public bool IsCompany { get; set; }

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
    /// ZIP code
    /// </summary>
    [JsonProperty("zipCode")]
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// User's birth date
    /// </summary>
    [JsonProperty("birthDate")]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Employer ID
    /// </summary>
    [JsonProperty("employerId")]
    public string? EmployerId { get; set; }

    /// <summary>
    /// URL to user's profile image
    /// </summary>
    [JsonProperty("imageURL")]
    public string ImageURL { get; set; } = string.Empty;

    /// <summary>
    /// Whether user receives notifications
    /// </summary>
    [JsonProperty("receiveNotification")]
    public bool ReceiveNotification { get; set; }

    /// <summary>
    /// User type
    /// </summary>
    [JsonProperty("type")]
    public int Type { get; set; }

    /// <summary>
    /// About me description
    /// </summary>
    [JsonProperty("aboutMe")]
    public string? AboutMe { get; set; }

    /// <summary>
    /// Languages spoken
    /// </summary>
    [JsonProperty("languages")]
    public string? Languages { get; set; }

    /// <summary>
    /// User rating
    /// </summary>
    [JsonProperty("rating")]
    public decimal Rating { get; set; }

    /// <summary>
    /// Usage frequency
    /// </summary>
    [JsonProperty("frequency")]
    public int Frequency { get; set; }

    /// <summary>
    /// Referral code
    /// </summary>
    [JsonProperty("referralCode")]
    public string? ReferralCode { get; set; }

    /// <summary>
    /// Whether user requested account deletion
    /// </summary>
    [JsonProperty("requestDeleteAccount")]
    public bool RequestDeleteAccount { get; set; }
}

/// <summary>
/// Response model for getting user info via GET /Users
/// </summary>
public class GetUsersResponse
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
    /// User profile
    /// </summary>
    [JsonProperty("result")]
    public User? Result { get; set; }
}
