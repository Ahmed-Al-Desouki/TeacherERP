using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public record GetMyCenterJoinRequestsQuery(int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IPagedRequest, IRequest<Result<PagedResult<CenterJoinRequestListItem>>>;
}
