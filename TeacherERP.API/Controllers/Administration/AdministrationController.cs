using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Administration.CreateCenter;
using TeacherERP.Application.Features.Administration.CreateTeacher;
using TeacherERP.Application.Features.Administration.LinkTeacherToCenter;
using TeacherERP.Application.Features.TeacherRequests.Admin;

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

        // طلبات انضمام المدرسين بصفحات (الأحدث أولاً)، مع تصفية اختيارية بالحالة
        [HttpGet("teacher-requests")]
        public async Task<IActionResult> ListTeacherRequests([FromQuery] ListTeacherRequestsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return ToResponse(result);
        }

        // يوافق على طلب مدرس: يُنشأ حسابه بباسورد مؤقت يصله على إيميله (بلا سقف شهري)
        [HttpPost("teacher-requests/{requestId:guid}/approve")]
        public async Task<IActionResult> ApproveTeacherRequest(Guid requestId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ApproveTeacherRequestByAdminCommand(requestId), cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("teacher-requests/{requestId:guid}/reject")]
        public async Task<IActionResult> RejectTeacherRequest(
            Guid requestId,
            [FromBody] RejectTeacherRequestRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RejectTeacherRequestByAdminCommand(requestId, request.Reason), cancellationToken);
            return ToResponse(result, new { Message = "The request was rejected." });
        }
    }
}
