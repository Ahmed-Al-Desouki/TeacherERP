using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Administration.CreateCenter;
using TeacherERP.Application.Features.Administration.CreateTeacher;
using TeacherERP.Application.Features.Administration.LinkTeacherToCenter;

namespace TeacherERP.API.Controllers
{
    // إدارة المنصة: مدير النظام فقط (حساب واحد)، ولا يحتاج تحديد سنتر
    [Authorize(Policy = AuthPolicies.AdministratorOnly)]
    public class AdministrationController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AdministrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // يضيف مدرساً بإيميله: حساب بباسورد مؤقت يصله على الإيميل + مساحته الخاصة
        [HttpPost("teachers")]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // ينشئ سنتراً مملوكاً لمدرس موجود (يصير مؤسسه)
        [HttpPost("centers")]
        public async Task<IActionResult> CreateCenter([FromBody] CreateCenterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // يربط مدرساً موجوداً بسنتر (مدرس إضافي في نفس المكان)
        [HttpPost("centers/{centerId:guid}/teachers")]
        public async Task<IActionResult> LinkTeacherToCenter(
            Guid centerId,
            [FromBody] LinkTeacherToCenterRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LinkTeacherToCenterCommand(centerId, request.TeacherUserId), cancellationToken);
            return ToResponse(result);
        }
    }
}
