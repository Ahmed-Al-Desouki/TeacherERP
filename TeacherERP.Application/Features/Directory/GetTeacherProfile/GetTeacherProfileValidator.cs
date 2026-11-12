// جديد — TeacherERP.Application/Features/Directory/GetTeacherProfile/GetTeacherProfileValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Directory.GetTeacherProfile
{
    public class GetTeacherProfileValidator : AbstractValidator<GetTeacherProfileQuery>
    {
        public GetTeacherProfileValidator()
        {
            Include(new PagedRequestRules<GetTeacherProfileQuery>());

            RuleFor(x => x.TeacherId)
                .NotEmpty().WithMessage("Teacher id is required.");

            RuleFor(x => x.GradeLevel)
                .MaximumLength(50).WithMessage("Grade level must not exceed 50 characters.");
        }
    }
}
