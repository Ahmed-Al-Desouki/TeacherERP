// جديد — TeacherERP.Application/Features/GroupAdmins/GetGroupAdminRequests/GetGroupAdminRequestsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests
{
    // طلبات المساعدين لتعيين/إزالة الطلاب المسؤولين (للمدرس فقط). الحالة اختيارية، والأحدث أولاً
    public record GetGroupAdminRequestsQuery(
        GroupAdminRequestStatus? Status,
        int Page = 1,
        int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<GroupAdminRequestDto>>>, IPagedRequest;
}