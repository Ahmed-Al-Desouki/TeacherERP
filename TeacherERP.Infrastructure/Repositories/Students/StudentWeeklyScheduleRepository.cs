using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories;

public sealed class StudentWeeklyScheduleRepository : IStudentWeeklyScheduleRepository
{
    private readonly AppDbContext _context;

    public StudentWeeklyScheduleRepository(AppDbContext context) => _context = context;

    public async Task<StudentWeeklyScheduleData> GetForStudentAsync(Guid tenantId, Guid studentId,
        CancellationToken cancellationToken)
    {
        var timeZoneId = await _context.Tenants.IgnoreQueryFilters().AsNoTracking()
            .Where(tenant => tenant.Id == tenantId)
            .Select(tenant => tenant.TimeZoneId)
            .SingleAsync(cancellationToken);

        var groups = await _context.Groups.IgnoreQueryFilters().AsNoTracking()
            .Where(group => group.TenantId == tenantId && group.IsActive
                && _context.GroupEnrollments.IgnoreQueryFilters().Any(enrollment =>
                    enrollment.TenantId == tenantId
                    && enrollment.GroupId == group.Id
                    && enrollment.StudentId == studentId
                    && enrollment.Status == EnrollmentStatus.Active))
            .Include(group => group.Slots)
            .OrderBy(group => group.Name)
            .ThenBy(group => group.Id)
            .ToListAsync(cancellationToken);

        var schedules = groups.Select(group => new StudentWeeklyScheduleGroupItem(
            group.Id,
            group.Name,
            group.CenterId,
            group.Room,
            group.Slots
                .OrderBy(slot => ((int)slot.Day + 1) % 7)
                .ThenBy(slot => slot.StartTime)
                .Select(slot => new StudentWeeklyScheduleSlotItem(
                    slot.Day, slot.StartTime, slot.DurationMinutes))
                .ToList()))
            .ToList();

        return new StudentWeeklyScheduleData(timeZoneId, schedules);
    }
}
