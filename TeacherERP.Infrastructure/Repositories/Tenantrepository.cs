using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _context;

        public TenantRepository(AppDbContext context)
        {
            _context = context;
        }

        // IgnoreQueryFilters مقصود في الاستعلامين: هما اللذان يحددان السنتر نفسه (قبل أن يتحدد)،
        // وأي استعلام آخر على TenantMembers داخل سنتر يتفلتر تلقائياً بالفلتر العام.
        public Task<TenantMembershipInfo?> GetMembershipAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(m => m.UserId == userId && m.TenantId == tenantId)
                .Select(m => new TenantMembershipInfo(m.TenantId, m.Tenant.Name, m.Role, m.Status, m.Tenant.Status))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<TenantMembershipInfo>> GetMembershipsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Tenant.Name)
                .ThenBy(m => m.TenantId)
                .Select(m => new TenantMembershipInfo(m.TenantId, m.Tenant.Name, m.Role, m.Status, m.Tenant.Status))
                .ToListAsync(cancellationToken);
        }
    }
}