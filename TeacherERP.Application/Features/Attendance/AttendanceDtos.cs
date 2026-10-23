// جديد — TeacherERP.Application/Features/Attendance/AttendanceDtos.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Attendance
{
    // محتوى الـ QR الثابت للطباعة: التطبيق يبني منه الكود (teacherId + code)
    public record AttendanceSettingsDto(
        Guid TeacherId,
        AttendanceMode Mode,
        bool RejectMockLocations,
        bool AutoRegisterOnScan,
        string? StaticCode);

    // الكود المتغير لشاشة المدرس (يُجدَّد قبل انتهاء ExpiresInSeconds)
    public record DisplayCodeDto(Guid TeacherId, string Code, int ExpiresInSeconds);
}