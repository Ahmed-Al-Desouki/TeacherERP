using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Schedule
{
    // نتيجة تعديل حصة واحدة (الأوقات UTC)
    public record SessionTimeDto(Guid Id, DateTime StartTime, DateTime EndTime, bool IsCancelled, string? CancelReason)
    {
        public static SessionTimeDto From(Session session)
            => new(session.Id, session.StartTime, session.EndTime, session.IsCancelled, session.CancelReason);
    }

    // نتيجة التعديل الجماعي: المتأثر، وما تم تخطيه (الملغاة أصلاً)
    public record BulkResultDto(int Affected, int Skipped);
}