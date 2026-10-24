// موجود (تعديل) — TeacherERP.API/Controllers/ScheduleController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Schedule.AddExtraSession;
using TeacherERP.Application.Features.Schedule.BulkCancelSessions;
using TeacherERP.Application.Features.Schedule.BulkShiftSessions;
using TeacherERP.Application.Features.Schedule.CancelSession;
using TeacherERP.Application.Features.Schedule.GetSessions;
using TeacherERP.Application.Features.Schedule.RescheduleSession;
using TeacherERP.Application.Features.Schedule.RestoreSession;

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

        // تأجيل أو تقديم حصة قادمة (للمدرس أو المساعد)
        [HttpPut("sessions/{id:guid}/reschedule")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> RescheduleSession(Guid id, [FromBody] RescheduleSessionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RescheduleSessionCommand(id, request.NewStartTime, request.DurationMinutes), cancellationToken);
            return ToResponse(result);
        }

        // إلغاء حصة قادمة بسبب اختياري (للمدرس أو المساعد)
        [HttpPost("sessions/{id:guid}/cancel")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> CancelSession(Guid id, [FromBody] CancelSessionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CancelSessionCommand(id, request.Reason), cancellationToken);
            return ToResponse(result);
        }

        // التراجع عن إلغاء حصة قادمة (للمدرس أو المساعد)
        [HttpPost("sessions/{id:guid}/restore")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> RestoreSession(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RestoreSessionCommand(id), cancellationToken);
            return ToResponse(result);
        }

        // حصة إضافية لمجموعة خارج القالب الأسبوعي (للمدرس أو المساعد)
        [HttpPost("sessions")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> AddExtraSession([FromBody] AddExtraSessionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // إلغاء جماعي لحصص نطاق (إجازة مثلاً) (للمدرس فقط)
        [HttpPost("sessions/bulk-cancel")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> BulkCancelSessions([FromBody] BulkCancelSessionsCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // تأجيل أو تقديم جماعي لحصص نطاق بعدد دقائق (للمدرس فقط)
        [HttpPost("sessions/bulk-shift")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> BulkShiftSessions([FromBody] BulkShiftSessionsCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }
    }
}