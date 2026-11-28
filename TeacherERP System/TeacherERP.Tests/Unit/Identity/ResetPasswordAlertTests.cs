using NSubstitute;
using TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Unit.Identity;

public sealed class ResetPasswordAlertTests
{
    private readonly IUserAccountService _users = Substitute.For<IUserAccountService>();
    private readonly ICredentialService _credentials = Substitute.For<ICredentialService>();
    private readonly IResetGrantService _grants = Substitute.For<IResetGrantService>();
    private readonly ISessionManager _sessions = Substitute.For<ISessionManager>();
    private readonly IPasswordChangedNotifier _notifier = Substitute.For<IPasswordChangedNotifier>();
    private readonly ApplicationUser _user = ApplicationUser.CreateStudent("Reset test", "reset@example.test");

    private ResetPasswordCommandHandler CreateHandler()
        => new(_users, _credentials, _grants, _sessions, _notifier);

    private void ConfigureValidReset()
    {
        _grants.IsValidAsync("reset@example.test", "reset-token", Arg.Any<CancellationToken>()).Returns(true);
        _users.FindByEmailAsync("reset@example.test", Arg.Any<CancellationToken>())
            .Returns(Result<ApplicationUser?>.Success(_user));
        _credentials.ResetPasswordAsync(_user, "NewPass1", Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(true));
    }

    [Fact]
    public async Task Handle_WhenResetSucceeds_RevokesSessionsAndQueuesAlert()
    {
        ConfigureValidReset();

        var result = await CreateHandler().Handle(
            new ResetPasswordCommand("reset@example.test", "reset-token", "NewPass1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _sessions.Received(1).RevokeAllForUserAsync(_user.Id, SessionRevokeReason.PasswordReset, Arg.Any<CancellationToken>());
        await _grants.Received(1).RevokeAsync("reset@example.test", Arg.Any<CancellationToken>());
        _notifier.Received(1).Enqueue(_user);
    }

    [Fact]
    public async Task Handle_WhenPasswordResetFails_DoesNotRevokeGrantOrQueueAlert()
    {
        ConfigureValidReset();
        _credentials.ResetPasswordAsync(_user, "NewPass1", Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Failure(new Error("Auth.PasswordResetFail", "failed")));

        var result = await CreateHandler().Handle(
            new ResetPasswordCommand("reset@example.test", "reset-token", "NewPass1"), CancellationToken.None);

        Assert.Equal("Auth.PasswordResetFail", result.Error.Code);
        await _grants.DidNotReceive().RevokeAsync("reset@example.test", Arg.Any<CancellationToken>());
        _notifier.DidNotReceiveWithAnyArgs().Enqueue(default!);
    }
}
