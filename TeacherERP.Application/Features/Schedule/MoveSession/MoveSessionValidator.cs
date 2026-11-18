using FluentValidation;

namespace TeacherERP.Application.Features.Schedule.MoveSession;

public sealed class MoveSessionValidator : AbstractValidator<MoveSessionCommand>
{
    public MoveSessionValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.TargetGroupId).NotEmpty();
        RuleFor(x => x.NewStartTime).NotEqual(default(DateTime));
        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(15, 480)
            .When(x => x.DurationMinutes.HasValue);
    }
}
