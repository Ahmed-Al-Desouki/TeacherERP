using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class GetCenterJoinRequestsValidator : AbstractValidator<GetCenterJoinRequestsQuery>
    {
        public GetCenterJoinRequestsValidator() => Include(new PagedRequestRules<GetCenterJoinRequestsQuery>());
    }
}
