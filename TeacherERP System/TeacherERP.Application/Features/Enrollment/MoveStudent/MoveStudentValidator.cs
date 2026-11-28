using FluentValidation;

namespace TeacherERP.Application.Features.Enrollment.MoveStudent;

public sealed class MoveStudentValidator : AbstractValidator<MoveStudentCommand>
{
    public MoveStudentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
        RuleFor(command => command.TargetGroupId).NotEmpty();
    }
}
