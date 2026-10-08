using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device id is required.")
                .MaximumLength(128).WithMessage("Device id must not exceed 128 characters.");

            RuleFor(x => x.DeviceInfo)
                .NotEmpty().WithMessage("Device information is required for security tracking.")
                .MaximumLength(256).WithMessage("Device information must not exceed 256 characters.");
        }
    }
}