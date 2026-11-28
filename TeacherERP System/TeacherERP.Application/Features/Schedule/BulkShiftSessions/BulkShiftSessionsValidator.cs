using FluentValidation;
using TeacherERP.Application.Scheduling;

namespace TeacherERP.Application.Features.Schedule.BulkShiftSessions
{
    public class BulkShiftSessionsValidator : AbstractValidator<BulkShiftSessionsCommand>
    {
        public BulkShiftSessionsValidator()
        {
            RuleFor(x => x.From)
                .NotEqual(default(DateTime)).WithMessage("From is required.");

            RuleFor(x => x.To)
                .NotEqual(default(DateTime)).WithMessage("To is required.");

            RuleFor(x => x)
                .Must(x => x.To > x.From && (x.To - x.From).TotalDays <= SchedulePolicy.MaxSessionsQueryDays)
                .WithMessage($"The range must be positive and at most {SchedulePolicy.MaxSessionsQueryDays} days.");

            RuleFor(x => x.GroupIds)
                .Must(ids => ids is null || ids.Count <= 100)
                .WithMessage("At most 100 groups can be selected.");

            RuleFor(x => x.ShiftMinutes)
                .NotEqual(0).WithMessage("Shift must not be zero.")
                .InclusiveBetween(-720, 720).WithMessage("Shift must be within 12 hours.");
        }
    }
}