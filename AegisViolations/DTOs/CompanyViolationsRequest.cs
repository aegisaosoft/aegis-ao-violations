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

namespace AegisViolations.DTOs;

/// <summary>
/// Request model for searching violations by company
/// </summary>
public class CompanyViolationsRequest
{
    /// <summary>
    /// List of states to search in
    /// </summary>
    public List<string> States { get; set; } = new();

    /// <summary>
    /// Start date in YYYY-MM-DD format
    /// </summary>
    public string StartDate { get; set; } = string.Empty;

    /// <summary>
    /// End date in YYYY-MM-DD format
    /// </summary>
    public string EndDate { get; set; } = string.Empty;
}

