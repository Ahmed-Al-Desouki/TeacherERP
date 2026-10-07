using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result<ForgotPasswordResponseDto>>;
}
