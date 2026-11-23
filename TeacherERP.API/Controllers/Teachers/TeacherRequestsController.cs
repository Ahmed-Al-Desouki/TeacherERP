// جديد — TeacherERP.API/Controllers/TeacherRequestsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.API.Extensions;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.TeacherRequests.Review;
using TeacherERP.Application.Features.TeacherRequests.Status;
using TeacherERP.Application.Features.TeacherRequests.Submit;
using TeacherERP.Application.Features.TeacherRequests.Verify;

namespace TeacherERP.API.Controllers
{
    // طلب مدرس للانضمام من صفحة الدخول: صاحب الطلب (بلا حساب) يرسل بياناته ويؤكد إيميله فيظهر له QR،
    // والمدرس الذي يمسحه يعاين ثم يقبل أو يرفض. السر يُرسل في الـ Body (لا في الـ URL) حتى لا يظهر في اللوجات
    public class TeacherRequestsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TeacherRequestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---------- صاحب الطلب (بلا حساب) ----------

        [AllowAnonymous]
        [HttpPost("submit")]
        [EnableRateLimiting(RateLimitPolicies.TeacherRequestSubmit)]
        public async Task<IActionResult> Submit([FromBody] SubmitTeacherRequestCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        [AllowAnonymous]
        [HttpPost("verify")]
        [EnableRateLimiting(RateLimitPolicies.AuthOtpVerify)]
        public async Task<IActionResult> Verify([FromBody] VerifyTeacherRequestCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        [AllowAnonymous]
        [HttpPost("status")]
        [EnableRateLimiting(RateLimitPolicies.AuthOtpVerify)]
        public async Task<IActionResult> GetStatus([FromBody] GetTeacherRequestStatusQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return ToResponse(result);
        }

        // ---------- المدرس الذي مسح الـ QR ----------

        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [HttpPost("preview")]
        [EnableRateLimiting(RateLimitPolicies.TeacherRequestReview)]
        public async Task<IActionResult> Preview([FromBody] PreviewTeacherRequestQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return ToResponse(result);
        }

        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [HttpPost("approve")]
        [EnableRateLimiting(RateLimitPolicies.TeacherRequestReview)]
        public async Task<IActionResult> Approve([FromBody] ApproveTeacherRequestCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [HttpPost("reject")]
        [EnableRateLimiting(RateLimitPolicies.TeacherRequestReview)]
        public async Task<IActionResult> Reject([FromBody] RejectTeacherRequestCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result, new { Message = "The request was rejected." });
        }
    }
}
