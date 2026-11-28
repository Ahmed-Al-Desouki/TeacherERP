// جديد — TeacherERP.Application/Features/TeacherRequests/Review/ApproveTeacherRequestValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    public class ApproveTeacherRequestValidator : AbstractValidator<ApproveTeacherRequestCommand>
    {
        public ApproveTeacherRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.")
                .MaximumLength(100).WithMessage("Token is invalid.");
        }
    }
}
