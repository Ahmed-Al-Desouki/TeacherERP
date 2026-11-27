using NSubstitute;
using TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword;
using TeacherERP.Application.Features.Users.Commands.Auth.SetPassword;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Unit;

public sealed class PasswordCommandTests
{
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly ICredentialService _credentials = Substitute.For<ICredentialService>();
    private readonly ISessionManager _sessions = Substitute.For<ISessionManager>();
    private readonly IPasswordChangedNotifier _passwordChangedNotifier = Substitute.For<IPasswordChangedNotifier>();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _sessionId = Guid.NewGuid();
    private readonly ApplicationUser _user = ApplicationUser.CreateStudent("Google User", "google@example.test");

    [Fact]
    public async Task SetPassword_RequiresAuthenticatedUserAndSession()
    {
        _currentUser.UserId.Returns(_userId);

        var result = await CreateSetPasswordHandler().Handle(new SetPasswordCommand("Google1"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Auth.Unauthorized", result.Error.Code);
        await _users.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
    }

    [Fact]
    public async Task SetPassword_WhenUserHasGoogleAccount_SetsPasswordAndRevokesOtherSessions()
    {
        ConfigureAuthenticatedUser();
        _users.GetByIdAsync(_userId, Arg.Any<CancellationToken>()).Returns(_user);
        _credentials.SetPasswordAsync(_user, "Google1", Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(true));

        var result = await CreateSetPasswordHandler().Handle(new SetPasswordCommand("Google1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _credentials.Received(1).SetPasswordAsync(_user, "Google1", Arg.Any<CancellationToken>());
        await _sessions.Received(1).RevokeOthersAsync(
            _userId, _sessionId, SessionRevokeReason.PasswordChanged, Arg.Any<CancellationToken>());
        _passwordChangedNotifier.Received(1).Enqueue(_user);
    }

    [Fact]
    public async Task SetPassword_WhenCredentialServiceRejects_DoesNotRevokeSessions()
    {
        ConfigureAuthenticatedUser();
        _users.GetByIdAsync(_userId, Arg.Any<CancellationToken>()).Returns(_user);
        _credentials.SetPasswordAsync(_user, "Google1", Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Failure(new Error("Auth.PasswordAlreadySet", "already set")));

        var result = await CreateSetPasswordHandler().Handle(new SetPasswordCommand("Google1"), CancellationToken.None);

        Assert.Equal("Auth.PasswordAlreadySet", result.Error.Code);
        await _sessions.DidNotReceiveWithAnyArgs().RevokeOthersAsync(default, default, default, default);
        _passwordChangedNotifier.DidNotReceiveWithAnyArgs().Enqueue(default!);
    }

    [Fact]
    public async Task ChangePassword_UsesCurrentPasswordAndRevokesOtherSessions()
    {
        ConfigureAuthenticatedUser();
        _users.GetByIdAsync(_userId, Arg.Any<CancellationToken>()).Returns(_user);
        _credentials.ChangePasswordAsync(_user, "Google1", "Local2", Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(true));

        var handler = new ChangePasswordCommandHandler(_currentUser, _users, _credentials, _sessions, _passwordChangedNotifier);
        var result = await handler.Handle(new ChangePasswordCommand("Google1", "Local2"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _credentials.Received(1).ChangePasswordAsync(
            _user, "Google1", "Local2", Arg.Any<CancellationToken>());
        await _sessions.Received(1).RevokeOthersAsync(
            _userId, _sessionId, SessionRevokeReason.PasswordChanged, Arg.Any<CancellationToken>());
        _passwordChangedNotifier.Received(1).Enqueue(_user);
    }

    [Fact]
    public async Task ChangePassword_WhenCredentialServiceRejects_DoesNotQueueNotification()
    {
        ConfigureAuthenticatedUser();
        _users.GetByIdAsync(_userId, Arg.Any<CancellationToken>()).Returns(_user);
        _credentials.ChangePasswordAsync(_user, "Google1", "Local2", Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Failure(new Error("Auth.InvalidCurrentPassword", "invalid")));

        var handler = new ChangePasswordCommandHandler(_currentUser, _users, _credentials, _sessions, _passwordChangedNotifier);
        var result = await handler.Handle(new ChangePasswordCommand("Google1", "Local2"), CancellationToken.None);

        Assert.Equal("Auth.InvalidCurrentPassword", result.Error.Code);
        _passwordChangedNotifier.DidNotReceiveWithAnyArgs().Enqueue(default!);
        await _sessions.DidNotReceiveWithAnyArgs().RevokeOthersAsync(default, default, default, default);
    }

    [Fact]
    public void PasswordValidators_EnforceIdentityPasswordRequirements()
    {
        var setValidator = new SetPasswordValidator();
        var changeValidator = new ChangePasswordValidator();

        Assert.False(setValidator.Validate(new SetPasswordCommand("short")).IsValid);
        Assert.False(setValidator.Validate(new SetPasswordCommand("lettersOnly")).IsValid);
        Assert.True(setValidator.Validate(new SetPasswordCommand("Google1")).IsValid);
        Assert.False(changeValidator.Validate(new ChangePasswordCommand("Google1", "Google1")).IsValid);
        Assert.True(changeValidator.Validate(new ChangePasswordCommand("Google1", "Local2")).IsValid);
    }

    private void ConfigureAuthenticatedUser()
    {
        _currentUser.UserId.Returns(_userId);
        _currentUser.SessionId.Returns(_sessionId);
    }

    private SetPasswordCommandHandler CreateSetPasswordHandler()
        => new(_currentUser, _users, _credentials, _sessions, _passwordChangedNotifier);
}
