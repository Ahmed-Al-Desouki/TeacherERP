// جديد — TeacherERP.Application/Features/Team/ResendWelcome/ResendWelcomeValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Team.ResendWelcome
{
    public class ResendWelcomeValidator : AbstractValidator<ResendWelcomeCommand>
    {
        public ResendWelcomeValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required.");
        }
    }
}