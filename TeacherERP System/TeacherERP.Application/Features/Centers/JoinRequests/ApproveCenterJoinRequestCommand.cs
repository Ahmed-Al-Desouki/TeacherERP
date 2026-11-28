using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public record ApproveCenterJoinRequestCommand(Guid RequestId) : IRequest<Result<bool>>;
}
