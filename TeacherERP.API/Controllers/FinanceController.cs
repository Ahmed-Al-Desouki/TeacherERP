using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.API.Extensions;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Finance.CollectCashPayment;
using TeacherERP.Application.Features.Finance.GetPaymentOptions;
using TeacherERP.Application.Features.Finance.GetTeacherPaymentSettings;
using TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings;
using TeacherERP.Application.Features.Finance.GetPendingInstaPayReceipts;
using TeacherERP.Application.Features.Finance.ApproveInstaPayReceipt;
using TeacherERP.Application.Features.Finance.RejectInstaPayReceipt;
using TeacherERP.Application.SessionContent;

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

        [HttpGet("instapay-settings")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> GetInstaPaySettings(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTeacherPaymentSettingsQuery(), cancellationToken);
            return ToResponse(result);
        }

        [HttpPut("instapay-settings")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        [EnableRateLimiting(RateLimitPolicies.ContentUpload)]
        [RequestSizeLimit(SessionContentPolicy.MaxRequestBytes)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateInstaPaySettings(
            [FromForm] UpdateTeacherPaymentSettingsForm form, CancellationToken cancellationToken)
        {
            await using var stream = form.QrImage?.OpenReadStream();
            var result = await _mediator.Send(new UpdateTeacherPaymentSettingsCommand(
                form.InstaPayLink ?? string.Empty, stream, form.QrImage?.FileName), cancellationToken);
            return ToResponse(result);
        }

        [HttpGet("instapay-receipts/pending")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> GetPendingInstaPayReceipts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPendingInstaPayReceiptsQuery(page, pageSize), cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("instapay-receipts/{transactionId:guid}/approve")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> ApproveInstaPayReceipt(Guid transactionId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ApproveInstaPayReceiptCommand(transactionId), cancellationToken);
            return ToResponse(result, new { Message = "The payment receipt was approved." });
        }

        [HttpPost("instapay-receipts/{transactionId:guid}/reject")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> RejectInstaPayReceipt(Guid transactionId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RejectInstaPayReceiptCommand(transactionId), cancellationToken);
            return ToResponse(result, new { Message = "The payment receipt was rejected." });
        }

        [HttpPost("cash-payments")]
        public async Task<IActionResult> CollectCash([FromBody] CollectCashPaymentCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }
    }

    public sealed class UpdateTeacherPaymentSettingsForm
    {
        public string? InstaPayLink { get; set; }
        public IFormFile? QrImage { get; set; }
    }
}
