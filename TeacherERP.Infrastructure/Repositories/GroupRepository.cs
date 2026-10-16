using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _context;

        public GroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Group group)
        {
            _context.Groups.Add(group);
        }

        public Task<List<Group>> GetVisibleAsync(Guid userId, bool seesAll, CancellationToken cancellationToken)
        {
            return VisibleQuery(userId, seesAll)
                .OrderBy(g => g.Name)
                .ToListAsync(cancellationToken);
        }

        public Task<Group?> GetVisibleByIdAsync(Guid groupId, Guid userId, bool seesAll, CancellationToken cancellationToken)
        {
            return VisibleQuery(userId, seesAll)
                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);
        }

        public Task<List<Group>> GetActiveWithSlotsAsync(CancellationToken cancellationToken)
        {
            return _context.Groups
                .AsNoTracking()
                .Include(g => g.Slots)
                .Where(g => g.IsActive)
                .ToListAsync(cancellationToken);
        }

        public Task<Group?> GetWithSlotsAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return _context.Groups
                .AsNoTracking()
                .Include(g => g.Slots)
                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);
        }

        public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
        {
            return _context.Groups.AnyAsync(g => g.Name == name, cancellationToken);
        }

        // المساعد يرى المجموعات المخصصة له فقط (ربط الحصة بالمساعد)
        private IQueryable<Group> VisibleQuery(Guid userId, bool seesAll)
        {
            IQueryable<Group> query = _context.Groups
                .AsNoTracking()
                .Include(g => g.Slots);

            return seesAll
                ? query
                : query.Where(g => g.AssignedAdmins.Any(admin => admin.Id == userId));
        }
    }
}