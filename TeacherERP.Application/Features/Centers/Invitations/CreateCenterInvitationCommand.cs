using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.Invitations
{
    public record CreateCenterInvitationCommand(Guid CenterId) : IRequest<Result<CenterInvitationDto>>;
}
