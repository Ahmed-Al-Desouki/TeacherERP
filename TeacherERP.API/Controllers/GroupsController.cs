// موجود (تعديل) — TeacherERP.API/Controllers/GroupsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Groups.CreateGroup;
using TeacherERP.Application.Features.Groups.GetGroup;
using TeacherERP.Application.Features.Groups.GetGroups;
using TeacherERP.Application.Features.Groups.UpdateGroup;
using TeacherERP.Application.Features.Groups.UpdateGroupSchedule;

namespace TeacherERP.API.Controllers
{
    // مجموعات المدرس الحالي (تحتاج تحديد المدرس: X-Teacher-Id أو عضوية واحدة)
    [Authorize]
    public class GroupsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public GroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // إنشاء مجموعة بجدولها الأسبوعي الثابت (للمدرس فقط)
        [HttpPost]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // تعديل بيانات المجموعة أو أرشفتها (للمدرس فقط)
        [HttpPut("{id:guid}")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateGroupCommand(
                id, request.Name, request.Grade, request.CenterId, request.Room,
                request.Capacity, request.WhatsAppGroupLink, request.IsActive, request.ConfirmRoomConflicts);

            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // استبدال الجدول الأسبوعي من تاريخ معيّن (للمدرس فقط)
        [HttpPut("{id:guid}/schedule")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> UpdateGroupSchedule(Guid id, [FromBody] UpdateGroupScheduleRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new UpdateGroupScheduleCommand(id, request.EffectiveFrom, request.Slots, request.ConfirmRoomConflicts), cancellationToken);
            return ToResponse(result);
        }

        // ?page=1&pageSize=20 : المدرس والمساعد يريان كل المجموعات
        [HttpGet]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetGroups(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = PageRequest.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetGroupsQuery(page, pageSize), cancellationToken);
            return ToResponse(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetGroup(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetGroupQuery(id), cancellationToken);
            return ToResponse(result);
        }
    }
}
