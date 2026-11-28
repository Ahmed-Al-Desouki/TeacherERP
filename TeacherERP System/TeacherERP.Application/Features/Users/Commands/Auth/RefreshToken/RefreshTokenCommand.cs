using MediatR;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponseDto>>;
}