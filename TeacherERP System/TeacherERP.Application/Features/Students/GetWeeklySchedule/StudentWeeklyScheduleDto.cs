using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Features.Students.GetWeeklySchedule;

public sealed record StudentWeeklyScheduleDto(string TimeZoneId,
    IReadOnlyList<StudentWeeklyGroupScheduleDto> Groups)
{
    public static StudentWeeklyScheduleDto From(StudentWeeklyScheduleData schedule) =>
        new(schedule.TimeZoneId, schedule.Groups.Select(group => new StudentWeeklyGroupScheduleDto(
            group.GroupId, group.GroupName, group.CenterId, group.Room,
            group.Slots.Select(slot => new StudentWeeklyScheduleSlotDto(slot.Day, slot.StartTime,
                slot.DurationMinutes)).ToList())).ToList());
}

public sealed record StudentWeeklyGroupScheduleDto(Guid GroupId, string GroupName, Guid CenterId,
    string? Room, IReadOnlyList<StudentWeeklyScheduleSlotDto> Slots);

public sealed record StudentWeeklyScheduleSlotDto(DayOfWeek Day, TimeSpan StartTime, int DurationMinutes);
