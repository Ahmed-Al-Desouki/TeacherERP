using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    private readonly IUserAccountService _userAccountService;
    private readonly ICredentialService _credentialService;
    private readonly IResetGrantService _resetGrantService;
    private readonly ISessionManager _sessionManager;
    private readonly IPasswordChangedNotifier _passwordChangedNotifier;

    public ResetPasswordCommandHandler(
        IUserAccountService userAccountService,
        ICredentialService credentialService,
        IResetGrantService resetGrantService,
        ISessionManager sessionManager,
        IPasswordChangedNotifier passwordChangedNotifier)
    {
        _userAccountService = userAccountService;
        _credentialService = credentialService;
        _resetGrantService = resetGrantService;
        _sessionManager = sessionManager;
        _passwordChangedNotifier = passwordChangedNotifier;
    }

    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var invalidSession = new Error("Auth.InvalidResetToken", "The reset session is invalid or has expired. Please start again.");

        if (!await _resetGrantService.IsValidAsync(request.Email, request.ResetToken, cancellationToken))
            return Result<bool>.Failure(invalidSession);

        var userResult = await _userAccountService.FindByEmailAsync(request.Email, cancellationToken);
        var user = userResult.Value;
        if (user is null)
            return Result<bool>.Failure(invalidSession);

        var resetResult = await _credentialService.ResetPasswordAsync(user, request.NewPassword, cancellationToken);
        if (resetResult.IsFailure)
            return Result<bool>.Failure(resetResult.Error);

        await _sessionManager.RevokeAllForUserAsync(user.Id, SessionRevokeReason.PasswordReset, cancellationToken);
        await _resetGrantService.RevokeAsync(request.Email, cancellationToken);
        _passwordChangedNotifier.Enqueue(user);

        return Result<bool>.Success(true);
    }
}
