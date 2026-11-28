using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.SetPassword;

public sealed class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, Result<bool>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly ICredentialService _credentialService;
    private readonly ISessionManager _sessionManager;
    private readonly IPasswordChangedNotifier _passwordChangedNotifier;

    public SetPasswordCommandHandler(
        ICurrentUserService currentUser,
        IUserRepository userRepository,
        ICredentialService credentialService,
        ISessionManager sessionManager,
        IPasswordChangedNotifier passwordChangedNotifier)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _credentialService = credentialService;
        _sessionManager = sessionManager;
        _passwordChangedNotifier = passwordChangedNotifier;
    }

    public async Task<Result<bool>> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId || _currentUser.SessionId is not { } sessionId)
        {
            return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure(new Error("User.NotFound", "User not found."));
        }

        var result = await _credentialService.SetPasswordAsync(user, request.NewPassword, cancellationToken);
        if (result.IsFailure)
        {
            return result;
        }

        await _sessionManager.RevokeOthersAsync(
            userId, sessionId, SessionRevokeReason.PasswordChanged, cancellationToken);
        _passwordChangedNotifier.Enqueue(user);

        return Result<bool>.Success(true);
    }
}
