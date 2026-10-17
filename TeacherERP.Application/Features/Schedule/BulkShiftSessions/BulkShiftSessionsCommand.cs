using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.BulkShiftSessions
{
    // تأجيل (موجب) أو تقديم (سالب) كل الحصص القادمة في نطاق بعدد دقائق. GroupIds فارغة = كل المجموعات
    public record BulkShiftSessionsCommand(DateTime From, DateTime To, List<Guid>? GroupIds, int ShiftMinutes)
        : IRequest<Result<BulkResultDto>>;
}