using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.RevokeSession
{
    // إنهاء جلسة جهاز معيّن من قائمة أجهزة المستخدم
    public record RevokeSessionCommand(Guid SessionId) : IRequest<Result<bool>>;
}