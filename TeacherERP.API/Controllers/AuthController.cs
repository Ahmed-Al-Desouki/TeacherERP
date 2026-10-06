using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;

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
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            // استخدام الدالة الموحدة مباشرة لإرجاع الـ AuthResponseDto
            return ToResponse(result);
        }
    }
}