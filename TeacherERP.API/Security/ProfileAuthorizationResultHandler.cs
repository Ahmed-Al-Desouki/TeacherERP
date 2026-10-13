//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authorization.Policy;

//namespace TeacherERP.API.Security
//{
//    // يحوّل رفض الـ Policy بسبب البروفايل الناقص إلى 403 بجسم واضح (نفس شكل أخطاء الـ API)، والباقي كالمعتاد
//    public class ProfileAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
//    {
//        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

//        public async Task HandleAsync(
//            RequestDelegate next,
//            HttpContext context,
//            AuthorizationPolicy policy,
//            PolicyAuthorizationResult authorizeResult)
//        {
//            var failedBecauseOfProfile = authorizeResult.Forbidden
//                && authorizeResult.AuthorizationFailure is { } failure
//                && failure.FailedRequirements.OfType<CompletedProfileRequirement>().Any();

//            if (failedBecauseOfProfile)
//            {
//                context.Response.StatusCode = StatusCodes.Status403Forbidden;
//                await context.Response.WriteAsJsonAsync(new
//                {
//                    errorCode = "Profile.Incomplete",
//                    errorMessage = "Please complete your profile to use this feature."
//                });
//                return;
//            }

//            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
//        }
//    }
//}