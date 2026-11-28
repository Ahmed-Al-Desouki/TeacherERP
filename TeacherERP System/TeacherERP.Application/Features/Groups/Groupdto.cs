using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Features.Groups
{
    public record GroupSlotDto(Guid Id, DayOfWeek Day, TimeSpan StartTime, int DurationMinutes);

    public record GroupDto(
        Guid Id,
        string Name,
        string GradeLevel,
        SchoolGrade? Grade,
        Guid CenterId,
        string? Room,
        int? Capacity,
        decimal? PerSessionPrice,
        decimal? MonthlyPrice,
        string? WhatsAppGroupLink,
        bool IsActive,
        List<GroupSlotDto> Slots)
    {
        public static GroupDto From(Group group)
            => new(
                group.Id,
                group.Name,
                group.GradeLevel,
                group.SchoolGrade,
                group.CenterId,
                group.Room,
                group.Capacity,
                group.PerSessionPrice,
                group.MonthlyPrice,
                group.WhatsAppGroupLink,
                group.IsActive,
                group.Slots
                    .OrderBy(s => ((int)s.Day + 1) % 7) // الأسبوع يبدأ بالسبت
                    .ThenBy(s => s.StartTime)
                    .Select(s => new GroupSlotDto(s.Id, s.Day, s.StartTime, s.DurationMinutes))
                    .ToList());
    }
}
