// جديد — TeacherERP.Application/Features/TeacherRequests/Review/RejectTeacherRequestValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    public class RejectTeacherRequestValidator : AbstractValidator<RejectTeacherRequestCommand>
    {
        public RejectTeacherRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.")
                .MaximumLength(100).WithMessage("Token is invalid.");

            RuleFor(x => x.Reason)
                .MaximumLength(300).WithMessage("Reason must not exceed 300 characters.");
        }
    }
}
