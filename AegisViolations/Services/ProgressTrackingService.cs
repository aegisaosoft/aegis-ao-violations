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
    void UpdateProgress(string requestId, int progress, string? status = null);
    ProgressInfo? GetProgress(string requestId);
    void RemoveProgress(string requestId);
}

public class ProgressTrackingService : IProgressTrackingService
{
    private readonly ConcurrentDictionary<string, ProgressInfo> _progressStore = new();
    private readonly ILogger<ProgressTrackingService> _logger;

    public ProgressTrackingService(ILogger<ProgressTrackingService> logger)
    {
        _logger = logger;
    }

    public string CreateProgressTracker()
    {
        var requestId = Guid.NewGuid().ToString();
        _progressStore.TryAdd(requestId, new ProgressInfo
        {
            RequestId = requestId,
            Progress = 0,
            Status = "Initializing",
            StartedAt = DateTime.UtcNow
        });
        _logger.LogDebug("Created progress tracker: {RequestId}", requestId);
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
                StartedAt = DateTime.UtcNow
            },
            (key, existing) =>
            {
                existing.Progress = progress;
                if (status != null)
                {
                    existing.Status = status;
                }
                existing.LastUpdated = DateTime.UtcNow;
                return existing;
            });

        _logger.LogDebug("Updated progress for {RequestId}: {Progress}% - {Status}", requestId, progress, status ?? "Processing");
    }

    public ProgressInfo? GetProgress(string requestId)
    {
        if (string.IsNullOrEmpty(requestId))
            return null;

        _progressStore.TryGetValue(requestId, out var progressInfo);
        return progressInfo;
    }

    public void RemoveProgress(string requestId)
    {
        if (string.IsNullOrEmpty(requestId))
            return;

        _progressStore.TryRemove(requestId, out _);
        _logger.LogDebug("Removed progress tracker: {RequestId}", requestId);
    }
}

public class ProgressInfo
{
    public string RequestId { get; set; } = string.Empty;
    public int Progress { get; set; }
    public string Status { get; set; } = "Processing";
    public DateTime StartedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
    public string? Error { get; set; }
}

