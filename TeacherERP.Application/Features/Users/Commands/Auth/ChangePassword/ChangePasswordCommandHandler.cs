// جديد — TeacherERP.Application/Features/Users/Commands/Auth/ChangePassword/ChangePasswordCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly ICredentialService _credentialService;
        private readonly ISessionManager _sessionManager;

        public ChangePasswordCommandHandler(
            ICurrentUserService currentUser,
            IUserRepository userRepository,
            ICredentialService credentialService,
            ISessionManager sessionManager)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
            _credentialService = credentialService;
            _sessionManager = sessionManager;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
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

            var result = await _credentialService.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword, cancellationToken);
            if (result.IsFailure)
            {
                return result;
            }

            // الأجهزة الأخرى تخرج فوراً (الباسورد القديم ممكن يكون اتسرّب)، والجهاز الحالي يكمل
            await _sessionManager.RevokeOthersAsync(userId, sessionId, SessionRevokeReason.PasswordChanged, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}