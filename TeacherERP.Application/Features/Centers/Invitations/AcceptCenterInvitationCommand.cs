using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.Invitations
{
    public record AcceptCenterInvitationCommand(string Token) : IRequest<Result<CenterDto>>;
}
