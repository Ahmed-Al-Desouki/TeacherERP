using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using TeacherERP.Application.Interfaces;
using TeacherERP.Infrastructure.Services;

namespace TeacherERP.Tests.Unit.Common;

public sealed class OtpServiceTimeProviderTests
{
    [Fact]
    public async Task GenerateAsync_UsesInjectedTimeForResendCooldown()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OtpService(
            cache,
            NullLogger<OtpService>.Instance,
            new FixedTimeProvider(new DateTime(2030, 4, 5, 6, 7, 8, DateTimeKind.Utc)));

        var first = await service.GenerateAsync(OtpPurpose.PasswordReset, "student@example.test", CancellationToken.None);
        var second = await service.GenerateAsync(OtpPurpose.PasswordReset, "student@example.test", CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsFailure);
        Assert.Equal("Otp.ResendTooSoon", second.Error.Code);
    }
}
