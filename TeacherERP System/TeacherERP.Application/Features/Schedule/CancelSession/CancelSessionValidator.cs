using FluentValidation;

namespace TeacherERP.Application.Features.Schedule.CancelSession
{
    public class CancelSessionValidator : AbstractValidator<CancelSessionCommand>
    {
        public CancelSessionValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session id is required.");

            RuleFor(x => x.Reason)
                .MaximumLength(300).WithMessage("Reason must not exceed 300 characters.");
        }
    }
}