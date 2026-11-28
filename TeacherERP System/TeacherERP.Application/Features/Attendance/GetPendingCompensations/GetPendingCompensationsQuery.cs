// موجود (تعديل) — TeacherERP.Application/Features/Attendance/GetPendingCompensations/GetPendingCompensationsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetPendingCompensations
{
    // طلبات التعويض المعلّقة بصفحات (الأقدم أولاً، كطابور للموظف)
    public record GetPendingCompensationsQuery(int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<PendingCompensationItem>>>, IPagedRequest;
}
