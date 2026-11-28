using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Features.DataRetention;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories;

public sealed class RetentionCleanupRepository(AppDbContext context) : IRetentionCleanupRepository
{
    public Task<int> PurgeReadNotificationsBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken)
        => DeleteBatchAsync(context.Notifications.IgnoreQueryFilters()
            .Where(x => x.IsRead && x.UpdatedAt != null && x.UpdatedAt <= cutoffUtc), batchSize, cancellationToken);

    public Task<int> PurgeClosedTeacherRequestsBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken)
        => DeleteBatchAsync(context.TeacherAccessRequests.IgnoreQueryFilters()
            .Where(x => (x.Status != TeacherAccessRequestStatus.Pending || x.ExpiresAtUtc <= cutoffUtc)
                && (x.UpdatedAt ?? x.ExpiresAtUtc) <= cutoffUtc), batchSize, cancellationToken);

    public Task<int> PurgeClosedEnrollmentRequestsBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken)
        => DeleteBatchAsync(context.EnrollmentRequests.IgnoreQueryFilters()
            .Where(x => x.Status != EnrollmentRequestStatus.Pending
                && (x.UpdatedAt ?? x.CreatedAt) <= cutoffUtc), batchSize, cancellationToken);

    public Task<int> PurgeClosedGroupAdminRequestsBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken)
        => DeleteBatchAsync(context.GroupAdminRequests.IgnoreQueryFilters()
            .Where(x => x.Status != GroupAdminRequestStatus.Pending
                && (x.UpdatedAt ?? x.CreatedAt) <= cutoffUtc), batchSize, cancellationToken);

    public async Task<bool> IsStoredFileReferencedAsync(string publicId, CancellationToken cancellationToken)
    {
        if (await context.SessionAttachments.IgnoreQueryFilters().AnyAsync(x => x.PublicId == publicId, cancellationToken))
            return true;
        if (await context.Transactions.IgnoreQueryFilters().AnyAsync(x => x.ReceiptImagePublicId == publicId, cancellationToken))
            return true;
        return await context.TeacherPaymentSettings.IgnoreQueryFilters()
            .AnyAsync(x => x.QrImagePublicId == publicId, cancellationToken);
    }

    private async Task<int> DeleteBatchAsync<TEntity>(
        IQueryable<TEntity> candidates, int batchSize, CancellationToken cancellationToken)
        where TEntity : class
    {
        var take = Math.Clamp(batchSize, 1, DataRetentionPolicy.DatabaseBatchSize);
        var ids = await candidates
            .OrderBy(x => EF.Property<DateTime>(x, "CreatedAt"))
            .ThenBy(x => EF.Property<Guid>(x, "Id"))
            .Select(x => EF.Property<Guid>(x, "Id"))
            .Take(take)
            .ToListAsync(cancellationToken);

        if (ids.Count == 0) return 0;
        return await candidates
            .Where(x => ids.Contains(EF.Property<Guid>(x, "Id")))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
