// موجود (تعديل) — TeacherERP.API/Controllers/UsersController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.Application.Interfaces;
using TeacherERP.API.Extensions;
using TeacherERP.API.Files;
using TeacherERP.Application.Features.Users.Commands.CreateStudent;
using TeacherERP.Application.Features.Users.Commands.VerifyOtp;

namespace TeacherERP.API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IFileStorage _fileStorage;

        public UsersController(IMediator mediator, IFileStorage fileStorage)
        {
            _mediator = mediator;
            _fileStorage = fileStorage;
        }

        [HttpPost("students")]
        [EnableRateLimiting(RateLimitPolicies.AuthOtpSend)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateStudent(
            [FromForm] CreateStudentForm form, CancellationToken cancellationToken)
        {
            string? profileImageUrl = null;
            if (form.ProfileImage is not null)
            {
                var uploaded = await ProfileImageUploader.UploadAsync(form.ProfileImage, _fileStorage, cancellationToken);
                if (uploaded.IsFailure) return ToErrorResponse(uploaded.Error.Code, uploaded.Error.Message);
                profileImageUrl = uploaded.Value!.Url;
            }

            var command = new CreateStudentCommand(form.FullName, form.Email, form.Password, profileImageUrl);
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
        [EnableRateLimiting(RateLimitPolicies.AuthOtpVerify)]
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

    public sealed class CreateStudentForm
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IFormFile? ProfileImage { get; set; }
    }
}
