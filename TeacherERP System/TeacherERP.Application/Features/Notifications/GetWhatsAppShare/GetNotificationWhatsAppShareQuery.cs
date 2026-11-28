using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.GetWhatsAppShare;

public sealed record GetNotificationWhatsAppShareQuery(Guid NotificationId)
    : IRequest<Result<NotificationWhatsAppShareDto>>;

public sealed record NotificationWhatsAppShareDto(
    string ShareText,
    string WhatsAppComposeUrl,
    string? WhatsAppGroupLink);
