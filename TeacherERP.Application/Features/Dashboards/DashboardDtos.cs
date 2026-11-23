using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Features.Dashboards;

public sealed record TeacherDashboardDto(
    DateTime GeneratedAtUtc,
    int PeriodYear,
    int PeriodMonth,
    int ActiveGroups,
    int ActiveStudents,
    int SessionsToday,
    int PendingEnrollmentRequests,
    int PendingPaymentReviews,
    int SuccessfulPaymentsThisMonth,
    decimal CollectedAmountThisMonth,
    decimal PendingPaymentAmountThisMonth,
    int OutstandingDebtCount,
    decimal OutstandingDebtAmount,
    decimal OutstandingPlatformCommissionAmount,
    IReadOnlyList<DashboardUpcomingSession> UpcomingSessions);

public sealed record AdministratorDashboardDto(
    DateTime GeneratedAtUtc,
    int PeriodYear,
    int PeriodMonth,
    int ActiveTeachers,
    int ActiveStudents,
    int Centers,
    int PendingTeacherRequests,
    int PendingCenterJoinRequests,
    int PendingPaymentReviews,
    int PendingPaymobAdjustments,
    int SuccessfulPaymentsThisMonth,
    decimal CollectedAmountThisMonth,
    decimal PendingPaymentAmountThisMonth,
    int OutstandingDebtCount,
    decimal OutstandingDebtAmount,
    int OutstandingPlatformCommissionCount,
    decimal OutstandingPlatformCommissionAmount,
    int SettledPlatformCommissionCount,
    decimal SettledPlatformCommissionAmount);
