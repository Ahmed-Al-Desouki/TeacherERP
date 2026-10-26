// جديد — TeacherERP.API/Controllers/TeamController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Team.CreateAssistant;

namespace TeacherERP.API.Controllers
{
    // فريق المدرس الحالي (تحتاج تحديد المدرس: X-Teacher-Id أو عضوية واحدة). إنشاء المساعدين للمدرس فقط
    [Authorize]
    public class TeamController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TeamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // المدرس ينشئ حساب مساعد بإيميله (أو يربط مساعداً موجوداً). الباسورد المؤقت يصل للمساعد على إيميله فقط
        [HttpPost("assistants")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> CreateAssistant([FromBody] CreateAssistantCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }
    }
}