using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public record CancelCenterJoinRequestCommand(Guid RequestId) : IRequest<Result<bool>>;
}
