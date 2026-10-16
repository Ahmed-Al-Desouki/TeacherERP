using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.GetSessions
{
    // حصص المدرس الحالي في نطاق (الافتراضي: 14 يوم قادمة، والحد الأقصى 62 يوم)
    public record GetSessionsQuery(DateTime? From, DateTime? To, Guid? GroupId) : IRequest<Result<List<SessionDto>>>;
}