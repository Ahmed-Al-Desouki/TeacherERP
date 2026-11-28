// موجود (تعديل) — TeacherERP.API/Controllers/AuditController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Audit.GetAuditLog;

namespace TeacherERP.API.Controllers
{
    // سجل التدقيق للمدرس الحالي (للمدرس فقط)
    [Authorize]
    public class AuditController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AuditController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ?targetUserId=&page=1&pageSize=20 : الأحدث أولاً
        [HttpGet]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> GetAuditLog(
            [FromQuery] Guid? targetUserId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = PageRequest.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAuditLogQuery(targetUserId, page, pageSize), cancellationToken);
            return ToResponse(result);
        }
    }
}
