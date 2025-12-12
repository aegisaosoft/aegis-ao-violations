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
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HuurApi.Services;

/// <summary>
/// Implementation of token management for Huur API
/// </summary>
public class TokenManager : ITokenManager
{
    private readonly HuurApiOptions _options;
    private readonly ILogger<TokenManager> _logger;
    private readonly string _configFilePath;

    /// <summary>
    /// Initializes a new instance of the TokenManager
    /// </summary>
    /// <param name="options">Configuration options for token management</param>
    /// <param name="logger">Logger for recording token operations</param>
    public TokenManager(IOptions<HuurApiOptions> options, ILogger<TokenManager> logger)
    {
        _options = options.Value;
        _logger = logger;
        
        // Determine the full path for the config file
        if (Path.IsPathRooted(_options.TokenConfigPath))
        {
            _configFilePath = _options.TokenConfigPath;
            _logger.LogInformation("Using absolute path for token config: {Path}", _configFilePath);
        }
        else
        {
            // Store in user's app data folder for security
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var huurApiPath = Path.Combine(appDataPath, "HuurApi");
            
            if (!Directory.Exists(huurApiPath))
            {
                Directory.CreateDirectory(huurApiPath);
            }
            
            _configFilePath = Path.Combine(huurApiPath, _options.TokenConfigPath);
            _logger.LogInformation("Using AppData path for token config: {Path}", _configFilePath);
        }
        
        _logger.LogInformation("Final token config path: {Path}", _configFilePath);
    }

    /// <summary>
    /// Saves authentication tokens to the configuration file
    /// </summary>
    /// <param name="loginResponse">Login response containing tokens</param>
    /// <param name="username">Username associated with the tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SaveTokensAsync(LoginResponse loginResponse, string username)
    {
        if (!_options.PersistTokens)
        {
            _logger.LogInformation("Token persistence is disabled");
            return;
        }

        try
        {
            var tokenConfig = new TokenConfiguration
            {
                AccessToken = loginResponse.Token,
                RefreshToken = loginResponse.RefreshToken,
                TokenExpiresAt = loginResponse.ExpiresAt,
                Username = username,
                LastUpdated = DateTime.UtcNow
            };

            var json = JsonConvert.SerializeObject(tokenConfig, Formatting.Indented);
            await File.WriteAllTextAsync(_configFilePath, json);
            
            _logger.LogInformation("Tokens saved successfully for user: {Username}", username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save tokens for user: {Username}", username);
            throw;
        }
    }

    /// <summary>
    /// Loads authentication tokens from the configuration file
    /// </summary>
    /// <returns>Token configuration if found, null otherwise</returns>
    public async Task<TokenConfiguration?> LoadTokensAsync()
    {
        if (!_options.PersistTokens)
        {
            _logger.LogInformation("Token persistence is disabled");
            return null;
        }

        _logger.LogInformation("Attempting to load tokens from: {Path}", _configFilePath);
        _logger.LogInformation("File exists: {Exists}", File.Exists(_configFilePath));

        try
        {
            if (!File.Exists(_configFilePath))
            {
                _logger.LogWarning("No token configuration file found at: {Path}", _configFilePath);
                return null;
            }

            var json = await File.ReadAllTextAsync(_configFilePath);
            var tokenConfig = JsonConvert.DeserializeObject<TokenConfiguration>(json);
            
            if (tokenConfig != null)
            {
                _logger.LogInformation("Token config loaded - Username: {Username}, Expires: {Expires}", 
                    tokenConfig.Username, tokenConfig.TokenExpiresAt);
                
                // Always return the token config if it exists - no validation or deletion
                _logger.LogInformation("Tokens loaded for user: {Username}", tokenConfig.Username);
                return tokenConfig;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load tokens from: {Path}", _configFilePath);
            return null;
        }
    }

    /// <summary>
    /// Clears stored authentication tokens by deleting the configuration file
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task ClearTokensAsync()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                File.Delete(_configFilePath);
                _logger.LogInformation("Tokens cleared successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to clear tokens - this is not critical");
            // Don't throw - this is not a critical operation
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if stored tokens exist and are valid
    /// </summary>
    /// <returns>True if tokens exist, false otherwise</returns>
    public async Task<bool> AreTokensValidAsync()
    {
        var tokenConfig = await LoadTokensAsync();
        // Always return true if tokens exist - no validation
        return tokenConfig != null && !string.IsNullOrEmpty(tokenConfig.AccessToken);
    }

    /// <summary>
    /// Retrieves the stored access token if it exists
    /// </summary>
    /// <returns>The access token if found, null otherwise</returns>
    public async Task<string?> GetValidAccessTokenAsync()
    {
        var tokenConfig = await LoadTokensAsync();
        // Always return the token if it exists - no validation
        return tokenConfig?.AccessToken;
    }
}
