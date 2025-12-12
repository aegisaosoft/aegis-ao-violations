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
/// Represents company information for an employee
/// </summary>
public class CompanyInfo
{
    /// <summary>
    /// Company ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Middle name
    /// </summary>
    [JsonProperty("middleName")]
    public string MiddleName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Nick name
    /// </summary>
    [JsonProperty("nickName")]
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// Company name
    /// </summary>
    [JsonProperty("company")]
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// Is company flag
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
    /// Address line 1
    /// </summary>
    [JsonProperty("addressLine1")]
    public string AddressLine1 { get; set; } = string.Empty;

    /// <summary>
    /// Address line 2
    /// </summary>
    [JsonProperty("addressLine2")]
    public string AddressLine2 { get; set; } = string.Empty;

    /// <summary>
    /// ZIP code
    /// </summary>
    [JsonProperty("zipCode")]
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Birth date
    /// </summary>
    [JsonProperty("birthDate")]
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Employer ID
    /// </summary>
    [JsonProperty("employerId")]
    public string EmployerId { get; set; } = string.Empty;

    /// <summary>
    /// Image URL
    /// </summary>
    [JsonProperty("imageURL")]
    public string ImageURL { get; set; } = string.Empty;

    /// <summary>
    /// Receive notification flag
    /// </summary>
    [JsonProperty("receiveNotification")]
    public bool ReceiveNotification { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public int Type { get; set; }

    /// <summary>
    /// About me description
    /// </summary>
    [JsonProperty("aboutMe")]
    public string AboutMe { get; set; } = string.Empty;

    /// <summary>
    /// Languages
    /// </summary>
    [JsonProperty("languages")]
    public string Languages { get; set; } = string.Empty;

    /// <summary>
    /// Rating
    /// </summary>
    [JsonProperty("rating")]
    public int Rating { get; set; }

    /// <summary>
    /// Frequency
    /// </summary>
    [JsonProperty("frequency")]
    public int Frequency { get; set; }

    /// <summary>
    /// Referral code
    /// </summary>
    [JsonProperty("referralCode")]
    public string ReferralCode { get; set; } = string.Empty;

    /// <summary>
    /// Request delete account flag
    /// </summary>
    [JsonProperty("requestDeleteAccount")]
    public bool RequestDeleteAccount { get; set; }
}

/// <summary>
/// Represents a document associated with an employee
/// </summary>
public class EmployeeDocument
{
    /// <summary>
    /// Document ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID
    /// </summary>
    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Role
    /// </summary>
    [JsonProperty("role")]
    public int Role { get; set; }

    /// <summary>
    /// DTD (Document Type Definition)
    /// </summary>
    [JsonProperty("dtd")]
    public int Dtd { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Is empty flag
    /// </summary>
    [JsonProperty("isEmpty")]
    public bool IsEmpty { get; set; }

    /// <summary>
    /// Deleted flag
    /// </summary>
    [JsonProperty("deleted")]
    public bool Deleted { get; set; }

    /// <summary>
    /// Created by timestamp
    /// </summary>
    [JsonProperty("createdBy")]
    public DateTime CreatedBy { get; set; }
}

/// <summary>
/// Represents the result data for company info endpoint
/// </summary>
public class CompanyInfoResult
{
    /// <summary>
    /// Company information
    /// </summary>
    [JsonProperty("companyInfo")]
    public CompanyInfo CompanyInfo { get; set; } = new CompanyInfo();

    /// <summary>
    /// List of documents
    /// </summary>
    [JsonProperty("documents")]
    public List<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
}

/// <summary>
/// Response model for GET /Employees/companyinfo
/// </summary>
public class GetCompanyInfoResponse
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
    /// Company info result
    /// </summary>
    [JsonProperty("result")]
    public CompanyInfoResult? Result { get; set; }
}
