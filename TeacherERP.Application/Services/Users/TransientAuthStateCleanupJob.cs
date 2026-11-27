using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Services.Users;

public sealed class TransientAuthStateCleanupJob(
    ITransientAuthStateStore store,
    TimeProvider timeProvider,
    ILogger<TransientAuthStateCleanupJob> logger) : ITransientAuthStateCleanupJob
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        const int batchSize = 250;
        var deleted = await store.DeleteExpiredAsync(timeProvider.GetUtcNow().UtcDateTime, batchSize, cancellationToken);
        logger.LogInformation("Removed {Count} expired transient authentication records in a batch capped at {BatchSize}", deleted, batchSize);
    }
}
