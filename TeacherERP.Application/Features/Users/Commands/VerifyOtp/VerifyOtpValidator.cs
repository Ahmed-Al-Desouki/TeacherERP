using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.VerifyOtp
{
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student ID is required.");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required.")
                .Length(6).WithMessage("OTP code must be exactly 6 digits.");
        }
    }
}