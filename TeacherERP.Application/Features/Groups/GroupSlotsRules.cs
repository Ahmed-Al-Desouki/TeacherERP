// جديد — TeacherERP.Application/Features/Groups/GroupSlotsRules.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Groups
{
    // قواعد الجدول الأسبوعي في مكان واحد، تُضمَّن في validators الإنشاء والتعديل
    public class GroupSlotsRules<T> : AbstractValidator<T> where T : IGroupSchedule
    {
        private const int MaxSlotsPerGroup = 14;

        public GroupSlotsRules()
        {
            RuleFor(x => x.Slots)
                .NotEmpty().WithMessage("At least one weekly session is required.");

            RuleFor(x => x.Slots)
                .Must(slots => slots is null || slots.Count <= MaxSlotsPerGroup)
                .WithMessage($"A group can have at most {MaxSlotsPerGroup} weekly sessions.");

            RuleForEach(x => x.Slots).ChildRules(slot =>
            {
                slot.RuleFor(s => s.Day)
                    .IsInEnum().WithMessage("Day is invalid.");

                slot.RuleFor(s => s.StartTime)
                    .Must(time => time >= TimeSpan.Zero && time < TimeSpan.FromHours(24))
                    .WithMessage("Start time must be within the day.");

                slot.RuleFor(s => s.DurationMinutes)
                    .InclusiveBetween(15, 480).WithMessage("Duration must be between 15 and 480 minutes.");

                slot.RuleFor(s => s)
                    .Must(s => s.StartTime + TimeSpan.FromMinutes(s.DurationMinutes) <= TimeSpan.FromHours(24))
                    .WithMessage("A session must end within the same day.");
            });

            RuleFor(x => x.Slots)
                .Must(HaveNoOverlaps)
                .WithMessage("Weekly sessions of the same group must not overlap.")
                .When(x => x.Slots is not null);
        }

        private static bool HaveNoOverlaps(List<GroupSlotInput> slots)
        {
            foreach (var day in slots.GroupBy(s => s.Day))
            {
                var ordered = day.OrderBy(s => s.StartTime).ToList();

                for (var i = 1; i < ordered.Count; i++)
                {
                    var previousEnd = ordered[i - 1].StartTime + TimeSpan.FromMinutes(ordered[i - 1].DurationMinutes);
                    if (ordered[i].StartTime < previousEnd)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}