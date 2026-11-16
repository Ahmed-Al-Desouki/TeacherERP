// جديد — TeacherERP.Application/Features/TeacherRequests/Review/PreviewTeacherRequestValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    public class PreviewTeacherRequestValidator : AbstractValidator<PreviewTeacherRequestQuery>
    {
        public PreviewTeacherRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.")
                .MaximumLength(100).WithMessage("Token is invalid.");
        }
    }
}
