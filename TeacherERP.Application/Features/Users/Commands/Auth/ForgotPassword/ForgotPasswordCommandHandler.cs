using Hangfire;
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IOtpService _otpService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ForgotPasswordCommandHandler(
            IIdentityService identityService,
            IOtpService otpService,
            IBackgroundJobClient backgroundJobClient)
        {
            _identityService = identityService;
            _otpService = otpService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<ForgotPasswordResponseDto>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. نولّد الكود لأي إيميل (موجود أو لا) حتى يكون السلوك الظاهر للمهاجم متطابقاً
            var otpResult = await _otpService.GenerateAsync(OtpPurpose.PasswordReset, request.Email, cancellationToken);
            if (otpResult.IsFailure)
            {
                return Result<ForgotPasswordResponseDto>.Failure(otpResult.Error);
            }

            var otp = otpResult.Value!;

            // 2. الإيميل يُرسل فعلياً فقط إذا كان الحساب موجوداً
            var userResult = await _identityService.FindByEmailAsync(request.Email, cancellationToken);
            var user = userResult.Value;

            if (user is not null)
            {
                var toEmail = user.Email!;
                var otpCode = otp.Code;
                _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                    emailService.SendPasswordResetOtpAsync(toEmail, otpCode));
            }

            // 3. نفس الرد دائماً
            return Result<ForgotPasswordResponseDto>.Success(
                new ForgotPasswordResponseDto(otp.ExpiresInSeconds, otp.ResendAvailableInSeconds));
        }
    }
}