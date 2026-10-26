// موجود (تعديل) — TeacherERP.API/Controllers/PasswordController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.API.Extensions;
using TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword;
using TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword;
using TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword;
using TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp;

namespace TeacherERP.API.Controllers
{
    // استرجاع كلمة المرور (3 خطوات) وتغييرها للمسجّل دخوله. المسارات المطلقة (~/) تُبقي الـ URLs كما هي
    public class PasswordController : BaseApiController
    {
        private readonly IMediator _mediator;

        public PasswordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("~/api/Auth/forgot-password")]
        [EnableRateLimiting(RateLimitPolicies.AuthOtpSend)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("~/api/Auth/verify-reset-otp")]
        [EnableRateLimiting(RateLimitPolicies.AuthOtpVerify)]
        public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyResetOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("~/api/Auth/reset-password")]
        [EnableRateLimiting(RateLimitPolicies.AuthOtpVerify)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result, new { Message = "Password reset successfully. Please log in with your new password." });
        }

        // تغيير الباسورد بالباسورد الحالي. باقي الأجهزة تخرج، والجهاز الحالي يكمل
        [Authorize]
        [HttpPost("~/api/Users/me/password")]
        [EnableRateLimiting(RateLimitPolicies.AuthLogin)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result, new { Message = "Password changed successfully. Your other devices were signed out." });
        }
    }
}