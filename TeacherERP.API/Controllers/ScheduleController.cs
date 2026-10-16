using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Schedule.GetSessions;

namespace TeacherERP.API.Controllers
{
    // الجدول الفعلي (الحصص المولّدة) للمدرس الحالي
    [Authorize]
    public class ScheduleController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // الحصص في نطاق: ?from=&to=&groupId= (الافتراضي 14 يوم قادمة)
        [HttpGet("sessions")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetSessions([FromQuery] GetSessionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return ToResponse(result);
        }
    }
}