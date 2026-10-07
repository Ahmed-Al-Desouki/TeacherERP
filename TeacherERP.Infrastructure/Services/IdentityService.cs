using Microsoft.AspNetCore.Identity;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Result<bool>.Success(user != null);
        }

        public async Task<Result<Guid>> CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return Result<Guid>.Failure(new Error("User.CreationFail", result.Errors.First().Description));
            }

            await AssignRoleAsync(user, role);
            return Result<Guid>.Success(user.Id);
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

        public async Task<Result<ApplicationUser?>> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Result<ApplicationUser?>.Success(user);
        }

        public async Task<Result<ApplicationUser?>> FindByExternalLoginAsync(string provider, string providerKey, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByLoginAsync(provider, providerKey);
            return Result<ApplicationUser?>.Success(user);
        }

        public async Task<Result<Guid>> CreateExternalUserAsync(ApplicationUser user, string role, CancellationToken cancellationToken)
        {
            // إنشاء بدون باسورد: الدخول يتم عبر مزود خارجي (Google)
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return Result<Guid>.Failure(new Error("User.CreationFail", result.Errors.First().Description));
            }

            await AssignRoleAsync(user, role);
            return Result<Guid>.Success(user.Id);
        }

        public async Task<Result<bool>> LinkExternalLoginAsync(ApplicationUser user, string provider, string providerKey, CancellationToken cancellationToken)
        {
            var result = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
            if (!result.Succeeded)
            {
                return Result<bool>.Failure(new Error("User.ExternalLoginLinkFail", result.Errors.First().Description));
            }

            return Result<bool>.Success(true);
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

        private async Task AssignRoleAsync(ApplicationUser user, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            await _userManager.AddToRoleAsync(user, role);
        }
    }
}