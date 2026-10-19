// جديد — TeacherERP.Infrastructure/Repositories/DirectoryRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    // دليل الطلاب: IgnoreQueryFilters مقصود لأن الطالب يبحث عبر المدرسين، ويقتصر على الظاهرين في البحث والنشطين
    public class DirectoryRepository : IDirectoryRepository
    {
        private readonly AppDbContext _context;

        public DirectoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<DirectoryRow>> SearchAsync(string term, int maxRows, CancellationToken cancellationToken)
        {
            return (from link in _context.CenterTeachers.AsNoTracking()
                    join center in _context.Centers.AsNoTracking() on link.CenterId equals center.Id
                    join tenant in _context.Tenants.AsNoTracking() on link.TenantId equals tenant.Id
                    where link.Status == CenterTeacherStatus.Active
                          && tenant.Status == TenantStatus.Active
                          && tenant.IsListedInSearch
                          && (center.Name.Contains(term)
                              || tenant.Name.Contains(term)
                              || (tenant.Subject != null && tenant.Subject.Contains(term)))
                    orderby center.Name, tenant.Name
                    select new DirectoryRow(center.Id, center.Name, center.Address, tenant.Id, tenant.Name, tenant.Subject))
                .Distinct() // --> من عندي دي عشان مش يكرر الصفوف لو المدرس مرتبط بأكثر من مركز
                .Take(maxRows)
                .ToListAsync(cancellationToken);
        }

        public async Task<TeacherProfileData?> GetTeacherProfileAsync(Guid teacherId, string? gradeLevel, CancellationToken cancellationToken)
        {
            var tenant = await _context.Tenants
                .AsNoTracking()
                .Where(t => t.Id == teacherId && t.Status == TenantStatus.Active && t.IsListedInSearch)
                .Select(t => new { t.Id, t.Name, t.Subject, t.TimeZoneId })
                .FirstOrDefaultAsync(cancellationToken);

            if (tenant is null)
            {
                return null;
            }

            var centers = await (from link in _context.CenterTeachers.AsNoTracking()
                                 join center in _context.Centers.AsNoTracking() on link.CenterId equals center.Id
                                 where link.TenantId == teacherId && link.Status == CenterTeacherStatus.Active
                                 orderby center.Name
                                 select new DirectoryCenterInfo(center.Id, center.Name, center.Address))
                .ToListAsync(cancellationToken);

            var groups = await _context.Groups
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(g => g.Slots)
                .Where(g => g.TenantId == teacherId && g.IsActive && (gradeLevel == null || g.GradeLevel == gradeLevel))
                .OrderBy(g => g.Name)
                .ToListAsync(cancellationToken);

            var counts = await _context.GroupEnrollments
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(e => e.TenantId == teacherId && e.Status == EnrollmentStatus.Active)
                .GroupBy(e => e.GroupId)
                .Select(g => new { GroupId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return new TeacherProfileData(
                tenant.Id,
                tenant.Name,
                tenant.Subject,
                tenant.TimeZoneId,
                centers,
                groups,
                counts.ToDictionary(c => c.GroupId, c => c.Count));
        }
    }
}