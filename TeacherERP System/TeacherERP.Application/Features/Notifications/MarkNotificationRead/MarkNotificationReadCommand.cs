// جديد — TeacherERP.Application/Features/Notifications/MarkNotificationRead/MarkNotificationReadCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.MarkNotificationRead
{
    // تعليم إشعار واحد كمقروء (إشعاري أنا فقط)
    public record MarkNotificationReadCommand(Guid NotificationId) : IRequest<Result<bool>>;
}