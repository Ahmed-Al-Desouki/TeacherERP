using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    public class CenterRepository : ICenterRepository
    {
        private readonly AppDbContext _context;

        public CenterRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Center>> GetMineAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.Centers
                .AsNoTracking()
                .Where(center => center.OwnerUserId == userId
                    || _context.CenterTeachers.Any(link =>
                        link.CenterId == center.Id
                        && link.Status == CenterTeacherStatus.Active
                        && _context.Tenants.Any(tenant => tenant.Id == link.TenantId && tenant.OwnerId == userId)))
                .OrderBy(center => center.Name)
                .ThenBy(center => center.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<bool> IsActiveTeacherAtAsync(Guid centerId, Guid tenantId, CancellationToken cancellationToken)
        {
            return _context.CenterTeachers.AnyAsync(
                link => link.CenterId == centerId
                        && link.TenantId == tenantId
                        && link.Status == CenterTeacherStatus.Active,
                cancellationToken);
        }

        public Task<Center?> GetByIdAsync(Guid centerId, CancellationToken cancellationToken)
        {
            return _context.Centers.FirstOrDefaultAsync(center => center.Id == centerId, cancellationToken);
        }
    }
}