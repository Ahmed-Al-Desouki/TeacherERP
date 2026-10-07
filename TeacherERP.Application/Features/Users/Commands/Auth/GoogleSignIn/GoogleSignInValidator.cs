using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn
{
    public class GoogleSignInValidator : AbstractValidator<GoogleSignInCommand>
    {
        public GoogleSignInValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google ID token is required.");

            RuleFor(x => x.DeviceInfo)
                .NotEmpty().WithMessage("Device information is required for security tracking.");
        }
    }
}