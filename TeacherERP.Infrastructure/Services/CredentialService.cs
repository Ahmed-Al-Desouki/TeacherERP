using Microsoft.AspNetCore.Identity;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Infrastructure.Services
{
    public class CredentialService : ICredentialService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CredentialService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<ApplicationUser>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid)
                return Result<ApplicationUser>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));

            if (!user.EmailConfirmed)
            {
                return Result<ApplicationUser>.Failure(new Error("Auth.EmailNotVerified", "Please verify your email using the OTP sent to your inbox before logging in."));
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

            return Result<bool>.Success(true);
        }
    }
}