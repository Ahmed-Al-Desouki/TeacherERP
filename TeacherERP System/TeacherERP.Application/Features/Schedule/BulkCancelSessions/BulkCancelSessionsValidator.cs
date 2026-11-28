using FluentValidation;
using TeacherERP.Application.Scheduling;

namespace TeacherERP.Application.Features.Schedule.BulkCancelSessions
{
    public class BulkCancelSessionsValidator : AbstractValidator<BulkCancelSessionsCommand>
    {
        public BulkCancelSessionsValidator()
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

            RuleFor(x => x.Reason)
                .MaximumLength(300).WithMessage("Reason must not exceed 300 characters.");
        }
    }
}