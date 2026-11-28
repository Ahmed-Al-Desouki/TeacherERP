// موجود (تعديل) — TeacherERP.Application/Features/TeacherRequests/Admin/ListTeacherRequestsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    // قائمة طلبات المدرسين لمدير النظام بصفحات (الأحدث أولاً)، مع تصفية اختيارية بالحالة الفعلية
    public record ListTeacherRequestsQuery(
        TeacherAccessRequestStatus? Status = null,
        int Page = 1,
        int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<TeacherAccessRequestListItem>>>, IPagedRequest;
}
