// جديد — TeacherERP.Application/Features/Notifications/GetMyNotifications/GetMyNotificationsQuery.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.GetMyNotifications
{
    // إشعاراتي من كل مدرسيّ (أحدث 100)، والمقروءة اختيارياً
    public record GetMyNotificationsQuery(bool UnreadOnly) : IRequest<Result<List<NotificationItem>>>;
}