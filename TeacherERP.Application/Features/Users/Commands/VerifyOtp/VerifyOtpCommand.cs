using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.VerifyOtp
{
    public record VerifyOtpCommand(Guid StudentId, string OtpCode) : IRequest<Result<bool>>;
}