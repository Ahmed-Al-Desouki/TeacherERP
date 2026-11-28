// جديد — TeacherERP.Application/Features/Enrollment/RejectRequest/RejectRequestValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Enrollment.RejectRequest
{
    public class RejectRequestValidator : AbstractValidator<RejectRequestCommand>
    {
        public RejectRequestValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty().WithMessage("Request id is required.");

            RuleFor(x => x.Note)
                .MaximumLength(300).WithMessage("Note must not exceed 300 characters.");
        }
    }
}