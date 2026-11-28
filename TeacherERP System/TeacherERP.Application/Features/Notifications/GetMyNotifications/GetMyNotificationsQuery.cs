// موجود (تعديل) — TeacherERP.Application/Features/Notifications/GetMyNotifications/GetMyNotificationsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.GetMyNotifications
{
    // إشعاراتي من كل مدرسيّ بصفحات (الأحدث أولاً)، والمقروءة اختيارياً
    public record GetMyNotificationsQuery(bool UnreadOnly, int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<NotificationItem>>>, IPagedRequest;
}
