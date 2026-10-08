using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    // DeviceId: معرّف ثابت للجهاز من التطبيق، DeviceInfo: اسم الجهاز للعرض
    public record LoginCommand(string Email, string Password, string DeviceId, string DeviceInfo) : IRequest<Result<AuthResponseDto>>;
}