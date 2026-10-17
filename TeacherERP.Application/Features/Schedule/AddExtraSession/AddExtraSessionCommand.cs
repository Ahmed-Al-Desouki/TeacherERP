using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.AddExtraSession
{
    // حصة إضافية لمجموعة خارج القالب الأسبوعي (للمدرس فقط عبر الـ Policy)
    public record AddExtraSessionCommand(Guid GroupId, DateTime StartTime, int DurationMinutes)
        : IRequest<Result<SessionTimeDto>>;
}