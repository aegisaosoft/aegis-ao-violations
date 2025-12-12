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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AegisViolations.Models;

/// <summary>
/// ViolationsRequest entity representing a violation search request in the database
/// </summary>
[Table("violations_requests")]
public class ViolationsRequest
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("company_id")]
    public Guid? CompanyId { get; set; }

    [Column("vehicle_count")]
    public int VehicleCount { get; set; } = 0;

    [Column("requests_count")]
    public int RequestsCount { get; set; } = 0;

    [Column("finders_count")]
    public int FindersCount { get; set; } = 0;

    [Column("request_datetime")]
    public DateTime RequestDateTime { get; set; } = DateTime.UtcNow;

    [Column("violations_found")]
    public int ViolationsFound { get; set; } = 0;

    [Column("requestor")]
    [MaxLength(255)]
    public string? Requestor { get; set; }
}

