// جديد — TeacherERP.Application/Features/Attendance/ManualAttendance/ManualAttendanceCommand.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.ManualAttendance
{
    // إدخال حضور يدوي لطالب من قائمتي (للمدرس والمساعد). حصة جارية: تسجيل عادي. حصة انتهت: تصحيح بسبب إجباري
    public record ManualAttendanceCommand(Guid StudentId, Guid SessionId, string? Reason)
        : IRequest<Result<AttendanceResultDto>>;
}