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

using System.Collections.Concurrent;

namespace AegisViolations.Services;

public interface IProgressTrackingService
{
    string CreateProgressTracker();
    string CreateProgressTracker(Guid? companyId);
    void UpdateProgress(string requestId, int progress, string? status = null);
    ProgressInfo? GetProgress(string requestId);
    ProgressInfo? GetProgressByCompanyId(Guid companyId);
    void RemoveProgress(string requestId);
    bool IsCompanyProcessing(Guid companyId);
    void MarkTaskCompleted(string requestId);
}

public class ProgressTrackingService : IProgressTrackingService
{
    private readonly ConcurrentDictionary<string, ProgressInfo> _progressStore = new();
    private readonly ConcurrentDictionary<Guid, string> _companyIdToRequestId = new();
    private readonly ILogger<ProgressTrackingService> _logger;

    public ProgressTrackingService(ILogger<ProgressTrackingService> logger)
    {
        _logger = logger;
    }

    public string CreateProgressTracker()
    {
        return CreateProgressTracker(null);
    }

    public string CreateProgressTracker(Guid? companyId)
    {
        var requestId = Guid.NewGuid().ToString();
        var progressInfo = new ProgressInfo
        {
            RequestId = requestId,
            CompanyId = companyId,
            Progress = 0,
            Status = "Initializing",
            StartedAt = DateTime.UtcNow
        };
        
        _progressStore.TryAdd(requestId, progressInfo);
        
        if (companyId.HasValue)
        {
            _companyIdToRequestId.TryAdd(companyId.Value, requestId);
            _logger.LogDebug("Created progress tracker: {RequestId} for company {CompanyId}", requestId, companyId);
        }
        else
        {
            _logger.LogDebug("Created progress tracker: {RequestId}", requestId);
        }
        
        return requestId;
    }

    public void UpdateProgress(string requestId, int progress, string? status = null)
    {
        if (string.IsNullOrEmpty(requestId))
            return;

        // Clamp progress between 0 and 100
        progress = Math.Max(0, Math.Min(100, progress));

        _progressStore.AddOrUpdate(
            requestId,
            new ProgressInfo
            {
                RequestId = requestId,
                Progress = progress,
                Status = status ?? "Processing",
                StartedAt = DateTime.UtcNow,
                IsTaskRunning = true
            },
            (key, existing) =>
            {
                existing.Progress = progress;
                if (status != null)
                {
                    existing.Status = status;
                }
                existing.LastUpdated = DateTime.UtcNow;
                // If progress reaches 100%, mark task as not running (completed)
                if (progress >= 100 && existing.IsTaskRunning)
                {
                    existing.IsTaskRunning = false;
                    existing.CompletedAt = DateTime.UtcNow;
                }
                return existing;
            });

        _logger.LogDebug("Updated progress for {RequestId}: {Progress}% - {Status}", requestId, progress, status ?? "Processing");
    }

    public void MarkTaskCompleted(string requestId)
    {
        if (string.IsNullOrEmpty(requestId))
            return;

        if (_progressStore.TryGetValue(requestId, out var progress))
        {
            progress.IsTaskRunning = false;
            progress.CompletedAt = DateTime.UtcNow;
            progress.LastUpdated = DateTime.UtcNow;
            _logger.LogDebug("Marked task as completed for {RequestId}", requestId);
        }
    }

    public ProgressInfo? GetProgress(string requestId)
    {
        if (string.IsNullOrEmpty(requestId))
            return null;

        _progressStore.TryGetValue(requestId, out var progressInfo);
        return progressInfo;
    }

    public ProgressInfo? GetProgressByCompanyId(Guid companyId)
    {
        if (_companyIdToRequestId.TryGetValue(companyId, out var requestId))
        {
            return GetProgress(requestId);
        }
        return null;
    }

    public bool IsCompanyProcessing(Guid companyId)
    {
        if (!_companyIdToRequestId.TryGetValue(companyId, out var requestId))
        {
            return false;
        }

        var progress = GetProgress(requestId);
        if (progress == null)
        {
            // Clean up stale mapping
            _companyIdToRequestId.TryRemove(companyId, out _);
            return false;
        }

        // If task is still running, it's processing
        if (progress.IsTaskRunning)
        {
            return true;
        }

        // If task completed recently (within last 30 seconds), consider it still processing to prevent rapid restarts
        // This prevents infinite loops when tasks fail and restart immediately
        if (progress.CompletedAt.HasValue)
        {
            var timeSinceCompletion = DateTime.UtcNow - progress.CompletedAt.Value;
            // If there was an error, wait longer (60 seconds) before allowing restart
            var cooldownPeriod = !string.IsNullOrEmpty(progress.Error) ? 60 : 30;
            
            if (timeSinceCompletion.TotalSeconds < cooldownPeriod)
            {
                _logger.LogDebug("Task completed recently ({Seconds}s ago) for company {CompanyId}, preventing rapid restart. Cooldown: {Cooldown}s", 
                    timeSinceCompletion.TotalSeconds, companyId, cooldownPeriod);
                return true;
            }
        }

        // Check if process is still active (not completed or error)
        // If there's an error but task is still marked as running, it's processing
        if (progress.Progress < 100 && progress.IsTaskRunning)
        {
            return true;
        }

        return false;
    }

    public void RemoveProgress(string requestId)
    {
        if (string.IsNullOrEmpty(requestId))
            return;

        if (_progressStore.TryRemove(requestId, out var progressInfo))
        {
            // Remove company ID mapping if exists
            if (progressInfo.CompanyId.HasValue)
            {
                _companyIdToRequestId.TryRemove(progressInfo.CompanyId.Value, out _);
            }
            _logger.LogDebug("Removed progress tracker: {RequestId}", requestId);
        }
    }
}

public class ProgressInfo
{
    public string RequestId { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public int Progress { get; set; }
    public string Status { get; set; } = "Processing";
    public DateTime StartedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Error { get; set; }
    public bool IsTaskRunning { get; set; } = true;
}

