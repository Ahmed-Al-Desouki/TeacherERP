// جديد — TeacherERP.Application/Features/Students/GetStudents/GetStudentsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Students.GetStudents
{
    public class GetStudentsValidator : AbstractValidator<GetStudentsQuery>
    {
        public GetStudentsValidator()
        {
            Include(new PagedRequestRules<GetStudentsQuery>());

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Status is invalid.");
        }
    }
}
