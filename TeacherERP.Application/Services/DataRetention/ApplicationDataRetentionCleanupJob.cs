using Microsoft.Extensions.Logging;
using TeacherERP.Application.Features.DataRetention;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Services.DataRetention;

public sealed class ApplicationDataRetentionCleanupJob(
    IRetentionCleanupRepository repository,
    TimeProvider timeProvider,
    ILogger<ApplicationDataRetentionCleanupJob> logger) : IApplicationDataRetentionCleanupJob
{
    public Task PurgeReadNotificationsAsync()
        => PurgeAsync("read notifications", DataRetentionPolicy.ReadNotificationRetention,
            repository.PurgeReadNotificationsBeforeAsync);

    public Task PurgeClosedTeacherRequestsAsync()
        => PurgeAsync("closed teacher access requests", DataRetentionPolicy.ClosedRequestRetention,
            repository.PurgeClosedTeacherRequestsBeforeAsync);

    public Task PurgeClosedEnrollmentRequestsAsync()
        => PurgeAsync("closed enrollment requests", DataRetentionPolicy.ClosedRequestRetention,
            repository.PurgeClosedEnrollmentRequestsBeforeAsync);

    public Task PurgeClosedGroupAdminRequestsAsync()
        => PurgeAsync("closed group administrator requests", DataRetentionPolicy.ClosedRequestRetention,
            repository.PurgeClosedGroupAdminRequestsBeforeAsync);

    private async Task PurgeAsync(string category, TimeSpan retention,
        Func<DateTime, int, CancellationToken, Task<int>> purge)
    {
        var cutoff = timeProvider.GetUtcNow().UtcDateTime - retention;
        var deleted = await purge(cutoff, DataRetentionPolicy.DatabaseBatchSize, CancellationToken.None);
        logger.LogInformation("Retention cleanup removed {DeletedCount} {Category} in one bounded batch before {Cutoff:u}.",
            deleted, category, cutoff);
    }
}
