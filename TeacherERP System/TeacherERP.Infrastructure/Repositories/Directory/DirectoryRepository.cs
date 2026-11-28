// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/DirectoryRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

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

        public async Task<DirectorySearchPage> SearchAsync(string term, PageRequest page, CancellationToken cancellationToken)
        {
            var matching = from link in _context.CenterTeachers.AsNoTracking()
                           join center in _context.Centers.AsNoTracking() on link.CenterId equals center.Id
                           join tenant in _context.Tenants.AsNoTracking() on link.TenantId equals tenant.Id
                           where link.Status == CenterTeacherStatus.Active
                                 && tenant.Status == TenantStatus.Active
                                 && tenant.IsListedInSearch
                                 && (center.Name.Contains(term)
                                     || tenant.Name.Contains(term)
                                     || (tenant.Subject != null && tenant.Subject.Contains(term)))
                           select new
                           {
                               CenterId = center.Id,
                               CenterName = center.Name,
                               CenterAddress = center.Address,
                               TeacherId = tenant.Id,
                               TeacherName = tenant.Name,
                               tenant.Subject
                           };

            // 1) صفحة السناتر المطابقة (Distinct فلا يتكرر سنتر بسبب كثرة مدرسيه)
            var centers = await matching
                .Select(m => new { m.CenterId, m.CenterName })
                .Distinct()
                .OrderBy(c => c.CenterName)
                .ThenBy(c => c.CenterId)
                .ToPagedAsync(page, cancellationToken);

            if (centers.Items.Count == 0)
            {
                return new DirectorySearchPage(Array.Empty<DirectoryRow>(), centers.TotalCount);
            }

            // 2) مدرسو هذه السناتر فقط
            var centerIds = centers.Items.Select(c => c.CenterId).ToArray();

            var rows = await matching
                .Where(m => centerIds.Contains(m.CenterId))
                .OrderBy(m => m.CenterName)
                .ThenBy(m => m.CenterId)
                .ThenBy(m => m.TeacherName)
                .ThenBy(m => m.TeacherId)
                .Select(m => new DirectoryRow(m.CenterId, m.CenterName, m.CenterAddress, m.TeacherId, m.TeacherName, m.Subject))
                .ToListAsync(cancellationToken);

            return new DirectorySearchPage(rows, centers.TotalCount);
        }

        public async Task<TeacherProfileData?> GetTeacherProfileAsync(
            Guid teacherId, string? gradeLevel, PageRequest page, CancellationToken cancellationToken)
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
                                 orderby center.Name, center.Id
                                 select new DirectoryCenterInfo(center.Id, center.Name, center.Address))
                .ToListAsync(cancellationToken);

            var groups = await _context.Groups
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(g => g.Slots)
                .Where(g => g.TenantId == teacherId && g.IsActive && (gradeLevel == null || g.GradeLevel == gradeLevel))
                .OrderBy(g => g.Name)
                .ThenBy(g => g.Id)
                .ToPagedAsync(page, cancellationToken);

            // عدد المسجلين لمجموعات هذه الصفحة فقط
            var groupIds = groups.Items.Select(g => g.Id).ToArray();

            var counts = groupIds.Length == 0
                ? new Dictionary<Guid, int>()
                : (await _context.GroupEnrollments
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .Where(e => e.TenantId == teacherId && e.Status == EnrollmentStatus.Active && groupIds.Contains(e.GroupId))
                        .GroupBy(e => e.GroupId)
                        .Select(g => new { GroupId = g.Key, Count = g.Count() })
                        .ToListAsync(cancellationToken))
                    .ToDictionary(c => c.GroupId, c => c.Count);

            return new TeacherProfileData(
                tenant.Id,
                tenant.Name,
                tenant.Subject,
                tenant.TimeZoneId,
                centers,
                groups,
                counts);
        }
    }
}
