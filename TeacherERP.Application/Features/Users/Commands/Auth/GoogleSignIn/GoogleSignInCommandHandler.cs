using MediatR;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn
{
    public class GoogleSignInCommandHandler : IRequestHandler<GoogleSignInCommand, Result<AuthResponseDto>>
    {
        private const string GoogleProvider = "Google";
        private const string StudentRole = "Student";

        private readonly IGoogleAuthService _googleAuthService;
        private readonly IIdentityService _identityService;
        private readonly IAuthSessionService _authSessionService;

        public GoogleSignInCommandHandler(
            IGoogleAuthService googleAuthService,
            IIdentityService identityService,
            IAuthSessionService authSessionService)
        {
            _googleAuthService = googleAuthService;
            _identityService = identityService;
            _authSessionService = authSessionService;
        }

        public async Task<Result<AuthResponseDto>> Handle(GoogleSignInCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من التوكن عبر جوجل
            var googleResult = await _googleAuthService.ValidateIdTokenAsync(request.IdToken, cancellationToken);
            if (googleResult.IsFailure)
                return Result<AuthResponseDto>.Failure(googleResult.Error);

            var googleUser = googleResult.Value!;

            // 2. البحث بمعرّف جوجل الثابت (Subject)
            var linkedResult = await _identityService.FindByExternalLoginAsync(GoogleProvider, googleUser.Subject, cancellationToken);
            var user = linkedResult.Value;

            if (user is null)
            {
                // 3. البحث بالإيميل (حساب موجود بباسورد) وإلا إنشاء طالب جديد
                var emailResult = await _identityService.FindByEmailAsync(googleUser.Email, cancellationToken);
                user = emailResult.Value;

                if (user is null)
                {
                    user = ApplicationUser.CreateStudent(googleUser.FullName, googleUser.Email, string.Empty);
                    user.EmailConfirmed = true; // جوجل أثبتت ملكية الإيميل

                    var creationResult = await _identityService.CreateExternalUserAsync(user, StudentRole, cancellationToken);
                    if (creationResult.IsFailure)
                        return Result<AuthResponseDto>.Failure(creationResult.Error);
                }

                // 4. ربط حساب جوجل بالمستخدم
                var linkResult = await _identityService.LinkExternalLoginAsync(user, GoogleProvider, googleUser.Subject, cancellationToken);
                if (linkResult.IsFailure)
                    return Result<AuthResponseDto>.Failure(linkResult.Error);
            }

            // 5. نفس قاعدة الـ Login العادي
            if (user.Status != StudentStatus.Active)
            {
                return Result<AuthResponseDto>.Failure(new Error("Auth.AccountSuspended", "This account is suspended or blacklisted."));
            }

            // 6. إصدار الجلسة
            var response = await _authSessionService.IssueAsync(user, request.DeviceInfo, cancellationToken);
            return Result<AuthResponseDto>.Success(response);
        }
    }
}