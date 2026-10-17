using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.CancelSession
{
    // إلغاء حصة قادمة بسبب اختياري (للمدرس فقط عبر الـ Policy)
    public record CancelSessionCommand(Guid SessionId, string? Reason) : IRequest<Result<SessionTimeDto>>;
}