using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Sessions.GetMySessions;
using TeacherERP.Application.Features.Sessions.RevokeSession;

namespace TeacherERP.API.Controllers
{
    // أجهزة المستخدم المسجّل دخوله (القائمة وإنهاء جهاز)
    [Authorize]
    public class SessionsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public SessionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMySessions(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMySessionsQuery(), cancellationToken);
            return ToResponse(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RevokeSession(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RevokeSessionCommand(id), cancellationToken);
            return ToResponse(result, new { Message = "The device was signed out." });
        }
    }
}