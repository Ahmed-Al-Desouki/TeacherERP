using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.Logout
{
    // إنهاء الجلسة المرتبطة بالـ Refresh Token (الجهاز الحالي)
    public record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;
}