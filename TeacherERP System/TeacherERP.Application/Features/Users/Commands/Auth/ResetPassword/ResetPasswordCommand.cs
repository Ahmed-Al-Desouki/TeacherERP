using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword
{
    public record ResetPasswordCommand(string Email, string ResetToken, string NewPassword) : IRequest<Result<bool>>;
}