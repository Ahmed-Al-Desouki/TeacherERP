using Hangfire;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Features.DataRetention;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Services.DataRetention;

public sealed class OrphanedFileCleanupJob(
    IFileStorage fileStorage,
    IRetentionCleanupRepository repository,
    IBackgroundJobClient backgroundJobs,
    ILogger<OrphanedFileCleanupJob> logger) : IOrphanedFileCleanupJob
{
    public async Task RunPageAsync(string? nextCursor = null)
    {
        var page = await fileStorage.ListApplicationFilesAsync(
            nextCursor, DataRetentionPolicy.ExternalFileBatchSize, CancellationToken.None);
        var scheduled = 0;

        foreach (var resource in page.Resources)
        {
            if (await repository.IsStoredFileReferencedAsync(resource.PublicId, CancellationToken.None)) continue;

            logger.LogWarning("Application file {PublicId} has no database reference; deletion is scheduled after the orphan grace period.", resource.PublicId);
            backgroundJobs.Schedule<IOrphanedFileCleanupJob>(
                job => job.DeleteCandidateAsync(resource.PublicId), DataRetentionPolicy.OrphanedFileGracePeriod);
            scheduled++;
        }

        logger.LogInformation(
            "Orphaned-file scan processed {ScannedCount} application assets and scheduled {ScheduledCount} candidates for delayed revalidation.",
            page.Resources.Count, scheduled);

        if (!string.IsNullOrWhiteSpace(page.NextCursor))
            backgroundJobs.Schedule<IOrphanedFileCleanupJob>(job => job.RunPageAsync(page.NextCursor), TimeSpan.FromMinutes(5));
    }

    public async Task DeleteCandidateAsync(string publicId)
    {
        if (await repository.IsStoredFileReferencedAsync(publicId, CancellationToken.None))
        {
            logger.LogInformation("Skipped orphan-file candidate {PublicId}; it gained a database reference during the grace period.", publicId);
            return;
        }

        var result = await fileStorage.DeleteAsync(publicId, CancellationToken.None);
        if (result.IsFailure)
            throw new InvalidOperationException($"Failed to delete confirmed orphaned application file ({result.Error.Code}).");

        logger.LogInformation("Deleted revalidated orphaned application file {PublicId}.", publicId);
    }
}
