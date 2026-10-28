// جديد — TeacherERP.API/Controllers/StudentsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Students;
using TeacherERP.Application.Features.Students.BlacklistStudent;
using TeacherERP.Application.Features.Students.GetStudents;
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

        // ?status=Suspended&groupId= : قائمة الطلاب مع حالتهم وعداد غيابهم ومجموعتهم (للمدرس والمساعد)
        [HttpGet]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetStudents([FromQuery] MembershipStatus? status, [FromQuery] Guid? groupId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentsQuery(status, groupId), cancellationToken);
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