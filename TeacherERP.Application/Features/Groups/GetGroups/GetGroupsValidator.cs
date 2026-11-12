// جديد — TeacherERP.Application/Features/Groups/GetGroups/GetGroupsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Groups.GetGroups
{
    public class GetGroupsValidator : AbstractValidator<GetGroupsQuery>
    {
        public GetGroupsValidator()
        {
            Include(new PagedRequestRules<GetGroupsQuery>());
        }
    }
}
