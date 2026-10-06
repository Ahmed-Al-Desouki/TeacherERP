using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    public record LoginCommand(string Email, string Password, string DeviceInfo) : IRequest<Result<AuthResponseDto>>;
}