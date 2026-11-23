// جديد — TeacherERP.API/Controllers/GroupAdminsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.API.Extensions;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.GroupAdmins.ApproveGroupAdminRequest;
using TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests;
using TeacherERP.Application.Features.GroupAdmins.GetGroupAdmins;
using TeacherERP.Application.Features.GroupAdmins.GetGroupSessionAttendance;
using TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin;
using TeacherERP.Application.Features.GroupAdmins.GroupScan;
using TeacherERP.Application.Features.GroupAdmins.RejectGroupAdminRequest;
using TeacherERP.Application.Features.GroupAdmins.RequestGroupAdminChange;
using TeacherERP.Domain.Entities;

namespace TeacherERP.API.Controllers
{
    // الطلاب المسؤولون عن المجموعات (تحتاج تحديد المدرس: X-Teacher-Id أو عضوية واحدة).
    // المسارات المطلقة (~/) لتبقى تحت /api/Groups و/api/Attendance حيث يتوقعها العميل
    [Authorize]
    public class GroupAdminsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public GroupAdminsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---- للتطبيق (أي عضو) ----

        // هل أنا طالب مسؤول عند هذا المدرس؟ (يفتح شاشة المسح لو true)
        [HttpGet("me")]
        [Authorize(Policy = AuthPolicies.TenantMember)]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyGroupAdminQuery(), cancellationToken);
            return ToResponse(result);
        }

        // مسح كارت طالب في الحصة الجارية لمجموعتي (الموقع هو موقعي أنا)
        [HttpPost("~/api/Attendance/group-scan")]
        [Authorize(Policy = AuthPolicies.TenantMember)]
        [EnableRateLimiting(RateLimitPolicies.AttendanceScan)]
        public async Task<IActionResult> GroupScan([FromBody] GroupScanCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // حضور الحصة الجارية لمجموعتي (الاسم والحالة فقط)
        [HttpGet("~/api/Attendance/group-session")]
        [Authorize(Policy = AuthPolicies.TenantMember)]
        public async Task<IActionResult> GetGroupSession(
            [FromQuery] int page = 1, [FromQuery] int pageSize = PageRequest.DefaultPageSize, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetGroupSessionAttendanceQuery(page, pageSize), cancellationToken);
            return ToResponse(result);
        }

        // ---- للموظفين: التعيين والإزالة ----

        [HttpGet("~/api/Groups/{groupId:guid}/admins")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetAdmins(Guid groupId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetGroupAdminsQuery(groupId), cancellationToken);
            return ToResponse(result);
        }

        // تعيين: المدرس فوري (200). المساعد طلب ينتظر المدرس (202) إلا لو فعّل الموافقة التلقائية
        [HttpPut("~/api/Groups/{groupId:guid}/admins/{studentId:guid}")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public Task<IActionResult> Appoint(Guid groupId, Guid studentId, CancellationToken cancellationToken)
            => ChangeAsync(new RequestGroupAdminChangeCommand(groupId, studentId, GroupAdminRequestType.Appoint), cancellationToken);

        [HttpDelete("~/api/Groups/{groupId:guid}/admins/{studentId:guid}")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public Task<IActionResult> Remove(Guid groupId, Guid studentId, CancellationToken cancellationToken)
            => ChangeAsync(new RequestGroupAdminChangeCommand(groupId, studentId, GroupAdminRequestType.Remove), cancellationToken);

        // ---- للمدرس: طلبات المساعدين ----

        // ?status=Pending&page=1&pageSize=20
        [HttpGet("requests")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> GetRequests(
            [FromQuery] GroupAdminRequestStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = PageRequest.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetGroupAdminRequestsQuery(status, page, pageSize), cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("requests/{id:guid}/approve")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> ApproveRequest(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ApproveGroupAdminRequestCommand(id), cancellationToken);
            return ToResponse(result, new { Message = "The request was approved." });
        }

        [HttpPost("requests/{id:guid}/reject")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> RejectRequest(Guid id, [FromBody] RejectGroupAdminRequestRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RejectGroupAdminRequestCommand(id, request.Note), cancellationToken);
            return ToResponse(result, new { Message = "The request was rejected." });
        }

        private async Task<IActionResult> ChangeAsync(RequestGroupAdminChangeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            // 202: الطلب اتسجّل وينتظر موافقة المدرس
            if (result.IsSuccess && !result.Value!.Applied)
            {
                return Accepted(result.Value);
            }

            return ToResponse(result);
        }
    }
}