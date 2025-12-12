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

using HuurApi.Models;

namespace HuurApi.Services;

/// <summary>
/// Interface for managing authentication tokens
/// </summary>
public interface ITokenManager
{
    /// <summary>
    /// Saves authentication tokens to configuration file
    /// </summary>
    /// <param name="loginResponse">Login response containing tokens</param>
    /// <param name="username">Username for the tokens</param>
    Task SaveTokensAsync(LoginResponse loginResponse, string username);

    /// <summary>
    /// Loads authentication tokens from configuration file
    /// </summary>
    /// <returns>Token configuration if available and valid</returns>
    Task<TokenConfiguration?> LoadTokensAsync();

    /// <summary>
    /// Clears stored authentication tokens
    /// </summary>
    Task ClearTokensAsync();

    /// <summary>
    /// Checks if stored tokens are valid
    /// </summary>
    /// <returns>True if tokens are valid and not expired</returns>
    Task<bool> AreTokensValidAsync();

    /// <summary>
    /// Gets the current access token if valid
    /// </summary>
    /// <returns>Access token if valid, null otherwise</returns>
    Task<string?> GetValidAccessTokenAsync();
}
