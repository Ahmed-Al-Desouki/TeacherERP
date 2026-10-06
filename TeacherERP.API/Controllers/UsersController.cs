using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Users.Commands.CreateStudent;
using TeacherERP.Application.Features.Users.Commands.PromoteToAdmin;
using TeacherERP.Application.Features.Users.Commands.PromoteToSuperAdmin;
using TeacherERP.Application.Features.Users.Commands.VerifyOtp;

namespace TeacherERP.API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("students")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            // استخدام الدالة الموحدة مع تمرير بيانات النجاح المخصصة
            return ToResponse(result, new
            {
                StudentId = result.Value,
                Message = "Student created successfully. OTP sent to email."
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result, new { Message = "Email verified successfully." });
        }
    }
}