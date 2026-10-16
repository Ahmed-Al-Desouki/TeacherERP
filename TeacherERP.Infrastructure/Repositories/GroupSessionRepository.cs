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
                    s.Id, s.GroupId, s.Group.Name, s.StartTime, s.EndTime, s.IsCancelled, s.SlotId))
                .ToListAsync(cancellationToken);
        }
    }
}