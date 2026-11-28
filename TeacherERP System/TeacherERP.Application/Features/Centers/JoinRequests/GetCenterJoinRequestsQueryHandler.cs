using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class GetCenterJoinRequestsQueryHandler
        : IRequestHandler<GetCenterJoinRequestsQuery, Result<PagedResult<CenterJoinRequestListItem>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICenterMembershipRepository _requests;

        public GetCenterJoinRequestsQueryHandler(ICurrentUserService currentUser, ICenterMembershipRepository requests)
        {
            _currentUser = currentUser;
            _requests = requests;
        }

        public async Task<Result<PagedResult<CenterJoinRequestListItem>>> Handle(
            GetCenterJoinRequestsQuery query, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
                return Result<PagedResult<CenterJoinRequestListItem>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

            return Result<PagedResult<CenterJoinRequestListItem>>.Success(await _requests.GetRequestsForOwnerAsync(
                userId, query.Status, PageRequest.From(query), cancellationToken));
        }
    }
}
