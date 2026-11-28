using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class GetMyCenterJoinRequestsValidator : AbstractValidator<GetMyCenterJoinRequestsQuery>
    {
        public GetMyCenterJoinRequestsValidator() => Include(new PagedRequestRules<GetMyCenterJoinRequestsQuery>());
    }
}
