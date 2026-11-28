using FluentValidation;

namespace TeacherERP.Application.Features.Schedule.RescheduleSession
{
    public class RescheduleSessionValidator : AbstractValidator<RescheduleSessionCommand>
    {
        public RescheduleSessionValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session id is required.");

            RuleFor(x => x.NewStartTime)
                .NotEqual(default(DateTime)).WithMessage("New start time is required.");

            RuleFor(x => x.DurationMinutes)
                .InclusiveBetween(15, 480).When(x => x.DurationMinutes.HasValue)
                .WithMessage("Duration must be between 15 and 480 minutes.");
        }
    }
}