// جديد — TeacherERP.API/Controllers/EnrollmentController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Enrollment.ApproveRequest;
using TeacherERP.Application.Features.Enrollment.CancelRequest;
using TeacherERP.Application.Features.Enrollment.GetMyEnrollments;
using TeacherERP.Application.Features.Enrollment.GetRequests;
using TeacherERP.Application.Features.Enrollment.RejectRequest;
using TeacherERP.Application.Features.Enrollment.RequestGroup;
using TeacherERP.Domain.Entities;

namespace TeacherERP.API.Controllers
{
    // انضمام الطلاب ونقلهم بين المجموعات: جهة الطالب (بدون تحديد مدرس) وجهة الموظفين (تحتاج تحديد المدرس)
    [Authorize]
    public class EnrollmentController : BaseApiController
    {
        private readonly IMediator _mediator;

        public EnrollmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---- جهة الطالب ----

        // طلب انضمام (أو نقل لو مسجل عند نفس المدرس). يشترط اكتمال البروفايل
        [HttpPost("requests")]
        [Authorize(Policy = AuthPolicies.CompletedProfile)]
        public async Task<IActionResult> RequestGroup([FromBody] RequestGroupCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // تسجيلاتي وطلباتي
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyEnrollmentsQuery(), cancellationToken);
            return ToResponse(result);
        }

        // إلغاء طلبي المعلّق
        [HttpDelete("requests/{id:guid}")]
        public async Task<IActionResult> CancelRequest(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CancelRequestCommand(id), cancellationToken);
            return ToResponse(result, new { Message = "The request was cancelled." });
        }

        // ---- جهة الموظفين (المدرس الكل، والمساعد مجموعاته فقط) ----

        [HttpGet("requests")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetRequests([FromQuery] EnrollmentRequestStatus? status, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetRequestsQuery(status), cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("requests/{id:guid}/approve")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ApproveRequestCommand(id), cancellationToken);
            return ToResponse(result, new { Message = "The request was approved." });
        }

        [HttpPost("requests/{id:guid}/reject")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRequestRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RejectRequestCommand(id, request.Note), cancellationToken);
            return ToResponse(result, new { Message = "The request was rejected." });
        }
    }
}