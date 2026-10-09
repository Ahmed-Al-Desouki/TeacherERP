using FluentValidation;

namespace TeacherERP.Application.Features.Sessions.RevokeSession
{
    public class RevokeSessionValidator : AbstractValidator<RevokeSessionCommand>
    {
        public RevokeSessionValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session id is required.");
        }
    }
}