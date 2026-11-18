// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/GroupSessionRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    public class GroupSessionRepository : IGroupSessionRepository
    {
        private readonly AppDbContext _context;

        public GroupSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddRange(IEnumerable<Session> sessions)
        {
            _context.Sessions.AddRange(sessions);
        }

        public void RemoveRange(IEnumerable<Session> sessions)
        {
            _context.Sessions.RemoveRange(sessions);
        }

        public Task<List<Session>> GetTrackedBySlotIdsAsync(IReadOnlyCollection<Guid> slotIds, CancellationToken cancellationToken)
        {
            var ids = slotIds.ToArray();

            return _context.Sessions
                .Where(s => s.SlotId.HasValue && ids.Contains(s.SlotId.Value))
                .ToListAsync(cancellationToken);
        }

        public async Task<HashSet<Guid>> GetSessionIdsWithAttendanceAsync(IReadOnlyCollection<Guid> sessionIds, CancellationToken cancellationToken)
        {
            var ids = sessionIds.ToArray();

            var withAttendance = await _context.AttendanceRecords
                .AsNoTracking()
                .Where(a => ids.Contains(a.SessionId))
                .Select(a => a.SessionId)
                .Distinct()
                .ToListAsync(cancellationToken);

            return withAttendance.ToHashSet();
        }

        public async Task<HashSet<DateTime>> GetStartTimesAsync(
            Guid groupId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
        {
            var times = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.GroupId == groupId && s.StartTime >= fromUtc && s.StartTime < toUtc)
                .Select(s => s.StartTime)
                .ToListAsync(cancellationToken);

            // الحصة المنقولة تحتفظ بموعدها الأصلي في SlotId؛ احسب هذا الموعد مشغولاً في المجموعة المصدر
            var slotIds = await _context.GroupScheduleSlots
                .AsNoTracking()
                .Where(slot => slot.GroupId == groupId)
                .Select(slot => slot.Id)
                .ToListAsync(cancellationToken);
            if (slotIds.Count > 0)
            {
                var originalStarts = await _context.Sessions
                    .AsNoTracking()
                    .Where(s => s.SlotId.HasValue && slotIds.Contains(s.SlotId.Value)
                        && s.OriginalStartTime >= fromUtc && s.OriginalStartTime < toUtc)
                    .Select(s => s.OriginalStartTime!.Value)
                    .ToListAsync(cancellationToken);
                times.AddRange(originalStarts);
            }

            return times.ToHashSet();
        }

        public Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            return _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
        }

        public async Task<SessionMoveBlockers> GetMoveBlockersAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            var hasAttendance = await _context.AttendanceRecords
                .AnyAsync(record => record.SessionId == sessionId, cancellationToken);
            var hasCompensationReference = await _context.AttendanceRecords
                .AnyAsync(record => record.CompensatesSessionId == sessionId, cancellationToken);
            var hasPendingWalkInRequest = await _context.EnrollmentRequests
                .AnyAsync(request => request.SourceSessionId == sessionId
                    && request.Status == EnrollmentRequestStatus.Pending, cancellationToken);

            return new SessionMoveBlockers(hasAttendance, hasCompensationReference, hasPendingWalkInRequest);
        }

        public Task<List<Session>> GetTrackedByRangeAsync(
            DateTime fromUtc, DateTime toUtc, IReadOnlyCollection<Guid>? groupIds, CancellationToken cancellationToken)
        {
            var query = _context.Sessions.Where(s => s.StartTime >= fromUtc && s.StartTime < toUtc);

            if (groupIds is { Count: > 0 })
            {
                var ids = groupIds.ToArray();
                query = query.Where(s => ids.Contains(s.GroupId));
            }

            return query
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<bool> HasConflictAsync(
            Guid groupId, DateTime startUtc, DateTime endUtc, IReadOnlyCollection<Guid> excludeSessionIds, CancellationToken cancellationToken)
        {
            var excluded = excludeSessionIds.ToArray();

            // نفس وقت البداية يرفضه الـ Index الفريد حتى لو ملغاة، أما التداخل فمع غير الملغاة فقط
            return _context.Sessions.AnyAsync(
                s => s.GroupId == groupId
                     && !excluded.Contains(s.Id)
                     && (s.StartTime == startUtc
                         || (!s.IsCancelled && s.StartTime < endUtc && s.EndTime > startUtc)),
                cancellationToken);
        }

        public async Task<List<ScheduleConflictItem>> GetCrossGroupConflictsAsync(
            Guid tenantId, Guid groupId, Guid centerId, string? room, DateTime startUtc, DateTime endUtc,
            IReadOnlyCollection<Guid> excludeSessionIds, CancellationToken cancellationToken)
        {
            var excluded = excludeSessionIds.ToArray();
            var candidateQuery =
                from session in _context.Sessions.IgnoreQueryFilters().AsNoTracking()
                join peerGroup in _context.Groups.IgnoreQueryFilters() on session.GroupId equals peerGroup.Id
                join center in _context.Centers on peerGroup.CenterId equals center.Id
                where session.GroupId != groupId && !session.IsCancelled && !excluded.Contains(session.Id)
                    && session.StartTime < endUtc && session.EndTime > startUtc
                    && (session.TenantId == tenantId || (room != null && peerGroup.CenterId == centerId))
                select new ScheduleConflictItem(session.TenantId, session.GroupId, peerGroup.Name,
                    peerGroup.CenterId, center.Name, peerGroup.Room, session.StartTime, session.EndTime);
            var candidates = await candidateQuery.ToListAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(room))
                return candidates.Where(x => x.TenantId == tenantId).ToList();

            return candidates.Where(x => x.TenantId == tenantId
                || (x.CenterId == centerId && !string.IsNullOrWhiteSpace(x.Room)
                    && string.Equals(x.Room.Trim(), room.Trim(), StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public async Task<List<ScheduleConflictItem>> GetTemplateConflictsAsync(
            Guid tenantId, Guid groupId, Guid centerId, string? room, string timeZoneId,
            IReadOnlyCollection<ScheduleSlotCandidate> slots, DateOnly? effectiveFrom, CancellationToken cancellationToken)
        {
            if (slots.Count == 0) return new List<ScheduleConflictItem>();
            var peerQuery =
                from slot in _context.GroupScheduleSlots.IgnoreQueryFilters().AsNoTracking()
                join peerGroup in _context.Groups.IgnoreQueryFilters() on slot.GroupId equals peerGroup.Id
                join center in _context.Centers on peerGroup.CenterId equals center.Id
                join tenant in _context.Tenants.IgnoreQueryFilters() on peerGroup.TenantId equals tenant.Id
                where slot.GroupId != groupId && peerGroup.IsActive
                    && (slot.TenantId == tenantId || (!string.IsNullOrWhiteSpace(room) && peerGroup.CenterId == centerId))
                select new
                {
                    slot.TenantId, slot.Day, slot.StartTime, slot.DurationMinutes,
                    slot.GroupId, peerGroup.Name, peerGroup.CenterId, CenterName = center.Name,
                    Room = peerGroup.Room, tenant.TimeZoneId
                };
            var peers = await peerQuery.ToListAsync(cancellationToken);

            var homeZone = ResolveTimeZone(timeZoneId);
            var homeToday = effectiveFrom?.ToDateTime(TimeOnly.MinValue)
                ?? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, homeZone).Date;
            var conflicts = new Dictionary<(Guid GroupId, DateTime Start), ScheduleConflictItem>();
            var occurrences = new List<(DateTime Start, DateTime End)>();
            foreach (var slot in slots)
            {
                for (var offset = 0; offset < 28; offset++)
                {
                    var localDate = homeToday.AddDays(offset);
                    if (localDate.DayOfWeek != slot.Day) continue;
                    var start = ConvertLocalToUtc(localDate.Add(slot.StartTime), homeZone);
                    var end = start.AddMinutes(slot.DurationMinutes);
                    occurrences.Add((start, end));
                    foreach (var peer in peers)
                    {
                        if (peer.TenantId != tenantId && (string.IsNullOrWhiteSpace(room)
                            || !string.Equals(peer.Room?.Trim(), room.Trim(), StringComparison.OrdinalIgnoreCase))) continue;

                        var peerZone = ResolveTimeZone(peer.TimeZoneId);
                        var peerLocalDate = TimeZoneInfo.ConvertTimeFromUtc(start, peerZone).Date;
                        for (var dayShift = -1; dayShift <= 1; dayShift++)
                        {
                            var peerDate = peerLocalDate.AddDays(dayShift);
                            if (peerDate.DayOfWeek != peer.Day) continue;
                            var peerStart = ConvertLocalToUtc(peerDate.Add(peer.StartTime), peerZone);
                            var peerEnd = peerStart.AddMinutes(peer.DurationMinutes);
                            if (start >= peerEnd || end <= peerStart) continue;
                            conflicts[(peer.GroupId, peerStart)] = new ScheduleConflictItem(
                                peer.TenantId, peer.GroupId, peer.Name, peer.CenterId, peer.CenterName,
                                peer.Room, peerStart, peerEnd);
                        }
                    }
                }
            }

            if (occurrences.Count > 0)
            {
                var fromUtc = occurrences.Min(x => x.Start);
                var toUtc = occurrences.Max(x => x.End);
                var existingSessionQuery =
                    from session in _context.Sessions.IgnoreQueryFilters().AsNoTracking()
                    join peerGroup in _context.Groups.IgnoreQueryFilters() on session.GroupId equals peerGroup.Id
                    join center in _context.Centers on peerGroup.CenterId equals center.Id
                    where !session.IsCancelled && session.GroupId != groupId
                        && session.StartTime < toUtc && session.EndTime > fromUtc
                        && (session.TenantId == tenantId || (!string.IsNullOrWhiteSpace(room) && peerGroup.CenterId == centerId))
                    select new ScheduleConflictItem(session.TenantId, session.GroupId, peerGroup.Name,
                        peerGroup.CenterId, center.Name, peerGroup.Room, session.StartTime, session.EndTime);
                var existingSessions = await existingSessionQuery.ToListAsync(cancellationToken);

                foreach (var occurrence in occurrences)
                foreach (var existing in existingSessions)
                {
                    if (occurrence.Start >= existing.EndTime || occurrence.End <= existing.StartTime) continue;
                    if (existing.TenantId != tenantId && (string.IsNullOrWhiteSpace(room)
                        || existing.CenterId != centerId
                        || !string.Equals(existing.Room?.Trim(), room.Trim(), StringComparison.OrdinalIgnoreCase))) continue;
                    conflicts[(existing.GroupId, existing.StartTime)] = existing;
                }
            }
            return conflicts.Values.ToList();
        }

        private static TimeZoneInfo ResolveTimeZone(string? id)
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrWhiteSpace(id) ? "UTC" : id); }
            catch (TimeZoneNotFoundException) { return TimeZoneInfo.Utc; }
            catch (InvalidTimeZoneException) { return TimeZoneInfo.Utc; }
        }

        private static DateTime ConvertLocalToUtc(DateTime local, TimeZoneInfo zone)
        {
            local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
            for (var minute = 0; zone.IsInvalidTime(local) && minute < 180; minute++) local = local.AddMinutes(1);
            if (zone.IsAmbiguousTime(local))
                return new DateTimeOffset(local, zone.GetAmbiguousTimeOffsets(local).Min()).UtcDateTime;
            return TimeZoneInfo.ConvertTimeToUtc(local, zone);
        }

        public Task<PagedResult<SessionListItem>> GetByRangeAsync(
            DateTime fromUtc, DateTime toUtc, Guid? groupId, PageRequest page, CancellationToken cancellationToken)
        {
            var query = _context.Sessions
                .AsNoTracking()
                .Where(s => s.StartTime >= fromUtc && s.StartTime < toUtc);

            if (groupId.HasValue)
            {
                query = query.Where(s => s.GroupId == groupId.Value);
            }

            return query
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.Id)
                .Select(s => new SessionListItem(
                    s.Id, s.GroupId, s.Group.Name, s.StartTime, s.EndTime, s.IsCancelled, s.SlotId, s.CancelReason, s.IsModified))
                .ToPagedAsync(page, cancellationToken);
        }
    }
}
