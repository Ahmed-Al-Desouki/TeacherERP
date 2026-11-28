using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public record JoinCenterCommand(Guid CenterId) : IRequest<Result<CenterJoinRequestListItem>>;
}
