// جديد — TeacherERP.Application/Features/GroupAdmins/ApproveGroupAdminRequest/ApproveGroupAdminRequestValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.GroupAdmins.ApproveGroupAdminRequest
{
    public class ApproveGroupAdminRequestValidator : AbstractValidator<ApproveGroupAdminRequestCommand>
    {
        public ApproveGroupAdminRequestValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty().WithMessage("Request id is required.");
        }
    }
}