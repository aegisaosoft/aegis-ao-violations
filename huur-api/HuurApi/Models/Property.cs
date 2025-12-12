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
/// Property listing information
/// </summary>
public class Property
{
    /// <summary>
    /// Unique property identifier
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Property title/name
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Property description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Property price
    /// </summary>
    [JsonProperty("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// Currency for the price
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Property location information
    /// </summary>
    [JsonProperty("location")]
    public Location Location { get; set; } = new();

    /// <summary>
    /// List of property features
    /// </summary>
    [JsonProperty("features")]
    public List<string> Features { get; set; } = new();

    /// <summary>
    /// List of property image URLs
    /// </summary>
    [JsonProperty("images")]
    public List<string> Images { get; set; } = new();

    /// <summary>
    /// When the property becomes available
    /// </summary>
    [JsonProperty("availableFrom")]
    public DateTime? AvailableFrom { get; set; }

    /// <summary>
    /// Type of property (e.g., apartment, house)
    /// </summary>
    [JsonProperty("propertyType")]
    public string PropertyType { get; set; } = string.Empty;

    /// <summary>
    /// Number of bedrooms
    /// </summary>
    [JsonProperty("bedrooms")]
    public int Bedrooms { get; set; }

    /// <summary>
    /// Number of bathrooms
    /// </summary>
    [JsonProperty("bathrooms")]
    public int Bathrooms { get; set; }

    /// <summary>
    /// Property size in square feet/meters
    /// </summary>
    [JsonProperty("size")]
    public int Size { get; set; }

    /// <summary>
    /// Whether the property is furnished
    /// </summary>
    [JsonProperty("furnished")]
    public bool Furnished { get; set; }

    /// <summary>
    /// Whether pets are allowed
    /// </summary>
    [JsonProperty("petsAllowed")]
    public bool PetsAllowed { get; set; }

    /// <summary>
    /// Whether smoking is allowed
    /// </summary>
    [JsonProperty("smokingAllowed")]
    public bool SmokingAllowed { get; set; }

    /// <summary>
    /// When the property was created
    /// </summary>
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the property was last updated
    /// </summary>
    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Geographic location information
/// </summary>
public class Location
{
    /// <summary>
    /// Street address
    /// </summary>
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// City name
    /// </summary>
    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    [JsonProperty("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Country name
    /// </summary>
    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    [JsonProperty("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    [JsonProperty("longitude")]
    public double? Longitude { get; set; }
}
