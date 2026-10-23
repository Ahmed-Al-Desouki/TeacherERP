// جديد — TeacherERP.Application/Features/Attendance/UpdateAttendanceSettings/UpdateAttendanceSettingsCommand.cs
using MediatR;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings
{
    // وضع الحضور (ثابت أو متغير أو من المساعد فقط) ورفض تزييف الموقع والتسجيل التلقائي بالمسح (للمدرس فقط)
    public record UpdateAttendanceSettingsCommand(AttendanceMode Mode, bool RejectMockLocations, bool AutoRegisterOnScan)
        : IRequest<Result<AttendanceSettingsDto>>;
}