// جديد — TeacherERP.Application/Features/GroupAdmins/GetGroupAdminRequests/GetGroupAdminRequestsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests
{
    public class GetGroupAdminRequestsValidator : AbstractValidator<GetGroupAdminRequestsQuery>
    {
        public GetGroupAdminRequestsValidator()
        {
            Include(new PagedRequestRules<GetGroupAdminRequestsQuery>());

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Status is invalid.");
        }
    }
}