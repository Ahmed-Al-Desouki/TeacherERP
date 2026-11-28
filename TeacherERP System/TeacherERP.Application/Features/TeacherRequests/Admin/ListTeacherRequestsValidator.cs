// جديد — TeacherERP.Application/Features/TeacherRequests/Admin/ListTeacherRequestsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    public class ListTeacherRequestsValidator : AbstractValidator<ListTeacherRequestsQuery>
    {
        public ListTeacherRequestsValidator()
        {
            Include(new PagedRequestRules<ListTeacherRequestsQuery>());

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Status is invalid.");
        }
    }
}
