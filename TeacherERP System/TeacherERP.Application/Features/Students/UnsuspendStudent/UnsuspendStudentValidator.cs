// جديد — TeacherERP.Application/Features/Students/UnsuspendStudent/UnsuspendStudentValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Students.UnsuspendStudent
{
    public class UnsuspendStudentValidator : AbstractValidator<UnsuspendStudentCommand>
    {
        public UnsuspendStudentValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student id is required.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required.")
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
        }
    }
}