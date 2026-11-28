using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/paymob/webhooks")]
public sealed class PaymobWebhookController : ControllerBase
{
    private readonly IPaymobPaymentService _payments;

    public PaymobWebhookController(IPaymobPaymentService payments) => _payments = payments;

    [HttpPost("transaction")]
    [Consumes("application/json")]
    [RequestSizeLimit(262_144)]
    public async Task<IActionResult> Transaction(CancellationToken cancellationToken)
    {
        JsonDocument document;
        try
        {
            document = await JsonDocument.ParseAsync(Request.Body, cancellationToken: cancellationToken);
        }
        catch (JsonException)
        {
            return BaseApiController.CreateErrorResponse("Paymob.InvalidPayload", "The callback payload is invalid.");
        }

        using (document)
        {
            var outcome = await _payments.HandleTransactionCallbackAsync(
                document.RootElement, Request.Query["hmac"].FirstOrDefault(), cancellationToken);
            return outcome switch
            {
                PaymobWebhookOutcome.InvalidSignature => BaseApiController.CreateErrorResponse(
                    "Paymob.InvalidSignature", "The callback signature is invalid.", StatusCodes.Status401Unauthorized),
                PaymobWebhookOutcome.InvalidPayload => BaseApiController.CreateErrorResponse(
                    "Paymob.InvalidPayload", "The callback payload is invalid."),
                _ => Ok(new { received = true, duplicate = outcome == PaymobWebhookOutcome.Duplicate })
            };
        }
    }
}
