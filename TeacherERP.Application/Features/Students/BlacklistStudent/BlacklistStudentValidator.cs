// جديد — TeacherERP.Application/Features/Students/BlacklistStudent/BlacklistStudentValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Students.BlacklistStudent
{
    public class BlacklistStudentValidator : AbstractValidator<BlacklistStudentCommand>
    {
        public BlacklistStudentValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student id is required.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required.")
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
        }
    }
}