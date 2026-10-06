using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.VerifyOtp
{
    // يستقبل الإيميل والرمز، ويرجع الـ Guid الخاص بالمستخدم الجديد
    public record VerifyOtpCommand(string Email, string OtpCode) : IRequest<Result<Guid>>;
}