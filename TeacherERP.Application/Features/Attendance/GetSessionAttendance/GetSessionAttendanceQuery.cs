// جديد — TeacherERP.Application/Features/Attendance/GetSessionAttendance/GetSessionAttendanceQuery.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetSessionAttendance
{
    // سجلات حضور حصة (المدرس لكل حصصه، والمساعد لحصص مجموعاته فقط)
    public record GetSessionAttendanceQuery(Guid SessionId) : IRequest<Result<List<AttendanceListItem>>>;
}