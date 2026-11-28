using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public record GetCenterJoinRequestsQuery(
        int Page = 1, int PageSize = PageRequest.DefaultPageSize, CenterJoinRequestStatus? Status = null)
        : IPagedRequest, IRequest<Result<PagedResult<CenterJoinRequestListItem>>>;
}
