namespace TeacherERP.Application.Interfaces;

public interface IApplicationDataRetentionCleanupJob
{
    Task PurgeReadNotificationsAsync();
    Task PurgeClosedTeacherRequestsAsync();
    Task PurgeClosedEnrollmentRequestsAsync();
    Task PurgeClosedGroupAdminRequestsAsync();
}
