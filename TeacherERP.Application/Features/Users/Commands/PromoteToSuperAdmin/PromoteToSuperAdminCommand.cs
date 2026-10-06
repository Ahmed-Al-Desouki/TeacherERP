using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.PromoteToSuperAdmin
{
    public record PromoteToSuperAdminCommand(Guid UserId) : IRequest<Result<bool>>;
}