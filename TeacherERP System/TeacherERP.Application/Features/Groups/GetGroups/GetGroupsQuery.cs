// موجود (تعديل) — TeacherERP.Application/Features/Groups/GetGroups/GetGroupsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.GetGroups
{
    // مجموعات المدرس الحالي بصفحات: المدرس والمساعد يريان الكل (مرتبة بالاسم)
    public record GetGroupsQuery(int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<GroupDto>>>, IPagedRequest;
}
