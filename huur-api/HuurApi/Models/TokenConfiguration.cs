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
/// Configuration for storing authentication tokens
/// </summary>
public class TokenConfiguration
{
    /// <summary>
    /// Access token for API authentication
    /// </summary>
    [JsonProperty("accessToken")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    [JsonProperty("refreshToken")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// When the access token expires
    /// </summary>
    [JsonProperty("tokenExpiresAt")]
    public DateTime? TokenExpiresAt { get; set; }

    /// <summary>
    /// Username associated with the tokens
    /// </summary>
    [JsonProperty("username")]
    public string? Username { get; set; }

    /// <summary>
    /// When the tokens were last updated
    /// </summary>
    [JsonProperty("lastUpdated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the tokens are currently valid
    /// </summary>
    [JsonProperty("isValid")]
    public bool IsValid => !string.IsNullOrEmpty(AccessToken) && 
                          TokenExpiresAt.HasValue && 
                          TokenExpiresAt!.Value > DateTime.UtcNow;

    /// <summary>
    /// Debug method to get detailed validation info
    /// </summary>
    /// <returns>Detailed validation information as a string</returns>
    public string GetValidationDetails()
    {
        var hasToken = !string.IsNullOrEmpty(AccessToken);
        var hasExpiry = TokenExpiresAt.HasValue;
        var isNotExpired = hasExpiry && TokenExpiresAt!.Value > DateTime.UtcNow;
        var currentUtc = DateTime.UtcNow;
        
        return $"Token validation details: HasToken={hasToken}, HasExpiry={hasExpiry}, IsNotExpired={isNotExpired}, " +
               $"CurrentUTC={currentUtc:yyyy-MM-dd HH:mm:ss}, ExpiresAt={TokenExpiresAt:yyyy-MM-dd HH:mm:ss}";
    }
}
