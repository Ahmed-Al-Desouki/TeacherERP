// جديد — TeacherERP.Application/Features/Attendance/ScanAttendance/ScanAttendanceCommand.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.ScanAttendance
{
    // الطالب يمسح الكود (teacherId + code من الـ QR) بموقعه. GroupId اختياري لو أكثر من حصة جارية
    public record ScanAttendanceCommand(
        Guid TeacherId,
        string Code,
        double Latitude,
        double Longitude,
        double? AccuracyMeters,
        bool IsMockLocation,
        Guid? GroupId) : IRequest<Result<AttendanceResultDto>>;
}