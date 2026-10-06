using MediatR;
using Microsoft.AspNetCore.Identity;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<bool>>
    {
        private readonly IOtpService _otpService;
        private readonly UserManager<ApplicationUser> _userManager; // أضفنا UserManager هنا

        public VerifyOtpCommandHandler(IOtpService otpService, UserManager<ApplicationUser> userManager)
        {
            _otpService = otpService;
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من صحة وصلاحية الـ OTP
            var isValid = await _otpService.VerifyOtpAsync(request.StudentId, request.OtpCode, cancellationToken);

            if (!isValid)
            {
                return Result<bool>.Failure(new Error("Otp.Invalid", "The verification code is invalid or has expired."));
            }

            // 2. جلب المستخدم بواسطة الـ Id
            var user = await _userManager.FindByIdAsync(request.StudentId.ToString());
            if (user == null)
            {
                return Result<bool>.Failure(new Error("User.NotFound", "User not found."));
            }

            // 3. تغيير حالة الإيميل إلى مؤكد (True)
            user.EmailConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Result<bool>.Failure(new Error("User.UpdateFailed", "Failed to confirm email address."));
            }

            return Result<bool>.Success(true);
        }
    }
}