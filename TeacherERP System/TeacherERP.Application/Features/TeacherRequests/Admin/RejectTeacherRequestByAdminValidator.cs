// جديد — TeacherERP.Application/Features/TeacherRequests/Admin/RejectTeacherRequestByAdminValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    public class RejectTeacherRequestByAdminValidator : AbstractValidator<RejectTeacherRequestByAdminCommand>
    {
        public RejectTeacherRequestByAdminValidator()
        {
            RuleFor(x => x.Reason)
                .MaximumLength(300).WithMessage("Reason must not exceed 300 characters.");
        }
    }
}
