using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Repositories;
using TeacherERP.Tests.Integration;

namespace TeacherERP.Tests.Integration.DataRetention;

public sealed class RetentionCleanupIntegrationTests
{
    [Fact]
    public async Task Cleanup_IsBoundedAndPreservesUnreadNotificationsAndPendingTeacherRequests()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using (var setup = new AppDbContext(options))
            await setup.Database.EnsureCreatedAsync();

        await using (var seed = new AppDbContext(options))
        {
            var teacher = await TestData.CreateTeacherAsync(seed);
            for (var index = 0; index < 3; index++)
            {
                var read = Notification.Create(teacher.OwnerId, teacher.TenantId, NotificationType.SessionCancelled, "test", null);
                read.MarkRead();
                seed.Notifications.Add(read);
            }
            seed.Notifications.Add(Notification.Create(teacher.OwnerId, teacher.TenantId, NotificationType.SessionCancelled, "unread", null));

            var closed = TeacherAccessRequest.Create("Closed", "closed@example.test", "Math", null, "UTC", "hash-closed", DateTime.UtcNow.AddDays(-400));
            closed.Reject(Guid.NewGuid(), DateTime.UtcNow.AddDays(-370), "Not approved");
            seed.TeacherAccessRequests.Add(closed);
            seed.TeacherAccessRequests.Add(TeacherAccessRequest.Create(
                "Expired", "expired@example.test", "Math", null, "UTC", "hash-expired", DateTime.UtcNow.AddDays(-400)));
            seed.TeacherAccessRequests.Add(TeacherAccessRequest.Create(
                "Pending", "pending@example.test", "Math", null, "UTC", "hash-pending", DateTime.UtcNow.AddDays(5)));
            await seed.SaveChangesAsync();
        }

        await using var context = new AppDbContext(options);
        var repository = new RetentionCleanupRepository(context);
        var futureCutoff = DateTime.UtcNow.AddDays(1);

        Assert.Equal(2, await repository.PurgeReadNotificationsBeforeAsync(futureCutoff, 2, CancellationToken.None));
        Assert.Equal(2, await repository.PurgeClosedTeacherRequestsBeforeAsync(futureCutoff, 2, CancellationToken.None));
        Assert.Equal(2, await context.Notifications.IgnoreQueryFilters().CountAsync());
        Assert.Equal("pending@example.test", await context.TeacherAccessRequests.IgnoreQueryFilters().Select(x => x.Email).SingleAsync());
    }
}
