using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Groups.CreateGroup;
using TeacherERP.Application.Features.Groups.GetGroup;
using TeacherERP.Application.Features.Groups.GetGroups;

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