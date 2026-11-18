using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Finance.CollectCashPayment;
using TeacherERP.Application.Features.Finance.GetPaymentOptions;

namespace TeacherERP.API.Controllers
{
    [Authorize(Policy = AuthPolicies.TenantFinance)]
    public sealed class FinanceController : BaseApiController
    {
        private readonly IMediator _mediator;
        public FinanceController(IMediator mediator) => _mediator = mediator;

        [HttpGet("students/{studentId:guid}/sessions/{sessionId:guid}/payment-options")]
        public async Task<IActionResult> GetPaymentOptions(Guid studentId, Guid sessionId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPaymentOptionsQuery(studentId, sessionId), cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("cash-payments")]
        public async Task<IActionResult> CollectCash([FromBody] CollectCashPaymentCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }
    }
}
