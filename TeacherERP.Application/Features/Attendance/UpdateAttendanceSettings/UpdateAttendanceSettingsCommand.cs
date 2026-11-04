// موجود (تعديل) — TeacherERP.Application/Features/Attendance/UpdateAttendanceSettings/UpdateAttendanceSettingsCommand.cs
using MediatR;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings
{
    // وضع الحضور (ثابت أو متغير أو من المساعد فقط) ورفض تزييف الموقع والتسجيل التلقائي بالمسح،
    // واعتماد مسح الطالب المسؤول فوراً (للمدرس فقط). استبدال كامل: الحقل الناقص يرجع false
    public record UpdateAttendanceSettingsCommand(
        AttendanceMode Mode,
        bool RejectMockLocations,
        bool AutoRegisterOnScan,
        bool AutoApproveGroupAdminScans = false)
        : IRequest<Result<AttendanceSettingsDto>>;
}