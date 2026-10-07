using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp
{
    public record VerifyResetOtpCommand(string Email, string OtpCode) : IRequest<Result<ResetTokenDto>>;
}