// جديد — TeacherERP.Application/Features/Directory/DirectoryDtos.cs
using TeacherERP.Application.Features.Groups;

namespace TeacherERP.Application.Features.Directory
{
    public record DirectoryTeacherDto(Guid TeacherId, string Name, string? Subject);

    // السنتر أولاً ثم مدرسوه (نتيجة البحث)
    public record DirectoryCenterDto(Guid CenterId, string Name, string? Address, List<DirectoryTeacherDto> Teachers);

    public record DirectoryGroupDto(
        Guid GroupId,
        string Name,
        string GradeLevel,
        Guid CenterId,
        string? Room,
        int? Capacity,
        int? AvailableSeats,
        List<GroupSlotDto> Slots);

    public record DirectoryCenterInfoDto(Guid Id, string Name, string? Address);

    // ملف مدرس عام للطالب: مواعيد مجموعاته (بتوقيت المدرس TimeZoneId)
    public record TeacherProfileDto(
        Guid TeacherId,
        string Name,
        string? Subject,
        string TimeZoneId,
        List<DirectoryCenterInfoDto> Centers,
        List<DirectoryGroupDto> Groups);
}