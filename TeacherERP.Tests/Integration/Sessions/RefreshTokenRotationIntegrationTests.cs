using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Sessions;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration.Sessions;

public sealed class RefreshTokenRotationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public RefreshTokenRotationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TryRotateRefreshToken_OnlyTheFirstUpdateForCurrentHashSucceeds()
    {
        var user = ApplicationUser.CreateStudent("Rotation race", $"rotation-{Guid.NewGuid():N}@example.test");
        const string oldSecret = "initial-refresh-secret";
        var oldHash = TokenHasher.Hash(oldSecret);
        var now = DateTime.UtcNow;
        var session = LoginSession.Create(
            user.Id,
            "rotation-test-device",
            "Rotation test device",
            oldHash,
            now,
            SessionPolicy.RefreshLifetime,
            SessionPolicy.AbsoluteLifetime);

        await _factory.SeedAsync(async db =>
        {
            db.Users.Add(user);
            db.LoginSessions.Add(session);
            await db.SaveChangesAsync();
        });

        using var firstScope = _factory.Services.CreateScope();
        using var secondScope = _factory.Services.CreateScope();
        var firstRepository = firstScope.ServiceProvider.GetRequiredService<ISessionRepository>();
        var secondRepository = secondScope.ServiceProvider.GetRequiredService<ISessionRepository>();
        var firstSnapshot = await firstRepository.GetByIdAsync(session.Id, CancellationToken.None);
        var secondSnapshot = await secondRepository.GetByIdAsync(session.Id, CancellationToken.None);
        Assert.NotNull(firstSnapshot);
        Assert.NotNull(secondSnapshot);

        firstSnapshot.Rotate(TokenHasher.Hash("first-winner-secret"), now.AddSeconds(1), SessionPolicy.RefreshLifetime);
        secondSnapshot.Rotate(TokenHasher.Hash("stale-loser-secret"), now.AddSeconds(1), SessionPolicy.RefreshLifetime);

        var firstUpdate = await firstRepository.TryRotateRefreshTokenAsync(
            firstSnapshot, oldHash, now.AddSeconds(1), CancellationToken.None);
        var staleUpdate = await secondRepository.TryRotateRefreshTokenAsync(
            secondSnapshot, oldHash, now.AddSeconds(1), CancellationToken.None);

        Assert.True(firstUpdate);
        Assert.False(staleUpdate);

        var persisted = await _factory.QueryAsync(db => db.LoginSessions
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleAsync(item => item.Id == session.Id));
        Assert.Equal(TokenHasher.Hash("first-winner-secret"), persisted.RefreshTokenHash);
        Assert.Equal(oldHash, persisted.PreviousRefreshTokenHash);
    }
}
