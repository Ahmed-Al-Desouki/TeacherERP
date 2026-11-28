using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn
{
    public class GoogleSignInValidator : AbstractValidator<GoogleSignInCommand>
    {
        public GoogleSignInValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google ID token is required.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device id is required.")
                .MaximumLength(128).WithMessage("Device id must not exceed 128 characters.");

            RuleFor(x => x.DeviceInfo)
                .NotEmpty().WithMessage("Device information is required for security tracking.")
                .MaximumLength(256).WithMessage("Device information must not exceed 256 characters.");
        }
    }
}