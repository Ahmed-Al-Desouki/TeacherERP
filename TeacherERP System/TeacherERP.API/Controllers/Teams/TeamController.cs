// موجود (تعديل) — TeacherERP.API/Controllers/TeamController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Team.CreateAssistant;
using TeacherERP.Application.Features.Team.GetTeam;
using TeacherERP.Application.Features.Team.RemoveAssistant;
using TeacherERP.Application.Features.Team.ResendWelcome;
using TeacherERP.Application.Features.Team.SetAssistantSuspension;
using TeacherERP.Application.Features.Team.SetFinancialDelegation;

namespace TeacherERP.API.Controllers
{
    // فريق المدرس الحالي (تحتاج تحديد المدرس: X-Teacher-Id أو عضوية واحدة). كله للمدرس فقط
    [Authorize]
    public class TeamController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TeamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // قائمة المساعدين مع حالتهم والتفويض المالي وهل فعّلوا حساباتهم
        [HttpGet]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> GetTeam(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTeamQuery(), cancellationToken);
            return ToResponse(result);
        }

        // المدرس ينشئ حساب مساعد بإيميله (أو يربط مساعداً موجوداً). الباسورد المؤقت يصل للمساعد على إيميله فقط
        [HttpPost("assistants")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> CreateAssistant([FromBody] CreateAssistantCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // باسورد مؤقت جديد وإيميل ترحيب لمساعد لسه ما فعّلش
        [HttpPost("{userId:guid}/resend-welcome")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> ResendWelcome(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ResendWelcomeCommand(userId), cancellationToken);
            return ToResponse(result, new { Message = "A new temporary password was sent to the assistant's email." });
        }

        [HttpPut("{userId:guid}/financial-delegation")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> SetFinancialDelegation(Guid userId, [FromBody] SetFinancialDelegationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetFinancialDelegationCommand(userId, request.Granted), cancellationToken);
            return ToResponse(result, new { Message = "The financial delegation was updated." });
        }

        // إيقاف مؤقت: المساعد يُمنع فوراً عند هذا المدرس فقط
        [HttpPost("{userId:guid}/suspend")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> Suspend(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetAssistantSuspensionCommand(userId, true), cancellationToken);
            return ToResponse(result, new { Message = "The assistant was suspended." });
        }

        [HttpPost("{userId:guid}/activate")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> Activate(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetAssistantSuspensionCommand(userId, false), cancellationToken);
            return ToResponse(result, new { Message = "The assistant was activated." });
        }

        // إزالة المساعد من فريقي (ومسح حسابه لو لم يُفعَّل ولا ينتمي لمدرس آخر)
        [HttpDelete("{userId:guid}")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> RemoveAssistant(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveAssistantCommand(userId), cancellationToken);
            return ToResponse(result, new { Message = "The assistant was removed from your team." });
        }
    }
}