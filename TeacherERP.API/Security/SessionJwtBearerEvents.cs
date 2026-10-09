using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Sessions;

namespace TeacherERP.API.Security
{
    // بعد التحقق من توقيع التوكن وصلاحيته، نتأكد أن الجلسة المرتبطة به ما زالت نشطة
    public class SessionJwtBearerEvents : JwtBearerEvents
    {
        public override async Task TokenValidated(TokenValidatedContext context)
        {
            var sessionClaim = context.Principal?.FindFirst(AuthClaimTypes.SessionId)?.Value;

            if (!Guid.TryParse(sessionClaim, out var sessionId))
            {
                context.Fail("The token is not bound to a session.");
                return;
            }

            var sessionValidator = context.HttpContext.RequestServices.GetRequiredService<ISessionValidator>();

            if (!await sessionValidator.IsActiveAsync(sessionId, context.HttpContext.RequestAborted))
            {
                context.Fail("The session is no longer active.");
            }
        }
    }
}