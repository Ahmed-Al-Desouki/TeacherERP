using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications;

public static class NotificationErrors
{
    public static readonly Error ShareUnavailable =
        new("Notifications.ShareUnavailable", "This notification has no shareable session file.");
}
