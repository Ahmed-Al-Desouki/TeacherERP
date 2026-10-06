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

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            await _userManager.AddToRoleAsync(user, role);
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
    }
}