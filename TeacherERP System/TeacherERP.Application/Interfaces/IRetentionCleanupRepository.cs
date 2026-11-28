namespace TeacherERP.Application.Interfaces;

public interface IRetentionCleanupRepository
{
    Task<int> PurgeReadNotificationsBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken);
    Task<int> PurgeClosedTeacherRequestsBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken);
    Task<int> PurgeClosedEnrollmentRequestsBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken);
    Task<int> PurgeClosedGroupAdminRequestsBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken);
    Task<bool> IsStoredFileReferencedAsync(string publicId, CancellationToken cancellationToken);
}
