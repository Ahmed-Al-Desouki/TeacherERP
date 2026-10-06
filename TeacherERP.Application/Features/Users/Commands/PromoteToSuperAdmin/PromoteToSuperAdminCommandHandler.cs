using MediatR;
using Microsoft.AspNetCore.Identity;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.PromoteToSuperAdmin
{
    public class PromoteToSuperAdminCommandHandler : IRequestHandler<PromoteToSuperAdminCommand, Result<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PromoteToSuperAdminCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(PromoteToSuperAdminCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return Result<bool>.Failure(new Error("User.NotFound", "User not found."));
            }

            if (!await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                var result = await _userManager.AddToRoleAsync(user, "SuperAdmin");
                if (!result.Succeeded)
                {
                    return Result<bool>.Failure(new Error("Promotion.Failed", result.Errors.First().Description));
                }
            }

            return Result<bool>.Success(true);
        }
    }
}