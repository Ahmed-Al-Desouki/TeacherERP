using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Users.Commands.CreateStudent;
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

            // تعديل: الـ result.Value هنا هو الإيميل (string) وليس الـ Id
            // تعديل: الرسالة يجب أن توضح أن الحساب معلق على التفعيل
            return ToResponse(result, new
            {
                Email = result.Value,
                Message = "Registration initiated. OTP sent to email. Please verify to create your account."
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            // تعديل: الـ result.Value هنا أصبح الـ Guid الخاص بالطالب بعد ما اتكارييت فعلياً
            return ToResponse(result, new
            {
                Message = "Email verified and account created successfully."
            });
        }
    }
}