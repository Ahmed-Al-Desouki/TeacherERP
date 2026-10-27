// جديد — TeacherERP.Application/Features/Team/RemoveAssistant/RemoveAssistantValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Team.RemoveAssistant
{
    public class RemoveAssistantValidator : AbstractValidator<RemoveAssistantCommand>
    {
        public RemoveAssistantValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required.");
        }
    }
}