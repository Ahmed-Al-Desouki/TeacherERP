using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .MaximumLength(256).WithMessage("Refresh token is invalid.");
        }
    }
}