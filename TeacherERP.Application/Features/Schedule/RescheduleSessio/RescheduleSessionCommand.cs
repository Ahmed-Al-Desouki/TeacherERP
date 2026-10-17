using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.RescheduleSession
{
    // تأجيل أو تقديم حصة قادمة (للمدرس فقط عبر الـ Policy)
    public record RescheduleSessionCommand(Guid SessionId, DateTime NewStartTime, int? DurationMinutes)
        : IRequest<Result<SessionTimeDto>>;
}