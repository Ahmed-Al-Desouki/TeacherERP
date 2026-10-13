using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Centers.GetCurrentCenter;
using TeacherERP.Application.Features.Centers.GetMyCenters;

namespace TeacherERP.API.Controllers
{
    // سناتر المستخدم الحالي (المسار: /api/Centers)
    [Authorize]
    public class CentersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public CentersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // كل السناتر التي أنا عضو فيها (لا تحتاج سنتر محدد)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyCenters(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyCentersQuery(), cancellationToken);
            return ToResponse(result);
        }

        // السنتر الحالي للطلب (بالـ Header X-Center-Id أو تلقائياً لو عضوية واحدة)
        [HttpGet("current")]
        [Authorize(Policy = AuthPolicies.TenantMember)]
        public async Task<IActionResult> GetCurrentCenter(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentCenterQuery(), cancellationToken);
            return ToResponse(result);
        }
    }
}