using Hangfire;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Services.DataRetention;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Unit.DataRetention;

public sealed class OrphanedFileCleanupJobTests
{
    [Fact]
    public async Task ReportsCandidatesAndRechecksReferencesBeforeDelayedDeletion()
    {
        var storage = Substitute.For<IFileStorage>();
        var repository = Substitute.For<IRetentionCleanupRepository>();
        var jobs = Substitute.For<IBackgroundJobClient>();
        storage.ListApplicationFilesAsync(null, 100, CancellationToken.None).Returns(Task.FromResult(
            new FileStorageResourcePage(new[]
            {
                new FileStorageResource("teacher-erp/orphan"),
                new FileStorageResource("teacher-erp/referenced"),
                new FileStorageResource("teacher-erp/recent")
            }, null)));
        repository.IsStoredFileReferencedAsync("teacher-erp/orphan", CancellationToken.None).Returns(Task.FromResult(false));
        repository.IsStoredFileReferencedAsync("teacher-erp/referenced", CancellationToken.None).Returns(Task.FromResult(false));
        storage.DeleteAsync("teacher-erp/orphan", CancellationToken.None)
            .Returns(Task.FromResult(Result<bool>.Success(true)));

        var job = new OrphanedFileCleanupJob(storage, repository, jobs,
            NullLogger<OrphanedFileCleanupJob>.Instance);

        await job.RunPageAsync();
        await storage.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        // A reference added during the grace period cancels deletion; a still-unreferenced candidate is removed.
        repository.IsStoredFileReferencedAsync("teacher-erp/referenced", CancellationToken.None).Returns(Task.FromResult(true));
        await job.DeleteCandidateAsync("teacher-erp/referenced");
        await job.DeleteCandidateAsync("teacher-erp/orphan");

        await storage.Received(1).DeleteAsync("teacher-erp/orphan", CancellationToken.None);
        await storage.DidNotReceive().DeleteAsync("teacher-erp/referenced", CancellationToken.None);
        await storage.DidNotReceive().DeleteAsync("teacher-erp/recent", CancellationToken.None);
        await repository.Received(1).IsStoredFileReferencedAsync("teacher-erp/recent", CancellationToken.None);
    }
}
