using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.API.Security
{
    public class TenantHandler : AuthorizationHandler<TenantRequirement>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantHandler(
            ITenantRepository tenantRepository,
            ICurrentTenantAccessor currentTenant,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantRepository = tenantRepository;
            _currentTenant = currentTenant;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantRequirement requirement)
        {
            // غير المصادَق عليه يتعامل معه الـ Challenge الافتراضي (401)، لا نحكم عليه هنا
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var cancellationToken = httpContext?.RequestAborted ?? CancellationToken.None;

            var error = await ResolveAsync(userId, httpContext, cancellationToken);
            if (error is null)
            {
                context.Succeed(requirement);
                return;
            }

            // كود الخطأ يُقرأ لاحقاً في ApiAuthorizationResultHandler لتحويله إلى رد واضح
            context.Fail(new AuthorizationFailureReason(this, error.Code));
        }

        // null = نجح تحديد السنتر (وتم ضبطه في الـ Accessor)، وإلا الخطأ المناسب
        private async Task<Error?> ResolveAsync(Guid userId, HttpContext? httpContext, CancellationToken cancellationToken)
        {
            TenantMembershipInfo? membership;

            if (httpContext is not null && httpContext.Request.Headers.TryGetValue(TenantRequirement.HeaderName, out var values))
            {
                // Header موجود: لازم قيمة واحدة بصيغة Guid صالحة، وإلا هو خطأ من العميل
                if (values.Count != 1 || !Guid.TryParse(values[0], out var tenantId) || tenantId == Guid.Empty)
                {
                    return TenantErrors.InvalidHeader;
                }

                // سنتر غير موجود أو لست عضواً فيه: نفس الخطأ حتى لا نكشف وجود سناتر
                membership = await _tenantRepository.GetMembershipAsync(userId, tenantId, cancellationToken);
                if (membership is null)
                {
                    return TenantErrors.Forbidden;
                }
            }
            else
            {
                // بدون Header: التحديد التلقائي يحدث فقط لو للمستخدم عضوية واحدة
                var memberships = await _tenantRepository.GetMembershipsAsync(userId, cancellationToken);

                if (memberships.Count == 0)
                {
                    return TenantErrors.NoMembership;
                }

                if (memberships.Count > 1)
                {
                    return TenantErrors.Required;
                }

                membership = memberships[0];
            }

            if (membership.TenantStatus != TenantStatus.Active)
            {
                return TenantErrors.Suspended;
            }

            _currentTenant.Set(new TenantContext(membership.TenantId, membership.Role));
            return null;
        }
    }
}