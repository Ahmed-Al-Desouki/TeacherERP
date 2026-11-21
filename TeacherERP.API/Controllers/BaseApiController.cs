using Microsoft.AspNetCore.Mvc;
using TeacherERP.Domain.Shared;

namespace TeacherERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult ToResponse<T>(Result<T> result, object? successData = null)
        {
            if (result.IsSuccess)
            {
                return successData != null ? Ok(successData) : Ok(result.Value);
            }

            // تحويل الـ Error الموحد إلى استجابة خطأ واضحة ومنظمة
            if (result.Error.Code == "Schedule.RoomConflictConfirmationRequired")
            {
                return Conflict(new
                {
                    errorCode = result.Error.Code,
                    errorMessage = result.Error.Message,
                    details = result.Details
                });
            }

            if (result.Error.Code is "Finance.IdempotencyKeyReused" or "Finance.PaymobPaymentAlreadyPending")
                return Conflict(new { errorCode = result.Error.Code, errorMessage = result.Error.Message });

            if (result.Error.Code == "Finance.PaymobAdjustmentNotFound")
                return NotFound(new { errorCode = result.Error.Code, errorMessage = result.Error.Message });

            if (result.Error.Code is "Finance.PaymobAdjustmentAlreadyResolved"
                or "Finance.PaymobAdjustmentResolveInOrder"
                or "Finance.PaymobTransactionNotReviewable")
                return Conflict(new { errorCode = result.Error.Code, errorMessage = result.Error.Message });

            if (result.Error.Code is "Finance.InvalidPaymobAdjustmentDecision"
                or "Finance.PaymobAdjustmentExceedsRefund"
                or "Finance.RejectedPaymobAdjustmentHasAmounts"
                or "Finance.InvalidPaymobCommissionReversal")
                return BadRequest(new { errorCode = result.Error.Code, errorMessage = result.Error.Message });

            if (result.Error.Code is "Finance.PaymobShopifyIntegrationNotSupported"
                or "Finance.PaymobPaymentMethodUnsupported"
                or "Finance.PaymobBillingProfileIncomplete")
                return BadRequest(new { errorCode = result.Error.Code, errorMessage = result.Error.Message });

            if (result.Error.Code.StartsWith("Finance.Paymob", StringComparison.Ordinal))
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { errorCode = result.Error.Code, errorMessage = result.Error.Message });

            return BadRequest(new { errorCode = result.Error.Code, errorMessage = result.Error.Message });
        }
    }
}
