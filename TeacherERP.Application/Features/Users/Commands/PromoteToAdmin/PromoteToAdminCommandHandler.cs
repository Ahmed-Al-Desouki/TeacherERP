// موجود (تعديل) — TeacherERP.Application/Features/Users/Commands/PromoteToAdmin/PromoteToAdminCommandHandler.cs
using MediatR;
using Microsoft.AspNetCore.Identity;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.PromoteToAdmin
{
    public class PromoteToAdminCommandHandler : IRequestHandler<PromoteToAdminCommand, Result<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PromoteToAdminCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(PromoteToAdminCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return Result<bool>.Failure(new Error("User.NotFound", "User not found."));
            }

            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Student");
            }

            if (!await _userManager.IsInRoleAsync(user, RoleNames.Assistant))
            {
                var result = await _userManager.AddToRoleAsync(user, RoleNames.Assistant);
                if (!result.Succeeded)
                {
                    return Result<bool>.Failure(new Error("User.PromotionFailed", result.Errors.First().Description));
                }
            }

            return Result<bool>.Success(true);
        }
    }
}