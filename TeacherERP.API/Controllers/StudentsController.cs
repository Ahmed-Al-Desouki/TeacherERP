// موجود (تعديل) — TeacherERP.API/Controllers/StudentsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Students;
using TeacherERP.Application.Features.Students.BlacklistStudent;
using TeacherERP.Application.Features.Students.GetStudents;
using TeacherERP.Application.Features.Students.GetStudentDetails;
using TeacherERP.Application.Features.Students.GetStudentCard;
using TeacherERP.Application.Features.Students.GetWeeklySchedule;
using TeacherERP.Application.Features.Students.UnblacklistStudent;
using TeacherERP.Application.Features.Students.UnsuspendStudent;
using TeacherERP.Domain.Entities;

namespace TeacherERP.API.Controllers
{
    // طلاب المدرس الحالي (تحتاج تحديد المدرس: X-Teacher-Id أو عضوية واحدة)
    [Authorize]
    public class StudentsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // كارت الطالب وكود QR الخاص بالحساب المسجّل فقط
        [HttpGet("~/api/students/me/card")]
        [Authorize(Policy = AuthPolicies.TenantStudent)]
        public async Task<IActionResult> GetMyCard(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentCardQuery(), cancellationToken);
            return ToResponse(result);
        }

        // القالب الأسبوعي لمجموعات الطالب النشطة في السنتر الحالي؛ الوقت محلي حسب TimeZoneId
        [HttpGet("~/api/students/me/schedule")]
        [Authorize(Policy = AuthPolicies.TenantStudent)]
        public async Task<IActionResult> GetMySchedule(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentWeeklyScheduleQuery(), cancellationToken);
            return ToResponse(result);
        }

        // ?status=Suspended&groupId=&page=1&pageSize=20 : الطلاب مع حالتهم وعداد غيابهم ومجموعتهم (للمدرس والمساعد)
        [HttpGet]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetStudents(
            [FromQuery] MembershipStatus? status,
            [FromQuery] Guid? groupId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = PageRequest.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetStudentsQuery(status, groupId, page, pageSize), cancellationToken);
            return ToResponse(result);
        }

        // تفاصيل طالب داخل مساحة العمل الحالية (للمدرس والمساعد)
        [HttpGet("{studentId:guid}/details")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetStudentDetails(Guid studentId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentDetailsQuery(studentId), cancellationToken);
            if (!result.IsSuccess && result.Error.Code == StudentErrors.NotFound.Code)
            {
                return NotFound(new
                {
                    errorCode = result.Error.Code,
                    errorMessage = result.Error.Message
                });
            }

            return ToResponse(result);
        }

        // فك الإيقاف بسبب (للمدرس والمساعد)
        [HttpPost("{studentId:guid}/unsuspend")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> Unsuspend(Guid studentId, [FromBody] StudentReasonRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UnsuspendStudentCommand(studentId, request.Reason), cancellationToken);
            return ToResponse(result, new { Message = "The student was unsuspended." });
        }

        // حظر الطالب وإنهاء تسجيله (للمدرس فقط)
        [HttpPost("{studentId:guid}/blacklist")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> Blacklist(Guid studentId, [FromBody] StudentReasonRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new BlacklistStudentCommand(studentId, request.Reason), cancellationToken);
            return ToResponse(result, new { Message = "The student was blacklisted." });
        }

        // فك الحظر (للمدرس فقط)
        [HttpPost("{studentId:guid}/unblacklist")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> Unblacklist(Guid studentId, [FromBody] StudentReasonRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UnblacklistStudentCommand(studentId, request.Reason), cancellationToken);
            return ToResponse(result, new { Message = "The student was unblacklisted." });
        }
    }
}
