// جديد — TeacherERP.Application/Features/Students/UnblacklistStudent/UnblacklistStudentValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Students.UnblacklistStudent
{
    public class UnblacklistStudentValidator : AbstractValidator<UnblacklistStudentCommand>
    {
        public UnblacklistStudentValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student id is required.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required.")
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
        }
    }
}