using Hangfire;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Sessions;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Unit.Sessions;

public sealed class RefreshTokenRotationTests
{
    [Fact]
    public async Task RefreshAsync_WhenAtomicRotationLosesRace_ReturnsInvalidRefreshToken()
    {
        var (manager, repository, session, user) = CreateManager(rotationApplied: false);

        var result = await manager.RefreshAsync($"{session.Id:N}.known-secret", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Auth.InvalidRefreshToken", result.Error.Code);
        await repository.Received(1).TryRotateRefreshTokenAsync(
            Arg.Any<LoginSession>(),
            TokenHasher.Hash("known-secret"),
            Arg.Any<DateTime>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshAsync_WhenAtomicRotationWins_ReturnsTheNewRefreshToken()
    {
        var (manager, repository, session, _) = CreateManager(rotationApplied: true);

        var result = await manager.RefreshAsync($"{session.Id:N}.known-secret", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal($"{session.Id:N}.rotated-secret", result.Value!.RefreshToken);
        Assert.Equal("UTC", result.Value.TimeZoneId);
        await repository.Received(1).TryRotateRefreshTokenAsync(
            Arg.Any<LoginSession>(),
            TokenHasher.Hash("known-secret"),
            Arg.Any<DateTime>(),
            Arg.Any<CancellationToken>());
    }

    private static (SessionManager Manager, ISessionRepository Repository, LoginSession Session, ApplicationUser User)
        CreateManager(bool rotationApplied)
    {
        var user = ApplicationUser.CreateStudent("Refresh test", $"refresh-{Guid.NewGuid():N}@example.test");
        var session = LoginSession.Create(
            user.Id,
            "refresh-test-device",
            "Refresh test device",
            TokenHasher.Hash("known-secret"),
            DateTime.UtcNow.AddMinutes(-1),
            SessionPolicy.RefreshLifetime,
            SessionPolicy.AbsoluteLifetime);

        typeof(LoginSession).GetProperty(nameof(LoginSession.User))!.SetValue(session, user);

        var repository = Substitute.For<ISessionRepository>();
        repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<LoginSession?>(session));
        repository.TryRotateRefreshTokenAsync(
                Arg.Any<LoginSession>(),
                Arg.Any<string>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(rotationApplied));

        var jwtProvider = Substitute.For<IJwtProvider>();
        jwtProvider.GenerateRefreshToken().Returns("rotated-secret");
        jwtProvider.GenerateAccessTokenAsync(user, session.Id)
            .Returns(Task.FromResult(new AccessTokenResult("access-token", Array.Empty<string>())));

        var manager = new SessionManager(
            jwtProvider,
            repository,
            Substitute.For<ISessionValidator>(),
            Substitute.For<IUnitOfWork>(),
            Substitute.For<IBackgroundJobClient>(),
            Substitute.For<IUserTimeZoneAccessor>(),
            TimeProvider.System,
            NullLogger<SessionManager>.Instance);

        return (manager, repository, session, user);
    }
}
