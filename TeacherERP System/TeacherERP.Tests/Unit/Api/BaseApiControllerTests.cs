using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TeacherERP.API.Controllers;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Unit.Api;

public sealed class BaseApiControllerTests
{
    [Theory]
    [InlineData("Auth.Unauthorized", StatusCodes.Status401Unauthorized)]
    [InlineData("Auth.InvalidRefreshToken", StatusCodes.Status401Unauthorized)]
    [InlineData("Auth.AccountSuspended", StatusCodes.Status403Forbidden)]
    [InlineData("Teacher.InsufficientRole", StatusCodes.Status403Forbidden)]
    [InlineData("Attendance.OutOfRange", StatusCodes.Status403Forbidden)]
    [InlineData("Team.NotFound", StatusCodes.Status404NotFound)]
    [InlineData("Finance.PaymobAdjustmentNotFound", StatusCodes.Status404NotFound)]
    [InlineData("Finance.PaymentAlreadyPending", StatusCodes.Status409Conflict)]
    [InlineData("Finance.ExternalPaymentNotPending", StatusCodes.Status409Conflict)]
    [InlineData("Finance.PaymobPaymentNotPending", StatusCodes.Status409Conflict)]
    [InlineData("Finance.PaymobAdjustmentNotReviewable", StatusCodes.Status409Conflict)]
    [InlineData("Finance.PaymobTransactionNotReviewable", StatusCodes.Status409Conflict)]
    [InlineData("Center.JoinRequestNotPending", StatusCodes.Status409Conflict)]
    [InlineData("GroupAdmin.RequestNotPending", StatusCodes.Status409Conflict)]
    [InlineData("Session.TimeConflict", StatusCodes.Status409Conflict)]
    [InlineData("Team.LimitReached", StatusCodes.Status409Conflict)]
    [InlineData("Center.InvitationExpired", StatusCodes.Status409Conflict)]
    [InlineData("Profile.InvalidPhoneNumber", StatusCodes.Status400BadRequest)]
    [InlineData("Otp.Expired", StatusCodes.Status400BadRequest)]
    [InlineData("Session.NewTimeInPast", StatusCodes.Status400BadRequest)]
    [InlineData("Finance.InvalidPaymobAdjustmentDecision", StatusCodes.Status400BadRequest)]
    [InlineData("Finance.PaymobIntentCreationFailed", StatusCodes.Status503ServiceUnavailable)]
    [InlineData("Team.EmailSendFailed", StatusCodes.Status503ServiceUnavailable)]
    public void ToResponse_MapsProjectErrorsToTheirHttpStatusAndKeepsTheStandardBody(
        string errorCode, int expectedStatus)
    {
        var controller = new TestController();

        var response = Assert.IsType<ObjectResult>(controller.Send(
            Result<string>.Failure(new Error(errorCode, "Sample error."))));

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(errorCode, response.Value!.GetType().GetProperty("errorCode")!.GetValue(response.Value));
        Assert.Equal("Sample error.", response.Value.GetType().GetProperty("errorMessage")!.GetValue(response.Value));
    }

    [Fact]
    public void ToResponse_PreservesConflictDetails()
    {
        var controller = new TestController();
        var details = new[] { "Room A", "Room B" };

        var response = Assert.IsType<ObjectResult>(controller.Send(
            Result<string>.Failure(new Error("Schedule.RoomConflictConfirmationRequired", "Confirm room conflicts."), details)));

        Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        Assert.Same(details, response.Value!.GetType().GetProperty("details")!.GetValue(response.Value));
    }

    [Fact]
    public void ExplicitProtocolStatusOverridePreservesTheStandardErrorBody()
    {
        var response = BaseApiController.CreateErrorResponse(
            "Paymob.InvalidSignature", "Invalid callback signature.", StatusCodes.Status401Unauthorized);

        Assert.Equal(StatusCodes.Status401Unauthorized, response.StatusCode);
        Assert.Equal("Paymob.InvalidSignature", response.Value!.GetType().GetProperty("errorCode")!.GetValue(response.Value));
    }

    private sealed class TestController : BaseApiController
    {
        public IActionResult Send<T>(Result<T> result) => ToResponse(result);
    }
}
