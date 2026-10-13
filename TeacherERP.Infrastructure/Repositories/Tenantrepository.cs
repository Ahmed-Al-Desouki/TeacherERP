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

        public Task<TenantMembershipInfo?> GetMembershipAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .AsNoTracking()
                .Where(m => m.UserId == userId && m.TenantId == tenantId)
                .Select(m => new TenantMembershipInfo(m.TenantId, m.Tenant.Name, m.Role, m.Tenant.Status))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<TenantMembershipInfo>> GetMembershipsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .AsNoTracking()
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Tenant.Name)
                .ThenBy(m => m.TenantId)
                .Select(m => new TenantMembershipInfo(m.TenantId, m.Tenant.Name, m.Role, m.Tenant.Status))
                .ToListAsync(cancellationToken);
        }
    }
}