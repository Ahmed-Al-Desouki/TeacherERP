using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.Logout
{
    // إنهاء الجلسة الحالية (الجهاز الذي أرسل الطلب)
    public record LogoutCommand : IRequest<Result<bool>>;
}