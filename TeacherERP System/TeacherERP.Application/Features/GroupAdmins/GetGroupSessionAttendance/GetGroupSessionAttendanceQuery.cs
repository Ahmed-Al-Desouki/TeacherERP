// جديد — TeacherERP.Application/Features/GroupAdmins/GetGroupSessionAttendance/GetGroupSessionAttendanceQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GetGroupSessionAttendance
{
    // حضور الحصة الجارية لمجموعة الطالب المسؤول (الاسم والحالة فقط)
    public record GetGroupSessionAttendanceQuery(int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<AttendanceListItem>>>, IPagedRequest;
}