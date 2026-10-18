// موجود (تعديل) — TeacherERP.API/Controllers/GroupsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
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
                id, request.Name, request.GradeLevel, request.CenterId, request.Room,
                request.Capacity, request.WhatsAppGroupLink, request.IsActive);

            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // استبدال الجدول الأسبوعي من تاريخ معيّن (للمدرس فقط)
        [HttpPut("{id:guid}/schedule")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> UpdateGroupSchedule(Guid id, [FromBody] UpdateGroupScheduleRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new UpdateGroupScheduleCommand(id, request.EffectiveFrom, request.Slots), cancellationToken);
            return ToResponse(result);
        }

        // المدرس يرى كل مجموعاته، والمساعد المخصصة له فقط
        [HttpGet]
        [Authorize(Policy = AuthPolicies.TenantStaff)]
        public async Task<IActionResult> GetGroups(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetGroupsQuery(), cancellationToken);
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