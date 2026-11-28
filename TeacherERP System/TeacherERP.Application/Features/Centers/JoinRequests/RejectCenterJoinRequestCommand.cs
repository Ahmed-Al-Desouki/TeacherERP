using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public record RejectCenterJoinRequestCommand(Guid RequestId, string? Reason = null) : IRequest<Result<bool>>;
}
