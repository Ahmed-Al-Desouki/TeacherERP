using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Centers.GetMyCenters;
using TeacherERP.Application.Features.Centers.UpdateCenter;

namespace TeacherERP.API.Controllers
{
    // السناتر الفعلية (الأماكن). لا تحتاج مدرس محدد، والملكية تُتحقق في الـ Handlers
    [Authorize]
    public class CentersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public CentersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // السناتر التي أملكها أو أدرّس فيها
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyCenters(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyCentersQuery(), cancellationToken);
            return ToResponse(result);
        }

        // تعديل بيانات السنتر وموقعه (لصاحبه فقط)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCenter(Guid id, [FromBody] UpdateCenterRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCenterCommand(
                id, request.Name, request.Address, request.Latitude, request.Longitude, request.GeofenceRadiusMeters);

            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }
    }
}