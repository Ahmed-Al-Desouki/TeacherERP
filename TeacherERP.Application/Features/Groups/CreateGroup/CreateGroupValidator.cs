using FluentValidation;

namespace TeacherERP.Application.Features.Groups.CreateGroup
{
    public class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
    {
        private const int MaxSlotsPerGroup = 14;

        public CreateGroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Group name is required.")
                .MaximumLength(150).WithMessage("Group name must not exceed 150 characters.");

            RuleFor(x => x.GradeLevel)
                .NotEmpty().WithMessage("Grade level is required.")
                .MaximumLength(50).WithMessage("Grade level must not exceed 50 characters.");

            RuleFor(x => x.CenterId)
                .NotEmpty().WithMessage("Center id is required.");

            RuleFor(x => x.Room)
                .MaximumLength(50).WithMessage("Room must not exceed 50 characters.");

            RuleFor(x => x.Capacity)
                .InclusiveBetween(1, 500).When(x => x.Capacity.HasValue)
                .WithMessage("Capacity must be between 1 and 500.");

            RuleFor(x => x.WhatsAppGroupLink)
                .MaximumLength(500).WithMessage("WhatsApp link must not exceed 500 characters.")
                .Must(BeHttpUrl).WithMessage("WhatsApp link must be a valid http or https URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.WhatsAppGroupLink));

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

        private static bool BeHttpUrl(string? value)
            => Uri.TryCreate(value, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);

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