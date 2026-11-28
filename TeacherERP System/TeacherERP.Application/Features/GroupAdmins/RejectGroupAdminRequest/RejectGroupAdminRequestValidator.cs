// جديد — TeacherERP.Application/Features/GroupAdmins/RejectGroupAdminRequest/RejectGroupAdminRequestValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.GroupAdmins.RejectGroupAdminRequest
{
    public class RejectGroupAdminRequestValidator : AbstractValidator<RejectGroupAdminRequestCommand>
    {
        public RejectGroupAdminRequestValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty().WithMessage("Request id is required.");

            RuleFor(x => x.Note)
                .MaximumLength(300).WithMessage("Note must not exceed 300 characters.");
        }
    }
}