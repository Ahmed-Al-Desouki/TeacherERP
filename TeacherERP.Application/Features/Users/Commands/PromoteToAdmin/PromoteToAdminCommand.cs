using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.PromoteToAdmin
{
    public record PromoteToAdminCommand(Guid UserId) : IRequest<Result<bool>>;
}