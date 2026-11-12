// موجود (تعديل) — TeacherERP.Application/Features/Attendance/GetSessionAttendance/GetSessionAttendanceQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetSessionAttendance
{
    // سجلات حضور حصة بصفحات (للمدرس والمساعد). مرتبة بوقت التسجيل
    public record GetSessionAttendanceQuery(Guid SessionId, int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<AttendanceListItem>>>, IPagedRequest;
}
