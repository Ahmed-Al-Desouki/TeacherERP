// جديد — TeacherERP.Application/Features/Attendance/AdminScan/AdminScanCommand.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.AdminScan
{
    // المدرس أو المساعد يمسح كارت الطالب (StaticQrCode). الطالب غير المسجل يتسجل في الحصة الجارية بموافقة الماسح نفسه
    public record AdminScanCommand(string StudentQrCode, Guid? GroupId) : IRequest<Result<AttendanceResultDto>>;
}