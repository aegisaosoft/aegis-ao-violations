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

using Microsoft.EntityFrameworkCore;
using AegisViolations.Models;
using AegisViolations.Services;

namespace AegisViolations.Data;

public class ViolationsDbContext : DbContext
{
    public ViolationsDbContext(DbContextOptions<ViolationsDbContext> options) : base(options)
    {
    }

    public DbSet<Violation> Violations { get; set; }
    public DbSet<ViolationsRequest> ViolationsRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Violation entity
        modelBuilder.Entity<Violation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");

            // Configure company_id foreign key
            // Note: We're not creating a Company entity here since it's in a different context
            // The foreign key constraint is handled at the database level via the SQL migration
            entity.HasIndex(e => e.CompanyId).HasDatabaseName("idx_violations_company_id");
            entity.HasIndex(e => e.Tag).HasDatabaseName("idx_violations_tag");
            entity.HasIndex(e => e.State).HasDatabaseName("idx_violations_state");
            entity.HasIndex(e => e.IssueDate).HasDatabaseName("idx_violations_issue_date");
            entity.HasIndex(e => e.NoticeNumber).HasDatabaseName("idx_violations_notice_number");
            entity.HasIndex(e => e.CitationNumber).HasDatabaseName("idx_violations_citation_number");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("idx_violations_is_active");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_violations_created_at");
            entity.HasIndex(e => new { e.CompanyId, e.Tag }).HasDatabaseName("idx_violations_company_tag");
            entity.HasIndex(e => new { e.CompanyId, e.State }).HasDatabaseName("idx_violations_company_state");

            // Configure decimal precision
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2);

            // Configure timestamps
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure ViolationsRequest entity
        modelBuilder.Entity<ViolationsRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.RequestDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.CompanyId).HasDatabaseName("idx_violations_requests_company_id");
            entity.HasIndex(e => e.RequestDateTime).HasDatabaseName("idx_violations_requests_request_datetime");
        });
    }
}

