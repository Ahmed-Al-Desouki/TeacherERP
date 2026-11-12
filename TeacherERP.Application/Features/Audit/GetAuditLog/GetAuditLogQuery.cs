// موجود (تعديل) — TeacherERP.Application/Features/Audit/GetAuditLog/GetAuditLogQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Audit.GetAuditLog
{
    // سجل التدقيق بصفحات (الأحدث أولاً)، مع تصفية اختيارية بالمستخدم المتأثر (للمدرس فقط عبر الـ Policy)
    public record GetAuditLogQuery(Guid? TargetUserId, int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<AuditLogDto>>>, IPagedRequest;
}
