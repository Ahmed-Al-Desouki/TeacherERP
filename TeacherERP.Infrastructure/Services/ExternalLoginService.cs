using Microsoft.AspNetCore.Identity;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Infrastructure.Services
{
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalLoginService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<ApplicationUser?>> FindByExternalLoginAsync(string provider, string providerKey, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByLoginAsync(provider, providerKey);
            return Result<ApplicationUser?>.Success(user);
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
    }
}