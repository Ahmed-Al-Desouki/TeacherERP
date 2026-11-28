using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public record ResendCenterJoinRequestCommand(Guid RequestId) : IRequest<Result<bool>>;
}
