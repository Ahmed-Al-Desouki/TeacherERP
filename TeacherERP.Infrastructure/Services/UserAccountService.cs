using Microsoft.AspNetCore.Identity;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Infrastructure.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserAccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Result<bool>.Success(user != null);
        }

        public async Task<Result<ApplicationUser?>> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Result<ApplicationUser?>.Success(user);
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

        public async Task<Result<Guid>> CreateExternalUserAsync(ApplicationUser user, string role, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return Result<Guid>.Failure(new Error("User.CreationFail", result.Errors.First().Description));
            }

            await AssignRoleAsync(user, role);
            return Result<Guid>.Success(user.Id);
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