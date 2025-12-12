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
/// Criteria for searching properties
/// </summary>
public class PropertySearchCriteria
{
    /// <summary>
    /// City to search in
    /// </summary>
    [JsonProperty("city")]
    public string? City { get; set; }

    /// <summary>
    /// Minimum price for the property
    /// </summary>
    [JsonProperty("minPrice")]
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Maximum price for the property
    /// </summary>
    [JsonProperty("maxPrice")]
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Type of property (e.g., apartment, house)
    /// </summary>
    [JsonProperty("propertyType")]
    public string? PropertyType { get; set; }

    /// <summary>
    /// Minimum number of bedrooms
    /// </summary>
    [JsonProperty("minBedrooms")]
    public int? MinBedrooms { get; set; }

    /// <summary>
    /// Minimum number of bathrooms
    /// </summary>
    [JsonProperty("minBathrooms")]
    public int? MinBathrooms { get; set; }

    /// <summary>
    /// Whether the property is furnished
    /// </summary>
    [JsonProperty("furnished")]
    public bool? Furnished { get; set; }

    /// <summary>
    /// Whether pets are allowed
    /// </summary>
    [JsonProperty("petsAllowed")]
    public bool? PetsAllowed { get; set; }

    /// <summary>
    /// Whether smoking is allowed
    /// </summary>
    [JsonProperty("smokingAllowed")]
    public bool? SmokingAllowed { get; set; }

    /// <summary>
    /// When the property becomes available
    /// </summary>
    [JsonProperty("availableFrom")]
    public DateTime? AvailableFrom { get; set; }

    /// <summary>
    /// List of features the property should have
    /// </summary>
    [JsonProperty("features")]
    public List<string>? Features { get; set; }

    /// <summary>
    /// Search radius in kilometers
    /// </summary>
    [JsonProperty("radius")]
    public double? Radius { get; set; }

    /// <summary>
    /// Latitude for location-based search
    /// </summary>
    [JsonProperty("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitude for location-based search
    /// </summary>
    [JsonProperty("longitude")]
    public double? Longitude { get; set; }
}
