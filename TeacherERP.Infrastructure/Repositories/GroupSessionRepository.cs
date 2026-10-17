using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

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

        public async Task<HashSet<DateTime>> GetStartTimesAsync(
            Guid groupId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
        {
            var times = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.GroupId == groupId && s.StartTime >= fromUtc && s.StartTime < toUtc)
                .Select(s => s.StartTime)
                .ToListAsync(cancellationToken);

            return times.ToHashSet();
        }

        public Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            return _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
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

        public Task<List<SessionListItem>> GetByRangeAsync(
            DateTime fromUtc, DateTime toUtc, Guid? groupId, Guid userId, bool seesAll, CancellationToken cancellationToken)
        {
            var query = _context.Sessions
                .AsNoTracking()
                .Where(s => s.StartTime >= fromUtc && s.StartTime < toUtc);

            if (groupId.HasValue)
            {
                query = query.Where(s => s.GroupId == groupId.Value);
            }

            // المساعد يرى حصص مجموعاته المخصصة له فقط
            if (!seesAll)
            {
                query = query.Where(s => s.Group.AssignedAdmins.Any(admin => admin.Id == userId));
            }

            return query
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.Id)
                .Select(s => new SessionListItem(
                    s.Id, s.GroupId, s.Group.Name, s.StartTime, s.EndTime, s.IsCancelled, s.SlotId, s.CancelReason, s.IsModified))
                .ToListAsync(cancellationToken);
        }
    }
}