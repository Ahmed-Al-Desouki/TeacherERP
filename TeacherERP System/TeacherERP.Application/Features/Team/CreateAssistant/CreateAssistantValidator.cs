// جديد — TeacherERP.Application/Features/Team/CreateAssistant/CreateAssistantValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Team.CreateAssistant
{
    public class CreateAssistantValidator : AbstractValidator<CreateAssistantCommand>
    {
        public CreateAssistantValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");
        }
    }
}