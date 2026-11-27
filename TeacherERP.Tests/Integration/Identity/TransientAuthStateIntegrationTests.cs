using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Repositories;
using TeacherERP.Infrastructure.Services;
using TeacherERP.Infrastructure.Settings;

namespace TeacherERP.Tests.Integration.Identity;

public sealed class TransientAuthStateIntegrationTests
{
    private static readonly DateTime Now = new(2030, 4, 5, 6, 7, 8, DateTimeKind.Utc);
    private const string JwtSecret = "long-test-secret-for-hmac-key-1234567890";

    [Fact]
    public async Task Otp_IsHashed_PersistsAcrossContexts_AndCanOnlyBeUsedOnce()
    {
        await using var connection = await CreateDatabaseAsync();
        var options = OptionsFor(connection);
        var time = new FrozenTimeProvider(Now);
        string code;
        await using (var firstContext = new AppDbContext(options))
        {
            var service = CreateOtpService(firstContext, time);
            var issued = await service.GenerateAsync(OtpPurpose.PasswordReset, "student@example.test", CancellationToken.None);
            Assert.True(issued.IsSuccess);
            code = issued.Value!.Code;
            Assert.Equal("Otp.ResendTooSoon", (await service.GenerateAsync(OtpPurpose.PasswordReset, "student@example.test", CancellationToken.None)).Error.Code);
            var hash = await firstContext.TransientAuthStates.Select(x => x.OtpHash).SingleAsync();
            Assert.NotEqual(code, hash);
        }

        await using var restartedContext = new AppDbContext(options);
        var restartedService = CreateOtpService(restartedContext, time);
        Assert.True((await restartedService.VerifyAsync(OtpPurpose.PasswordReset, "student@example.test", code, CancellationToken.None)).IsSuccess);
        Assert.False((await restartedService.VerifyAsync(OtpPurpose.PasswordReset, "student@example.test", code, CancellationToken.None)).IsSuccess);
    }

    [Fact]
    public async Task Otp_PersistsAttemptLockAndHourlyIssueLimit()
    {
        await using var connection = await CreateDatabaseAsync();
        var options = OptionsFor(connection);
        await using var context = new AppDbContext(options);
        var time = new FrozenTimeProvider(Now);
        var service = CreateOtpService(context, time);
        var issued = await service.GenerateAsync(OtpPurpose.Activation, "teacher@example.test", CancellationToken.None);
        Assert.True(issued.IsSuccess);
        var wrongCode = issued.Value!.Code == "000000" ? "000001" : "000000";

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            var result = await service.VerifyAsync(OtpPurpose.Activation, "teacher@example.test", wrongCode, CancellationToken.None);
            Assert.Equal(attempt == 5 ? "Otp.TooManyAttempts" : "Otp.Invalid", result.Error.Code);
        }
        Assert.Equal("Otp.TooManyAttempts", (await service.VerifyAsync(OtpPurpose.Activation, "teacher@example.test", issued.Value!.Code, CancellationToken.None)).Error.Code);

        // انتهاء قفل الكود بعد 10 دقائق لا يلغي حد الإصدارات المحفوظ لمدة ساعة.
        time.Advance(TimeSpan.FromMinutes(11));
        Assert.True((await service.GenerateAsync(OtpPurpose.Activation, "teacher@example.test", CancellationToken.None)).IsSuccess);
        time.Advance(TimeSpan.FromMinutes(1));
        Assert.True((await service.GenerateAsync(OtpPurpose.Activation, "teacher@example.test", CancellationToken.None)).IsSuccess);
        time.Advance(TimeSpan.FromMinutes(1));
        Assert.Equal("Otp.HourlyLimit", (await service.GenerateAsync(OtpPurpose.Activation, "teacher@example.test", CancellationToken.None)).Error.Code);
    }

    [Fact]
    public async Task RegistrationDraft_PersistsPasswordHashOnly_AndCanBeRemoved()
    {
        await using var connection = await CreateDatabaseAsync();
        var options = OptionsFor(connection);
        await using var context = new AppDbContext(options);
        var store = new TransientAuthStateStore(context, new FrozenTimeProvider(Now));
        const string rawPassword = "NeverPersistThisPassword123";
        const string passwordHash = "identity-password-hash";
        var draft = new StudentDraft("Test Student", "student@example.test", passwordHash);

        await store.SavePayloadAsync(OtpPurpose.Registration.ToString(), draft.Email, draft, TimeSpan.FromMinutes(10), CancellationToken.None);

        var json = await context.TransientAuthStates.Select(x => x.PayloadJson).SingleAsync();
        Assert.DoesNotContain(rawPassword, json);
        Assert.Contains(passwordHash, json);
        Assert.Equal(draft, await store.GetPayloadAsync<StudentDraft>(OtpPurpose.Registration.ToString(), draft.Email, CancellationToken.None));
        await store.RemovePayloadAsync(OtpPurpose.Registration.ToString(), draft.Email, CancellationToken.None);
        Assert.Null(await store.GetPayloadAsync<StudentDraft>(OtpPurpose.Registration.ToString(), draft.Email, CancellationToken.None));
    }

    [Fact]
    public async Task ResetGrant_IsPersistedAsHash_AndCanBeRevoked()
    {
        await using var connection = await CreateDatabaseAsync();
        var options = OptionsFor(connection);
        await using var context = new AppDbContext(options);
        var store = new TransientAuthStateStore(context, new FrozenTimeProvider(Now));
        var service = new ResetGrantService(store, new FrozenTimeProvider(Now));

        var grant = await service.IssueAsync("student@example.test", CancellationToken.None);

        Assert.True(await service.IsValidAsync("student@example.test", grant.Token, CancellationToken.None));
        var storedHash = await context.TransientAuthStates.Select(x => x.GrantHash).SingleAsync();
        Assert.NotEqual(grant.Token, storedHash);
        await service.RevokeAsync("student@example.test", CancellationToken.None);
        Assert.False(await service.IsValidAsync("student@example.test", grant.Token, CancellationToken.None));
    }

    [Fact]
    public async Task Cleanup_RemovesOnlyOneBoundedBatchAndPreservesAnyStateWithAnActivePayload()
    {
        await using var connection = await CreateDatabaseAsync();
        var options = OptionsFor(connection);
        await using (var seedContext = new AppDbContext(options))
        {
            for (var index = 0; index < 251; index++)
            {
                seedContext.TransientAuthStates.Add(new TransientAuthState
                {
                    Purpose = "expired",
                    Email = $"expired{index}@example.test"
                });
            }
            seedContext.TransientAuthStates.Add(new TransientAuthState
            {
                Purpose = "active",
                Email = "active@example.test",
                PayloadJson = "{}",
                PayloadExpiresAtUtc = Now.AddMinutes(5)
            });
            await seedContext.SaveChangesAsync();
        }

        await using var context = new AppDbContext(options);
        var store = new TransientAuthStateStore(context, new FrozenTimeProvider(Now));

        Assert.Equal(250, await store.DeleteExpiredAsync(Now, 250, CancellationToken.None));
        Assert.Equal(2, await context.TransientAuthStates.CountAsync());
        Assert.Equal(1, await store.DeleteExpiredAsync(Now, 250, CancellationToken.None));
        Assert.Equal("active@example.test", await context.TransientAuthStates.Select(x => x.Email).SingleAsync());
    }

    private static async Task<SqliteConnection> CreateDatabaseAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = new AppDbContext(OptionsFor(connection));
        await context.Database.EnsureCreatedAsync();
        return connection;
    }

    private static DbContextOptions<AppDbContext> OptionsFor(SqliteConnection connection)
        => new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;

    private static OtpService CreateOtpService(AppDbContext context, TimeProvider timeProvider)
        => new(context, NullLogger<OtpService>.Instance, timeProvider,
            Options.Create(new JwtSettings { Secret = JwtSecret }));

    private sealed record StudentDraft(string FullName, string Email, string PasswordHash);

    private sealed class FrozenTimeProvider(DateTime now) : TimeProvider
    {
        private DateTimeOffset _now = new(DateTime.SpecifyKind(now, DateTimeKind.Utc));
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan duration) => _now = _now.Add(duration);
    }
}
