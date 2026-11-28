using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context) => _context = context;

    public async Task<TeacherDashboardOperationalStats> GetTeacherStatsAsync(
        Guid tenantId, DateTime dayStartUtc, DateTime dayEndUtc, DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var activeGroups = await _context.Groups
            .CountAsync(group => group.TenantId == tenantId && group.IsActive, cancellationToken);

        var activeStudents = await _context.TenantMembers
            .CountAsync(member => member.TenantId == tenantId
                && member.Role == TenantRole.Student
                && member.Status == MembershipStatus.Active
                && member.User.Status == StudentStatus.Active, cancellationToken);

        var sessionsToday = await _context.Sessions
            .CountAsync(session => session.TenantId == tenantId
                && !session.IsCancelled
                && session.StartTime >= dayStartUtc
                && session.StartTime < dayEndUtc, cancellationToken);

        var pendingEnrollmentRequests = await _context.EnrollmentRequests
            .CountAsync(request => request.TenantId == tenantId
                && request.Status == EnrollmentRequestStatus.Pending, cancellationToken);

        var pendingPaymentReviews = await _context.Transactions
            .CountAsync(transaction => transaction.TenantId == tenantId
                && transaction.Status == TransactionStatus.PendingApproval, cancellationToken);

        var upcomingSessions = await _context.Sessions
            .AsNoTracking()
            .Where(session => session.TenantId == tenantId
                && !session.IsCancelled
                && session.StartTime >= nowUtc)
            .OrderBy(session => session.StartTime)
            .ThenBy(session => session.Id)
            .Take(5)
            .Select(session => new DashboardUpcomingSession(
                session.Id, session.GroupId, session.Group.Name, session.StartTime, session.EndTime))
            .ToListAsync(cancellationToken);

        return new TeacherDashboardOperationalStats(
            activeGroups, activeStudents, sessionsToday, pendingEnrollmentRequests,
            pendingPaymentReviews, upcomingSessions);
    }

    public async Task<AdministratorDashboardOperationalStats> GetAdministratorStatsAsync(
        DateTime nowUtc, CancellationToken cancellationToken)
    {
        var activeTeachers = await _context.Tenants.IgnoreQueryFilters()
            .CountAsync(tenant => tenant.Status == TenantStatus.Active, cancellationToken);

        var activeStudents = await _context.TenantMembers.IgnoreQueryFilters()
            .Where(member => member.Role == TenantRole.Student
                && member.Status == MembershipStatus.Active
                && member.User.Status == StudentStatus.Active)
            .Select(member => member.UserId)
            .Distinct()
            .CountAsync(cancellationToken);

        var centers = await _context.Centers.IgnoreQueryFilters().CountAsync(cancellationToken);

        var pendingTeacherRequests = await _context.TeacherAccessRequests.IgnoreQueryFilters()
            .CountAsync(request => request.Status == TeacherAccessRequestStatus.Pending
                && request.ExpiresAtUtc > nowUtc, cancellationToken);

        var pendingCenterJoinRequests = await _context.CenterJoinRequests.IgnoreQueryFilters()
            .CountAsync(request => request.Status == CenterJoinRequestStatus.Pending, cancellationToken);

        var pendingPaymentReviews = await _context.Transactions.IgnoreQueryFilters()
            .CountAsync(transaction => transaction.Status == TransactionStatus.PendingApproval, cancellationToken);

        var pendingPaymobAdjustments = await _context.PaymobAdjustmentReviews.IgnoreQueryFilters()
            .CountAsync(review => review.Status == PaymobAdjustmentReviewStatus.Pending, cancellationToken);

        return new AdministratorDashboardOperationalStats(
            activeTeachers, activeStudents, centers, pendingTeacherRequests,
            pendingCenterJoinRequests, pendingPaymentReviews, pendingPaymobAdjustments);
    }
}
