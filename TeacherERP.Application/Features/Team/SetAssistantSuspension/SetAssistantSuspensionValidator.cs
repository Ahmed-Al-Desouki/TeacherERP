// جديد — TeacherERP.Application/Features/Team/SetAssistantSuspension/SetAssistantSuspensionValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Team.SetAssistantSuspension
{
    public class SetAssistantSuspensionValidator : AbstractValidator<SetAssistantSuspensionCommand>
    {
        public SetAssistantSuspensionValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required.");
        }
    }
}