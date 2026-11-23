using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces;

public sealed record StudentWeeklyScheduleSlotItem(DayOfWeek Day, TimeSpan StartTime, int DurationMinutes);

public sealed record StudentWeeklyScheduleGroupItem(Guid GroupId, string GroupName, Guid CenterId,
    string? Room, IReadOnlyList<StudentWeeklyScheduleSlotItem> Slots);

public sealed record StudentWeeklyScheduleData(string TimeZoneId,
    IReadOnlyList<StudentWeeklyScheduleGroupItem> Groups);

public interface IStudentWeeklyScheduleRepository
{
    Task<StudentWeeklyScheduleData> GetForStudentAsync(Guid tenantId, Guid studentId,
        CancellationToken cancellationToken);
}
