using FluentValidation;

namespace TeacherERP.Application.Features.Sessions.Logout
{
    public class LogoutValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .MaximumLength(256).WithMessage("Refresh token is invalid.");
        }
    }
}