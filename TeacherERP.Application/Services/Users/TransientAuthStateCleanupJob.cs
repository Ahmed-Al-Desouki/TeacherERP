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
        var deleted = await store.DeleteExpiredAsync(timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
        logger.LogInformation("Removed {Count} expired transient authentication records", deleted);
    }
}
