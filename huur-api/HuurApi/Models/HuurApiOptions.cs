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

namespace HuurApi.Models;

/// <summary>
/// Configuration options for the Huur API client
/// </summary>
public class HuurApiOptions
{
    /// <summary>
    /// Base URL for the Huur API
    /// </summary>
    //public string BaseUrl { get; set; } = "https://agsm-huur-production-api.azurewebsites.net";
    public string BaseUrl { get; set; } = "https://agsm-back.azurewebsites.net";

    /// <summary>
    /// Documentation URL for the Huur API
    /// </summary>
    public string DocumentationUrl { get; set; } = "https://agsm-huur-production-api.azurewebsites.net/";

    /// <summary>
    /// API key for authentication (if required)
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Timeout for HTTP requests in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum number of retries for failed requests
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Delay between retries in milliseconds
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Path to the configuration file for storing tokens
    /// </summary>
    public string TokenConfigPath { get; set; } = "huur-api-config.json";

    /// <summary>
    /// Whether to persist authentication tokens
    /// </summary>
    public bool PersistTokens { get; set; } = true;
}
