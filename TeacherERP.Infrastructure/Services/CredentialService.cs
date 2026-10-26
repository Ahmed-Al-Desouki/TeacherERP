// موجود (تعديل) — TeacherERP.Infrastructure/Services/CredentialService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Infrastructure.Services
{
    public class CredentialService : ICredentialService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CredentialService> _logger;

        public CredentialService(UserManager<ApplicationUser> userManager, ILogger<CredentialService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // القفل والباسورد فقط. تفعيل الإيميل (حساب المساعد الجديد) يتعامل معه LoginCommandHandler بعد نجاح الباسورد
        public async Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<ApplicationUser>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));

            // الحساب المقفول لا يُفحص له باسورد أصلاً (حتى لا يُستخدم القفل لتخمين الباسورد)
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Login rejected because the account is locked out. UserId {UserId}", user.Id);
                return Result<ApplicationUser>.Failure(new Error("Auth.AccountLocked", "Too many failed attempts. Please try again later."));
            }

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid)
            {
                // يعدّ المحاولة الفاشلة، ويقفل الحساب عند الوصول للحد (5 محاولات / 15 دقيقة)
                await _userManager.AccessFailedAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Account locked after repeated failed login attempts. UserId {UserId}", user.Id);
                }

                return Result<ApplicationUser>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));
            }

            // تصفير العداد بعد الدخول الصحيح (بدون كتابة في الداتابيز لو مفيش محاولات فاشلة)
            if (user.AccessFailedCount > 0)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            return Result<ApplicationUser>.Success(user);
        }

        public async Task<Result<bool>> ResetPasswordAsync(ApplicationUser user, string newPassword, CancellationToken cancellationToken)
        {
            // التوكن يُولَّد ويُستهلك داخل نفس الطلب، فلا يتأثر بإعادة تشغيل التطبيق
            // وتعمل أيضاً للحسابات التي لا تملك باسورد (القادمة من Google)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return Result<bool>.Failure(new Error("Auth.PasswordResetFail", errors));
            }

            // إثبات ملكية الإيميل (OTP) يفك القفل، وإلا يقدر أي حد يعطّل حساب غيره بمحاولات غلط
            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            // نفس حماية الدخول: توكن مسروق لا يُستخدم لتخمين الباسورد الحالي
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Password change rejected because the account is locked out. UserId {UserId}", user.Id);
                return Result<bool>.Failure(new Error("Auth.AccountLocked", "Too many failed attempts. Please try again later."));
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordMismatch)))
                {
                    await _userManager.AccessFailedAsync(user);
                    return Result<bool>.Failure(new Error("Auth.InvalidCurrentPassword", "The current password is incorrect."));
                }

                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return Result<bool>.Failure(new Error("Auth.PasswordChangeFail", errors));
            }

            if (user.AccessFailedCount > 0)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            return Result<bool>.Success(true);
        }
    }
}