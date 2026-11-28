using MediatR;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn
{
    public record GoogleSignInCommand(string IdToken, string DeviceId, string DeviceInfo) : IRequest<Result<AuthResponseDto>>;
}