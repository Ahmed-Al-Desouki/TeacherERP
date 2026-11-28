// جديد — TeacherERP.Application/Features/Notifications/MarkAllNotificationsRead/MarkAllNotificationsReadCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.MarkAllNotificationsRead
{
    public record MarkAllNotificationsReadCommand : IRequest<Result<bool>>;
}