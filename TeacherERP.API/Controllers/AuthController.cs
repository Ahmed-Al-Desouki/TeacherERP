// موجود (تعديل) — TeacherERP.API/Controllers/AuthController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.API.Extensions;
using TeacherERP.Application.Features.Sessions.Logout;
using TeacherERP.Application.Features.Sessions.LogoutAll;
using TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken;

namespace TeacherERP.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [EnableRateLimiting(RateLimitPolicies.AuthLogin)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("google")]
        [EnableRateLimiting(RateLimitPolicies.AuthLogin)]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("refresh")]
        [EnableRateLimiting(RateLimitPolicies.AuthSession)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // بدون Authorize: الـ Refresh Token نفسه يثبت ملكية الجلسة، فيعمل حتى لو انتهى الـ Access Token
        [HttpPost("logout")]
        [EnableRateLimiting(RateLimitPolicies.AuthSession)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result, new { Message = "Logged out successfully." });
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LogoutAllCommand(), cancellationToken);
            return ToResponse(result, new { Message = "Logged out from all devices." });
        }
    }
}