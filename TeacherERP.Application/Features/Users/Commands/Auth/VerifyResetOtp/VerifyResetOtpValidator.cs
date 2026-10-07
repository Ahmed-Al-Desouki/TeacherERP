using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp
{
    public class VerifyResetOtpValidator : AbstractValidator<VerifyResetOtpCommand>
    {
        public VerifyResetOtpValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required.")
                .Length(6).WithMessage("OTP code must be exactly 6 digits.")
                .Matches("^[0-9]+$").WithMessage("OTP code must contain only numbers.");
        }
    }
}