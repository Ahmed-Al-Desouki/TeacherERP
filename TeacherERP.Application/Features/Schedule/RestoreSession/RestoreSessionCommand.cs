using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.RestoreSession
{
    // التراجع عن إلغاء حصة قادمة (للمدرس فقط عبر الـ Policy)
    public record RestoreSessionCommand(Guid SessionId) : IRequest<Result<SessionTimeDto>>;
}