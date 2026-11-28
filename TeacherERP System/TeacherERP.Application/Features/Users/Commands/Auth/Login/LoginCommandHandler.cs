// موجود (تعديل) — TeacherERP.Application/Features/Users/Commands/Auth/Login/LoginCommandHandler.cs
using Hangfire;
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private const string ResendTooSoonCode = "Otp.ResendTooSoon";

        private readonly ICredentialService _credentialService;
        private readonly ISessionManager _sessionManager;
        private readonly IOtpService _otpService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public LoginCommandHandler(
            ICredentialService credentialService,
            ISessionManager sessionManager,
            IOtpService otpService,
            IBackgroundJobClient backgroundJobClient)
        {
            _credentialService = credentialService;
            _sessionManager = sessionManager;
            _otpService = otpService;
            _backgroundJobClient = backgroundJobClient;
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

            // 3. حساب لم يُفعَّل بعد (مساعد أنشأه المدرس): لا توكنز قبل إثبات ملكية الإيميل بالـ OTP
            // الباسورد اتفحص فوق، فلا يمكن إغراق صندوق أي حد بأكواد من غير معرفة الباسورد المؤقت
            if (!user.EmailConfirmed)
            {
                return await RequireActivationAsync(user, cancellationToken);
            }

            // 4. إصدار الجلسة (التوكنز + حفظ الجلسة + تنبيه الدخول)
            var response = await _sessionManager.IssueAsync(user, request.DeviceId, request.DeviceInfo, cancellationToken);
            return Result<AuthResponseDto>.Success(response);
        }

        private async Task<Result<AuthResponseDto>> RequireActivationAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var otpResult = await _otpService.GenerateAsync(OtpPurpose.Activation, user.Email!, cancellationToken);

            if (otpResult.IsFailure)
            {
                // كود اتبعت من أقل من دقيقة: التطبيق يفتح شاشة الـ OTP بدل ما يعرض خطأ
                if (otpResult.Error.Code == ResendTooSoonCode)
                {
                    return Result<AuthResponseDto>.Failure(new Error(
                        "Auth.ActivationRequired",
                        "Your account is not activated yet. A verification code was already sent to your email."));
                }

                return Result<AuthResponseDto>.Failure(otpResult.Error);
            }

            var toEmail = user.Email!;
            var otpCode = otpResult.Value!.Code;
            _backgroundJobClient.Enqueue<IEmailService>(emailService => emailService.SendOtpEmailAsync(toEmail, otpCode));

            return Result<AuthResponseDto>.Failure(new Error(
                "Auth.ActivationRequired",
                "Your account is not activated yet. A verification code was sent to your email."));
        }
    }
}