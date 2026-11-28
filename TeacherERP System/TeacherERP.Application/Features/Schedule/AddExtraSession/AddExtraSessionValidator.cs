using FluentValidation;

namespace TeacherERP.Application.Features.Schedule.AddExtraSession
{
    public class AddExtraSessionValidator : AbstractValidator<AddExtraSessionCommand>
    {
        public AddExtraSessionValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Group id is required.");

            RuleFor(x => x.StartTime)
                .NotEqual(default(DateTime)).WithMessage("Start time is required.");

            RuleFor(x => x.DurationMinutes)
                .InclusiveBetween(15, 480).WithMessage("Duration must be between 15 and 480 minutes.");
        }
    }
}