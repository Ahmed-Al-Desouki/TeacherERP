using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.API.Extensions;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Finance.GetMyPaymentOptions;
using TeacherERP.Application.Features.Finance.SubmitInstaPayReceipt;
using TeacherERP.Application.Finance;
using TeacherERP.Application.SessionContent;

namespace TeacherERP.API.Controllers;

[Authorize(Policy = AuthPolicies.TenantMember)]
public sealed class StudentPaymentsController : BaseApiController
{
    private readonly IMediator _mediator;
    public StudentPaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("sessions/{sessionId:guid}/payment-options")]
    public async Task<IActionResult> GetPaymentOptions(Guid sessionId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyPaymentOptionsQuery(sessionId), cancellationToken);
        return ToResponse(result);
    }

    [HttpPost("receipts")]
    [EnableRateLimiting(RateLimitPolicies.ContentUpload)]
    [RequestSizeLimit(SessionContentPolicy.MaxRequestBytes)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SubmitReceipt(
        [FromForm] SubmitInstaPayReceiptForm form, CancellationToken cancellationToken)
    {
        if (form.Receipt is null)
            return BadRequest(new { errorCode = FinanceErrors.PaymentReceiptRequired.Code, errorMessage = FinanceErrors.PaymentReceiptRequired.Message });

        await using var stream = form.Receipt.OpenReadStream();
        var result = await _mediator.Send(new SubmitInstaPayReceiptCommand(
            form.SessionId, form.Type, form.Amount, stream, form.Receipt.FileName), cancellationToken);
        if (result.IsFailure) return ToResponse(result);
        return ToResponse(result, new { Message = "The receipt was submitted for teacher review.", TransactionId = result.Value });
    }
}

public sealed class SubmitInstaPayReceiptForm
{
    public Guid SessionId { get; set; }
    public TeacherERP.Domain.Entities.PaymentType Type { get; set; }
    public decimal Amount { get; set; }
    public IFormFile? Receipt { get; set; }
}
