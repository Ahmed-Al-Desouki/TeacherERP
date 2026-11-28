using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class GetMyCenterJoinRequestsQueryHandler
        : IRequestHandler<GetMyCenterJoinRequestsQuery, Result<PagedResult<CenterJoinRequestListItem>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITenantRepository _tenants;
        private readonly ICenterMembershipRepository _requests;

        public GetMyCenterJoinRequestsQueryHandler(
            ICurrentUserService currentUser, ITenantRepository tenants, ICenterMembershipRepository requests)
        {
            _currentUser = currentUser;
            _tenants = tenants;
            _requests = requests;
        }

        public async Task<Result<PagedResult<CenterJoinRequestListItem>>> Handle(
            GetMyCenterJoinRequestsQuery query, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
                return Result<PagedResult<CenterJoinRequestListItem>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

            var tenant = await _tenants.GetByOwnerIdAsync(userId, cancellationToken);
            if (tenant is null)
                return Result<PagedResult<CenterJoinRequestListItem>>.Failure(CenterErrors.TeacherSpaceNotFound);

            return Result<PagedResult<CenterJoinRequestListItem>>.Success(await _requests.GetRequestsForTeacherAsync(
                tenant.Id, PageRequest.From(query), cancellationToken));
        }
    }
}
