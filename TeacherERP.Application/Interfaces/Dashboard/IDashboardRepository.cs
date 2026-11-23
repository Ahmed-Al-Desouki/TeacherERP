namespace TeacherERP.Application.Interfaces;

public sealed record DashboardUpcomingSession(
    Guid SessionId, Guid GroupId, string GroupName, DateTime StartTimeUtc, DateTime EndTimeUtc);

public sealed record TeacherDashboardOperationalStats(
    int ActiveGroups,
    int ActiveStudents,
    int SessionsToday,
    int PendingEnrollmentRequests,
    int PendingPaymentReviews,
    IReadOnlyList<DashboardUpcomingSession> UpcomingSessions);

public sealed record AdministratorDashboardOperationalStats(
    int ActiveTeachers,
    int ActiveStudents,
    int Centers,
    int PendingTeacherRequests,
    int PendingCenterJoinRequests,
    int PendingPaymentReviews,
    int PendingPaymobAdjustments);

public interface IDashboardRepository
{
    Task<TeacherDashboardOperationalStats> GetTeacherStatsAsync(
        Guid tenantId, DateTime dayStartUtc, DateTime dayEndUtc, DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<AdministratorDashboardOperationalStats> GetAdministratorStatsAsync(
        DateTime nowUtc, CancellationToken cancellationToken);
}
