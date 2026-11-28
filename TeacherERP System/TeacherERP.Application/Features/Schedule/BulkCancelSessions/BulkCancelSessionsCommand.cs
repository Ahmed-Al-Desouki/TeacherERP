using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.BulkCancelSessions
{
    // إلغاء كل الحصص القادمة في نطاق (إجازة مثلاً). GroupIds فارغة = كل مجموعات المدرس
    public record BulkCancelSessionsCommand(DateTime From, DateTime To, List<Guid>? GroupIds, string? Reason)
        : IRequest<Result<BulkResultDto>>;
}