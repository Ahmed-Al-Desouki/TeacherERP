using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly ICredentialService _credentialService;
        private readonly IAuthSessionService _authSessionService;

        public LoginCommandHandler(ICredentialService credentialService, IAuthSessionService authSessionService)
        {
            _credentialService = credentialService;
            _authSessionService = authSessionService;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من صحة بيانات الدخول واسترجاع المستخدم
            var userResult = await _credentialService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);
            if (userResult.IsFailure)
            {
                return Result<AuthResponseDto>.Failure(userResult.Error);
            }

            var user = userResult.Value!;

            // 2. التحقق من حالة المستخدم (Business Rule)
            if (user.Status != StudentStatus.Active)
            {
                return Result<AuthResponseDto>.Failure(new Error("Auth.AccountSuspended", "This account is suspended or blacklisted."));
            }

            // 3. إصدار الجلسة (التوكنز + حفظ الـ Refresh Token + تنبيه الدخول)
            var response = await _authSessionService.IssueAsync(user, request.DeviceInfo, cancellationToken);
            return Result<AuthResponseDto>.Success(response);
        }
    }
}