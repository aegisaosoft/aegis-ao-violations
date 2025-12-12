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
using Microsoft.Extensions.DependencyInjection;

namespace HuurApi.Services;

/// <summary>
/// Extension methods for registering Huur API services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Huur API client and related services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure the options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHuurApi(this IServiceCollection services, Action<HuurApiOptions> configureOptions)
    {
        // Configure options
        services.Configure(configureOptions);
        
        // Add HTTP client
        services.AddHttpClient<IHuurApiClient, HuurApiClient>();
        
        return services;
    }
}
