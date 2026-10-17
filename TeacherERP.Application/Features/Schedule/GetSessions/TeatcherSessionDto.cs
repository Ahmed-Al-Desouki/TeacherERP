using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Features.Schedule.GetSessions
{
    // الأوقات UTC (تظهر في الـ JSON بالـ Z) والتطبيق يحوّلها لتوقيت الجهاز
    public record SessionDto(
        Guid Id,
        Guid GroupId,
        string GroupName,
        DateTime StartTime,
        DateTime EndTime,
        bool IsCancelled,
        Guid? SlotId,
        string? CancelReason,
        bool IsModified)
    {
        public static SessionDto From(SessionListItem item)
            => new(item.Id, item.GroupId, item.GroupName, item.StartTime, item.EndTime, item.IsCancelled, item.SlotId, item.CancelReason, item.IsModified);
    }
}