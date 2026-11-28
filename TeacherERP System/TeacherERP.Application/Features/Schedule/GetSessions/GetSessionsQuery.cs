// موجود (تعديل) — TeacherERP.Application/Features/Schedule/GetSessions/GetSessionsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.GetSessions
{
    // حصص المدرس الحالي في نطاق (الافتراضي: 14 يوم قادمة، والحد الأقصى 62 يوم) بصفحات، مرتبة بوقت البداية
    public record GetSessionsQuery(
        DateTime? From,
        DateTime? To,
        Guid? GroupId,
        int Page = 1,
        int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<SessionDto>>>, IPagedRequest;
}
