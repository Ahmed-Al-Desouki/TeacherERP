using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

            if (result.Details is not null)
            {
                return StatusCode(ResolveErrorStatusCode(result.Error.Code), new
                {
                    errorCode = result.Error.Code,
                    errorMessage = result.Error.Message,
                    details = result.Details
                });
            }

            return ToErrorResponse(result.Error.Code, result.Error.Message);
        }

        protected IActionResult ToErrorResponse(string errorCode, string errorMessage)
            => CreateErrorResponse(errorCode, errorMessage);

        public static ObjectResult CreateErrorResponse(
            string errorCode, string errorMessage, int? statusCodeOverride = null)
            => new(new { errorCode, errorMessage })
            {
                StatusCode = statusCodeOverride ?? ResolveErrorStatusCode(errorCode)
            };

        private static int ResolveErrorStatusCode(string code)
        {
            if (code is "Auth.Unauthorized" or "Auth.ActivationRequired"
                or "Auth.InvalidRefreshToken" or "Auth.InvalidResetToken")
            {
                return StatusCodes.Status401Unauthorized;
            }

            if (code is "Auth.AccountSuspended" or "Teacher.NoMembership"
                or "Teacher.Forbidden" or "Teacher.InsufficientRole"
                or "Teacher.MembershipSuspended" or "Teacher.Suspended"
                or "Attendance.Suspended" or "Attendance.SelfScanDisabled"
                or "Attendance.MockLocation" or "Attendance.OutOfRange"
                or "Attendance.StudentNotInGroup" or "Finance.StudentOnly"
                or "GroupAdmin.NotAdmin" or "GroupAdmin.NotEnrolled"
                or "GroupAdmin.CannotRecordSelf" or "GroupAdmin.StudentNotActive"
                or "Enrollment.NotEnrolled")
            {
                return StatusCodes.Status403Forbidden;
            }

            if (code.EndsWith(".Forbidden", StringComparison.Ordinal)
                || code.EndsWith(".NotAllowed", StringComparison.Ordinal)
                || code.EndsWith(".OnlyOwnFiles", StringComparison.Ordinal)
                || code.EndsWith(".OutsideAdminWindow", StringComparison.Ordinal)
                || code.EndsWith(".ReviewerNotAllowed", StringComparison.Ordinal))
            {
                return StatusCodes.Status403Forbidden;
            }

            if (code.Contains("NotFound", StringComparison.Ordinal))
            {
                return StatusCodes.Status404NotFound;
            }

            if (code.Contains("Already", StringComparison.Ordinal)
                || code.Contains("Conflict", StringComparison.Ordinal)
                || code.Contains("IdempotencyKeyReused", StringComparison.Ordinal)
                || code is "Finance.PaymobAdjustmentResolveInOrder"
                or "Finance.PreviousDebtMustBeSettled"
                or "Finance.ExternalPaymentNotPending"
                or "Finance.PaymobPaymentNotPending"
                or "Finance.PaymobAdjustmentNotReviewable"
                or "Finance.PaymobTransactionNotReviewable"
                or "Center.JoinRequestNotPending"
                or "GroupAdmin.RequestNotPending"
                or "Attendance.NoSessionNow" or "Attendance.SessionNotStarted"
                or "Attendance.CorrectionWindowClosed" or "Attendance.NotPending"
                || code.EndsWith(".NotPending", StringComparison.Ordinal)
                || code.EndsWith(".NotReviewable", StringComparison.Ordinal)
                || code.EndsWith(".QuotaReached", StringComparison.Ordinal)
                || code.EndsWith(".NameTaken", StringComparison.Ordinal)
                || code.EndsWith(".LocationAlreadyOwned", StringComparison.Ordinal)
                || code.EndsWith(".LimitReached", StringComparison.Ordinal)
                || code.EndsWith(".JoinRequestResendTooSoon", StringComparison.Ordinal)
                || code is "Session.Cancelled" or "Session.InPast"
                or "Enrollment.GroupFull" or "Group.Inactive"
                or "Center.InvitationExpired" or "TeacherRequest.Expired")
            {
                return StatusCodes.Status409Conflict;
            }

            if (code is "Finance.InvalidPaymobAdjustmentDecision"
                or "Finance.PaymobAdjustmentExceedsRefund"
                or "Finance.RejectedPaymobAdjustmentHasAmounts"
                or "Finance.InvalidPaymobCommissionReversal"
                or "Finance.PaymobShopifyIntegrationNotSupported"
                or "Finance.PaymobPaymentMethodUnsupported"
                or "Finance.PaymobBillingProfileIncomplete")
            {
                return StatusCodes.Status400BadRequest;
            }

            if (code.StartsWith("Finance.Paymob", StringComparison.Ordinal)
                || code is "Administration.EmailSendFailed" or "Team.EmailSendFailed"
                or "Center.NotificationUnavailable" or "Notifications.ShareUnavailable"
                || code is "FileStorage.NotConfigured" or "FileStorage.UploadFailed" or "FileStorage.DeleteFailed")
            {
                return StatusCodes.Status503ServiceUnavailable;
            }

            return StatusCodes.Status400BadRequest;
        }
    }
}
