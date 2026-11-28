// جديد — TeacherERP.Application/Features/TeacherRequests/Verify/VerifyTeacherRequestValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.TeacherRequests.Verify
{
    public class VerifyTeacherRequestValidator : AbstractValidator<VerifyTeacherRequestCommand>
    {
        public VerifyTeacherRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("The verification code is required.")
                .Matches(@"^\d{6}$").WithMessage("The verification code must be 6 digits.");
        }
    }
}
