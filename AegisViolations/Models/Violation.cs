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
/// Violation entity representing a parking violation in the database
/// Based on HuurApi.Models.ParkingViolation structure
/// </summary>
[Table("violations")]
public class Violation
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("company_id")]
    public Guid CompanyId { get; set; }

    [Column("citation_number")]
    [MaxLength(255)]
    public string? CitationNumber { get; set; }

    [Column("notice_number")]
    [MaxLength(255)]
    public string? NoticeNumber { get; set; }

    [Column("provider")]
    public int Provider { get; set; } = 0;

    [Column("agency")]
    [MaxLength(255)]
    public string? Agency { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("tag")]
    [MaxLength(50)]
    public string? Tag { get; set; }

    [Column("state")]
    [MaxLength(10)]
    public string? State { get; set; }

    [Column("issue_date")]
    public DateTime? IssueDate { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("amount", TypeName = "numeric(10,2)")]
    public decimal Amount { get; set; } = 0.00m;

    [Column("currency")]
    [MaxLength(3)]
    public string? Currency { get; set; }

    [Column("payment_status")]
    public int PaymentStatus { get; set; } = 0;

    [Column("fine_type")]
    public int FineType { get; set; } = 0;

    [Column("note")]
    public string? Note { get; set; }

    [Column("link")]
    public string? Link { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property to Company (optional, if you want to include company data)
    // Note: We don't create a Company model here since it's in a different database context
    // If needed, you can add it later or use a shared models project
}

