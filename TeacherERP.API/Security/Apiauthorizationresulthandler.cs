using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Domain.Shared;

namespace TeacherERP.API.Security
{
    // يحوّل رفض الـ Policies (البروفايل الناقص أو تحديد السنتر) إلى ردود واضحة بنفس شكل أخطاء الـ API، والباقي كالمعتاد
    public class ApiAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure is { } failure)
            {
                // أخطاء السنتر تأتي عبر FailureReasons (لأن الـ Handler استدعى Fail)، فنفحصها أولاً
                if (FindTenantError(failure) is { } tenantError)
                {
                    context.Response.StatusCode = StatusCodeFor(tenantError);
                    await context.Response.WriteAsJsonAsync(new
                    {
                        errorCode = tenantError.Code,
                        errorMessage = tenantError.Message
                    });
                    return;
                }

                if (failure.FailedRequirements.OfType<CompletedProfileRequirement>().Any())
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        errorCode = "Profile.Incomplete",
                        errorMessage = "Please complete your profile to use this feature."
                    });
                    return;
                }
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }

        private static Error? FindTenantError(AuthorizationFailure failure)
            => failure.FailureReasons
                .Where(reason => reason.Handler is TenantHandler)
                .Select(reason => TenantErrors.FromCode(reason.Message))
                .FirstOrDefault(error => error is not null);

        // نقص بيانات من العميل = 400، وغير ذلك (ممنوع أو موقوف) = 403
        private static int StatusCodeFor(Error error)
            => error == TenantErrors.Required || error == TenantErrors.InvalidHeader
                ? StatusCodes.Status400BadRequest
                : StatusCodes.Status403Forbidden;
    }
}