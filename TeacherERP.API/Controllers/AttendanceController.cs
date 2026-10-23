// جديد — TeacherERP.API/Controllers/AttendanceController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Attendance.AdminScan;
using TeacherERP.Application.Features.Attendance.ApproveCompensation;
using TeacherERP.Application.Features.Attendance.GetAttendanceSettings;
using TeacherERP.Application.Features.Attendance.GetDisplayCode;
using TeacherERP.Application.Features.Attendance.GetPendingCompensations;
using TeacherERP.Application.Features.Attendance.GetSessionAttendance;
using TeacherERP.Application.Features.Attendance.RegenerateStaticCode;
using TeacherERP.Application.Features.Attendance.RejectCompensation;
using TeacherERP.Application.Features.Attendance.ScanAttendance;
using TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings;

namespace TeacherERP.API.Controllers
{
    // الحضور: مسح الطالب (بدون تحديد مدرس، فالمدرس داخل الكود) وأعمال الموظفين (تحتاج تحديد المدرس)
    [Authorize]
    public class AttendanceController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AttendanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---- جهة الطالب ----

        // مسح الكود بالموقع. يشترط اكتمال البروفايل
        [HttpPost("scan")]
        [Authorize(Policy = AuthPolicies.CompletedProfile)]
        public async Task<IActionResult> Scan([FromBody] ScanAttendanceCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // ---- جهة الموظفين ----

        // مسح كارت الطالب (للمدرس أو المساعد)
        [HttpPost("admin-scan")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> AdminScan([FromBody] AdminScanCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // الكود المتغير الحالي لشاشة السنتر (يُجدَّد قبل ExpiresInSeconds)
        [HttpGet("display-code")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetDisplayCode(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDisplayCodeQuery(), cancellationToken);
            return ToResponse(result);
        }

        [HttpGet("sessions/{sessionId:guid}")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetSessionAttendance(Guid sessionId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSessionAttendanceQuery(sessionId), cancellationToken);
            return ToResponse(result);
        }

        [HttpGet("compensations/pending")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetPendingCompensations(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPendingCompensationsQuery(), cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("compensations/{id:guid}/approve")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> ApproveCompensation(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ApproveCompensationCommand(id), cancellationToken);
            return ToResponse(result, new { Message = "The compensation was approved." });
        }

        [HttpPost("compensations/{id:guid}/reject")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> RejectCompensation(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RejectCompensationCommand(id), cancellationToken);
            return ToResponse(result, new { Message = "The compensation was rejected." });
        }

        // ---- إعدادات الحضور (للمدرس فقط) ----

        [HttpGet("settings")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAttendanceSettingsQuery(), cancellationToken);
            return ToResponse(result);
        }

        [HttpPut("settings")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateAttendanceSettingsCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // تغيير سر الحضور فيبطل الكود المطبوع والمتغير الحالي
        [HttpPost("settings/regenerate-static-code")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> RegenerateStaticCode(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RegenerateStaticCodeCommand(), cancellationToken);
            return ToResponse(result);
        }
    }
}