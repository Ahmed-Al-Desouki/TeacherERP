// جديد — TeacherERP.Application/Features/Attendance/GetAttendanceSettings/GetAttendanceSettingsQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetAttendanceSettings
{
    // إعدادات الحضور للمدرس (مع الكود الثابت للطباعة لو الوضع ثابت)
    public record GetAttendanceSettingsQuery : IRequest<Result<AttendanceSettingsDto>>;
}