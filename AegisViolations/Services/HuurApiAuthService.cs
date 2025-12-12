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

using HuurApi.Models;
using HuurApi.Services;
using AegisViolations.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AegisViolations.Services;

/// <summary>
/// Service for managing Huur API authentication and providing authenticated API client
/// </summary>
public interface IHuurApiAuthService
{
    /// <summary>
    /// Gets an authenticated bearer token for Huur API
    /// </summary>
    Task<string> GetBearerTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Forces token refresh by clearing cache and re-authenticating
    /// </summary>
    Task<string> RefreshTokenAsync(CancellationToken cancellationToken = default);
}

public class HuurApiAuthService : IHuurApiAuthService
{
    private readonly ViolationsDbContext _context;
    private readonly IHuurApiClient _huurApiClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<HuurApiAuthService> _logger;
    private const string TokenCacheKey = "huur_api_bearer_token";
    private const int TokenCacheMinutes = 55; // Cache for 55 minutes (tokens typically expire in 60 minutes)

    public HuurApiAuthService(
        ViolationsDbContext context,
        IHuurApiClient huurApiClient,
        IMemoryCache cache,
        ILogger<HuurApiAuthService> logger)
    {
        _context = context;
        _huurApiClient = huurApiClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken = default)
    {
        // Try to get token from cache
        if (_cache.TryGetValue(TokenCacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
        {
            _logger.LogDebug("Using cached Huur API bearer token");
            return cachedToken;
        }

        // Token not in cache or expired, authenticate
        return await AuthenticateAsync(cancellationToken);
    }

    public async Task<string> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        // Clear cache and re-authenticate
        _cache.Remove(TokenCacheKey);
        return await AuthenticateAsync(cancellationToken);
    }

    private async Task<string> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Authenticating with Huur API...");

            // Get credentials from settings table
            string? email = null;
            string? password = null;

            try
            {
                await _context.Database.OpenConnectionAsync(cancellationToken);
                using var emailCommand = _context.Database.GetDbConnection().CreateCommand();
                emailCommand.CommandText = "SELECT value FROM settings WHERE key = 'huur.api.email'";
                var emailResult = await emailCommand.ExecuteScalarAsync(cancellationToken);
                email = emailResult?.ToString();

                using var passwordCommand = _context.Database.GetDbConnection().CreateCommand();
                passwordCommand.CommandText = "SELECT value FROM settings WHERE key = 'huur.api.password'";
                var passwordResult = await passwordCommand.ExecuteScalarAsync(cancellationToken);
                password = passwordResult?.ToString();
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _logger.LogError("Huur API credentials not found in settings table");
                throw new InvalidOperationException("Huur API credentials not configured. Please add 'huur.api.email' and 'huur.api.password' to the settings table.");
            }

            // Sign in to get bearer token
            var signinRequest = new SigninRequest
            {
                Email = email,
                Password = password
            };

            var signinResponse = await _huurApiClient.SigninAsync(signinRequest, cancellationToken);
            if (signinResponse.Reason != 0)
            {
                _logger.LogError("Failed to authenticate with Huur API. Reason: {Reason}, Message: {Message}",
                    signinResponse.Reason, signinResponse.Message);
                throw new UnauthorizedAccessException($"Failed to authenticate with Huur API: {signinResponse.Message}");
            }

            var bearerToken = signinResponse.Result.Token;
            if (string.IsNullOrEmpty(bearerToken))
            {
                _logger.LogError("Received empty token from Huur API");
                throw new UnauthorizedAccessException("Failed to get authentication token from Huur API");
            }

            // Cache the token
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(TokenCacheMinutes)
            };
            _cache.Set(TokenCacheKey, bearerToken, cacheOptions);

            _logger.LogInformation("Successfully authenticated with Huur API and cached token");
            return bearerToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating with Huur API");
            throw;
        }
    }
}

