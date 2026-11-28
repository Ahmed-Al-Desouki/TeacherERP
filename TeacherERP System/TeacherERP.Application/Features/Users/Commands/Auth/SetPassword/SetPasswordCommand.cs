using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.SetPassword;

public record SetPasswordCommand(string NewPassword) : IRequest<Result<bool>>;
