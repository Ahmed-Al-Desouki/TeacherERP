using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TeacherERP.Application.Features.Profile;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.API.Security
{
    public class CompletedProfileHandler : AuthorizationHandler<CompletedProfileRequirement>
    {
        private readonly IProfileCompletionChecker _profileCompletionChecker;

        public CompletedProfileHandler(IProfileCompletionChecker profileCompletionChecker)
        {
            _profileCompletionChecker = profileCompletionChecker;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CompletedProfileRequirement requirement)
        {
            // المدرس والمساعد غير مطلوب منهما بروفايل
            var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            if (!ProfileRequirement.AppliesTo(roles))
            {
                context.Succeed(requirement);
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return;
            }

            if (await _profileCompletionChecker.IsCompleteAsync(userId, CancellationToken.None))
            {
                context.Succeed(requirement);
            }
        }
    }
}