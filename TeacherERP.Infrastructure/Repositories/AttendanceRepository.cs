// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/AttendanceRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---- عبر المدرسين بقصد (IgnoreQueryFilters) مع tenantId صريح، لأن الطالب ليس عضواً بعد ----

        public Task<AttendanceSettingsInfo?> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            return (from s in _context.TenantSettings.IgnoreQueryFilters().AsNoTracking()
                    join t in _context.Tenants.AsNoTracking() on s.TenantId equals t.Id
                    where s.TenantId == tenantId
                    select new AttendanceSettingsInfo(s.TenantId, t.Status, s.Mode, s.QrSecret, s.RejectMockLocations, s.AutoRegisterOnScan))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<RunningSessionInfo>> GetRunningSessionsAsync(
            Guid tenantId, DateTime nowUtc, int earlyMinutes, Guid? onlyGroupId, CancellationToken cancellationToken)
        {
            var startLimit = nowUtc.AddMinutes(earlyMinutes);

            return (from s in _context.Sessions.IgnoreQueryFilters().AsNoTracking()
                    join g in _context.Groups.IgnoreQueryFilters().AsNoTracking() on s.GroupId equals g.Id
                    join c in _context.Centers.AsNoTracking() on g.CenterId equals c.Id
                    where s.TenantId == tenantId
                          && !s.IsCancelled
                          && g.IsActive
                          && s.StartTime <= startLimit
                          && s.EndTime >= nowUtc
                          && (onlyGroupId == null || g.Id == onlyGroupId)
                    orderby s.StartTime
                    select new RunningSessionInfo(
                        s.Id, g.Id, g.Name, s.StartTime, s.EndTime, c.Latitude, c.Longitude, c.GeofenceRadiusMeters))
                .ToListAsync(cancellationToken);
        }

        public Task<AttendanceRecord?> GetRecordAsync(Guid sessionId, Guid studentId, CancellationToken cancellationToken)
        {
            return _context.AttendanceRecords
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.SessionId == sessionId && a.StudentId == studentId, cancellationToken);
        }

        public async Task<Guid?> FindCompensableSessionAsync(
            Guid tenantId, Guid groupId, Guid studentId, DateTime nowUtc, int windowDays, CancellationToken cancellationToken)
        {
            var from = nowUtc.AddDays(-windowDays);
            var to = nowUtc.AddDays(windowDays);

            // حصة لم يحضرها الطالب فعلاً (لا سجل حضور له فيها غير "غائب")، ولم يعوّضها بحضور آخر
            var candidates = await _context.Sessions
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(s => s.TenantId == tenantId
                            && s.GroupId == groupId
                            && !s.IsCancelled
                            && s.StartTime >= from
                            && s.StartTime <= to
                            && !_context.AttendanceRecords.IgnoreQueryFilters().Any(a =>
                                a.StudentId == studentId
                                && ((a.SessionId == s.Id && a.Status != AttendanceStatus.Absent)
                                    || a.CompensatesSessionId == s.Id)))
                .Select(s => new { s.Id, s.StartTime })
                .ToListAsync(cancellationToken);

            return candidates
                .OrderBy(c => Math.Abs((c.StartTime - nowUtc).Ticks))
                .Select(c => (Guid?)c.Id)
                .FirstOrDefault();
        }

        public Task<Guid?> GetStudentIdByQrAsync(string studentQrCode, CancellationToken cancellationToken)
        {
            return _context.Users
                .AsNoTracking()
                .Where(u => u.StaticQrCode == studentQrCode)
                .Select(u => (Guid?)u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public void AddRecord(AttendanceRecord record) => _context.AttendanceRecords.Add(record);

        // ---- للموظفين داخل المدرس الحالي (الفلتر العام يعمل) ----

        public Task<TenantSettings?> GetTrackedSettingsAsync(CancellationToken cancellationToken)
        {
            return _context.TenantSettings.FirstOrDefaultAsync(cancellationToken);
        }

        public Task<AttendanceRecord?> GetTrackedRecordAsync(Guid recordId, CancellationToken cancellationToken)
        {
            return _context.AttendanceRecords.FirstOrDefaultAsync(a => a.Id == recordId, cancellationToken);
        }

        public Task<List<AttendanceListItem>> GetSessionAttendanceAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            return _context.AttendanceRecords
                .AsNoTracking()
                .Where(a => a.SessionId == sessionId)
                .OrderBy(a => a.CreatedAt)
                .Select(a => new AttendanceListItem(
                    a.Id, a.StudentId, a.Student.FullName, a.Status, a.ScannedVia, a.CreatedAt, a.CompensatesSessionId))
                .ToListAsync(cancellationToken);
        }

        public Task<List<PendingCompensationItem>> GetPendingCompensationsAsync(CancellationToken cancellationToken)
        {
            var query = _context.AttendanceRecords
                .AsNoTracking()
                .Where(a => a.Status == AttendanceStatus.PendingCompensation);

            return query
                .OrderBy(a => a.CreatedAt)
                .Take(200)
                .Select(a => new PendingCompensationItem(
                    a.Id,
                    a.StudentId,
                    a.Student.FullName,
                    a.SessionId,
                    a.Session.Group.Name,
                    a.Session.StartTime,
                    a.CompensatesSessionId,
                    _context.Sessions.Where(s => s.Id == a.CompensatesSessionId).Select(s => (DateTime?)s.StartTime).FirstOrDefault(),
                    _context.Sessions.Where(s => s.Id == a.CompensatesSessionId).Select(s => s.Group.Name).FirstOrDefault(),
                    a.CreatedAt))
                .ToListAsync(cancellationToken);
        }
    }
}