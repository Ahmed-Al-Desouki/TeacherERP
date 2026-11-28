// جديد — TeacherERP.Application/Features/Users/Commands/Auth/ActivateAccount/ActivateAccountValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount
{
    public class ActivateAccountValidator : AbstractValidator<ActivateAccountCommand>
    {
        public ActivateAccountValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required.")
                .Length(6).WithMessage("OTP code must be exactly 6 digits.")
                .Matches("^[0-9]+$").WithMessage("OTP code must contain only numbers.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device id is required.")
                .MaximumLength(128).WithMessage("Device id must not exceed 128 characters.");

            RuleFor(x => x.DeviceInfo)
                .NotEmpty().WithMessage("Device information is required for security tracking.")
                .MaximumLength(256).WithMessage("Device information must not exceed 256 characters.");
        }
    }
}