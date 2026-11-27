// موجود (تعديل) — TeacherERP.Infrastructure/Services/UserAccountService.cs
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
            if (IsAdministratorRole(role))
            {
                return Result<Guid>.Failure(AdministratorRoleNotAllowed);
            }

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return Result<Guid>.Failure(new Error("User.CreationFail", result.Errors.First().Description));
            }

            await AssignRoleAsync(user, role);
            return Result<Guid>.Success(user.Id);
        }

        public async Task<Result<string>> HashPasswordAsync(ApplicationUser user, string password, CancellationToken cancellationToken)
        {
            foreach (var validator in _userManager.PasswordValidators)
            {
                var result = await validator.ValidateAsync(_userManager, user, password);
                if (!result.Succeeded)
                    return Result<string>.Failure(new Error("User.CreationFail", result.Errors.First().Description));
            }
            return Result<string>.Success(_userManager.PasswordHasher.HashPassword(user, password));
        }

        public async Task<Result<Guid>> CreateUserWithPasswordHashAsync(ApplicationUser user, string passwordHash, string role, CancellationToken cancellationToken)
        {
            if (IsAdministratorRole(role)) return Result<Guid>.Failure(AdministratorRoleNotAllowed);
            user.PasswordHash = passwordHash;
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return Result<Guid>.Failure(new Error("User.CreationFail", result.Errors.First().Description));
            await AssignRoleAsync(user, role);
            return Result<Guid>.Success(user.Id);
        }

        public async Task<Result<Guid>> CreateExternalUserAsync(ApplicationUser user, string role, CancellationToken cancellationToken)
        {
            if (IsAdministratorRole(role))
            {
                return Result<Guid>.Failure(AdministratorRoleNotAllowed);
            }

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return Result<Guid>.Failure(new Error("User.CreationFail", result.Errors.First().Description));
            }

            await AssignRoleAsync(user, role);
            return Result<Guid>.Success(user.Id);
        }

        public async Task<Result<IReadOnlyList<string>>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return Result<IReadOnlyList<string>>.Success(roles.ToList());
        }

        public async Task<Result<bool>> ConfirmEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user.EmailConfirmed)
            {
                return Result<bool>.Success(true);
            }

            user.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result<bool>.Failure(new Error("User.ActivationFail", result.Errors.First().Description));
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return Result<bool>.Failure(new Error("User.DeleteFail", result.Errors.First().Description));
            }

            return Result<bool>.Success(true);
        }

        // دور Administrator لحساب واحد يُبذر من الإعدادات فقط، فلا يُمنح عبر هذه الخدمة أبداً
        private static readonly Error AdministratorRoleNotAllowed = new(
            "User.RoleNotAllowed", "The Administrator role cannot be assigned through the application.");

        private static bool IsAdministratorRole(string role)
            => string.Equals(role, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase);

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
