using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Profile.CompleteProfile;
using TeacherERP.Application.Features.Profile.GetMyProfile;

namespace TeacherERP.API.Controllers
{
    // بروفايل المستخدم الحالي. المسار المطلق (~/) يعطي: /api/Users/me/profile
    [Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("~/api/Users/me/profile")]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyProfileQuery(), cancellationToken);
            return ToResponse(result);
        }

        [HttpPut("~/api/Users/me/profile")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }
    }
}