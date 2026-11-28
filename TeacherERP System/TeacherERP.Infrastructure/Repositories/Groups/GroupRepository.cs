// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/GroupRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

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

        public Task<PagedResult<Group>> GetPageWithSlotsAsync(PageRequest page, CancellationToken cancellationToken)
        {
            return _context.Groups
                .AsNoTracking()
                .Include(g => g.Slots)
                .OrderBy(g => g.Name)
                .ThenBy(g => g.Id)
                .ToPagedAsync(page, cancellationToken);
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

        public Task<Group?> GetTrackedWithSlotsAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return _context.Groups
                .Include(g => g.Slots)
                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);
        }

        public Task<GroupJoinCodeLookup?> GetJoinCodeLookupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return (from g in _context.Groups.IgnoreQueryFilters().AsNoTracking()
                    join tenant in _context.Tenants.IgnoreQueryFilters().AsNoTracking()
                        on g.TenantId equals tenant.Id
                    where g.Id == groupId
                    select new GroupJoinCodeLookup(
                        g.Id, g.TenantId, g.IsActive, tenant.Status,
                        g.StaticJoinEnabled, g.DynamicJoinEnabled,
                        g.StaticJoinCode, g.DynamicJoinSecret))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<GroupJoinCodeLookup?> GetJoinCodeLookupByStaticCodeAsync(
            string normalizedCode, CancellationToken cancellationToken)
        {
            return (from g in _context.Groups.IgnoreQueryFilters().AsNoTracking()
                    join tenant in _context.Tenants.IgnoreQueryFilters().AsNoTracking()
                        on g.TenantId equals tenant.Id
                    where g.StaticJoinCode == normalizedCode
                    select new GroupJoinCodeLookup(
                        g.Id, g.TenantId, g.IsActive, tenant.Status,
                        g.StaticJoinEnabled, g.DynamicJoinEnabled,
                        g.StaticJoinCode, g.DynamicJoinSecret))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<bool> NameExistsExceptAsync(string name, Guid excludeGroupId, CancellationToken cancellationToken)
        {
            return _context.Groups.AnyAsync(g => g.Name == name && g.Id != excludeGroupId, cancellationToken);
        }

        public void TrackNewSlot(GroupScheduleSlot slot)
        {
            _context.GroupScheduleSlots.Add(slot);
        }

        public void RemoveSlots(IEnumerable<GroupScheduleSlot> slots)
        {
            _context.GroupScheduleSlots.RemoveRange(slots);
        }
    }
}
