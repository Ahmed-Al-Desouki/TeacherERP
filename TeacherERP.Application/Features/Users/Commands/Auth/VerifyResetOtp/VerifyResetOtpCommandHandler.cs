using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp
{
    public class VerifyResetOtpCommandHandler : IRequestHandler<VerifyResetOtpCommand, Result<ResetTokenDto>>
    {
        private readonly IOtpService _otpService;
        private readonly IUserAccountService _userAccountService;
        private readonly IResetGrantService _resetGrantService;

        public VerifyResetOtpCommandHandler(
            IOtpService otpService,
            IUserAccountService userAccountService,
            IResetGrantService resetGrantService)
        {
            _otpService = otpService;
            _userAccountService = userAccountService;
            _resetGrantService = resetGrantService;
        }

        public async Task<Result<ResetTokenDto>> Handle(VerifyResetOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من الكود (يشمل حد المحاولات والقفل)
            var otpResult = await _otpService.VerifyAsync(OtpPurpose.PasswordReset, request.Email, request.OtpCode, cancellationToken);
            if (otpResult.IsFailure)
            {
                return Result<ResetTokenDto>.Failure(otpResult.Error);
            }

            // 2. لا يُمنح تصريح إلا لحساب موجود فعلاً
            var userResult = await _userAccountService.FindByEmailAsync(request.Email, cancellationToken);
            if (userResult.Value is null)
            {
                return Result<ResetTokenDto>.Failure(new Error("Otp.Invalid", "The verification code is invalid."));
            }

            // 3. إصدار التصريح لمرة واحدة
            var grant = _resetGrantService.Issue(request.Email);
            return Result<ResetTokenDto>.Success(new ResetTokenDto(grant.Token, grant.ExpiresInSeconds));
        }
    }
}