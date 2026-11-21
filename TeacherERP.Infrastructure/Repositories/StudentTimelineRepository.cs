using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories;

public sealed class StudentTimelineRepository : IStudentTimelineRepository
{
    private readonly AppDbContext _context;

    public StudentTimelineRepository(AppDbContext context) => _context = context;

    public Task<PagedResult<StudentTimelineSessionItem>> GetPageAsync(Guid tenantId, Guid studentId,
        DateTime fromUtc, DateTime toUtc, Guid? groupId, PageRequest page,
        CancellationToken cancellationToken)
    {
        var query = _context.Sessions.IgnoreQueryFilters().AsNoTracking()
            .Where(session => session.TenantId == tenantId
                && session.StartTime >= fromUtc && session.StartTime < toUtc
                && (groupId == null || session.GroupId == groupId.Value)
                && _context.GroupEnrollments.IgnoreQueryFilters().Any(enrollment =>
                    enrollment.TenantId == tenantId
                    && enrollment.GroupId == session.GroupId
                    && enrollment.StudentId == studentId
                    && enrollment.Status == EnrollmentStatus.Active
                    && enrollment.EnrolledAt <= session.StartTime));

        return query.OrderBy(session => session.StartTime).ThenBy(session => session.Id)
            .Select(session => new StudentTimelineSessionItem(
                session.Id,
                session.GroupId,
                session.Group.Name,
                session.StartTime,
                session.EndTime,
                session.IsCancelled,
                session.CancelReason,
                _context.AttendanceRecords.IgnoreQueryFilters()
                    .Where(attendance => attendance.TenantId == tenantId
                        && attendance.SessionId == session.Id
                        && attendance.StudentId == studentId)
                    .Select(attendance => (AttendanceStatus?)attendance.Status)
                    .FirstOrDefault()))
            .ToPagedAsync(page, cancellationToken);
    }
}
