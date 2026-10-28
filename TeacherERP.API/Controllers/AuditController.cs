// جديد — TeacherERP.API/Controllers/AuditController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
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

        // ?targetUserId= اختياري: آخر 100 إجراء
        [HttpGet]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> GetAuditLog([FromQuery] Guid? targetUserId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAuditLogQuery(targetUserId), cancellationToken);
            return ToResponse(result);
        }
    }
}