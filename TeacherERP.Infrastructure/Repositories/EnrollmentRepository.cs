// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/EnrollmentRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---- عبر المدرسين بقصد (IgnoreQueryFilters) مع tenantId صريح، لأن الطالب ليس عضواً بعد ----

        public Task<GroupJoinInfo?> GetGroupJoinInfoAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return (from g in _context.Groups.IgnoreQueryFilters().AsNoTracking()
                    join t in _context.Tenants.AsNoTracking() on g.TenantId equals t.Id
                    where g.Id == groupId
                    select new GroupJoinInfo(g.Id, g.Name, g.TenantId, g.IsActive, g.Capacity, t.Status, t.AutoApproveJoinRequests))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<GroupEnrollment?> GetActiveEnrollmentAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken)
        {
            return _context.GroupEnrollments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    e => e.TenantId == tenantId && e.StudentId == studentId && e.Status == EnrollmentStatus.Active,
                    cancellationToken);
        }

        public Task<int> CountActiveAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken)
        {
            return _context.GroupEnrollments
                .IgnoreQueryFilters()
                .CountAsync(
                    e => e.TenantId == tenantId && e.GroupId == groupId && e.Status == EnrollmentStatus.Active,
                    cancellationToken);
        }

        public Task<TenantMember?> GetMemberAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.TenantId == tenantId && m.UserId == userId, cancellationToken);
        }

        public Task<bool> HasPendingRequestAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken)
        {
            return _context.EnrollmentRequests
                .IgnoreQueryFilters()
                .AnyAsync(
                    r => r.TenantId == tenantId && r.StudentId == studentId && r.Status == EnrollmentRequestStatus.Pending,
                    cancellationToken);
        }

        public void AddRequest(EnrollmentRequest request) => _context.EnrollmentRequests.Add(request);

        public void AddEnrollment(GroupEnrollment enrollment) => _context.GroupEnrollments.Add(enrollment);

        public void AddMember(TenantMember member) => _context.TenantMembers.Add(member);

        // ---- للموظفين داخل المدرس الحالي (الفلتر العام يعمل، والمدرس والمساعد يريان كل الطلبات) ----

        public Task<EnrollmentRequest?> GetTrackedRequestAsync(Guid requestId, CancellationToken cancellationToken)
        {
            return _context.EnrollmentRequests.FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
        }

        public async Task<PagedResult<EnrollmentRequestListItem>> GetRequestsPageAsync(
            EnrollmentRequestStatus? status, PageRequest page, CancellationToken cancellationToken)
        {
            var query = _context.EnrollmentRequests.AsNoTracking().AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            // العدّ قبل الإسقاط (الـ Select فيه Subquery للمجموعة الحالية) فيبقى الاستعلام بسيطاً
            var total = await query.CountAsync(cancellationToken);
            if (total == 0)
            {
                return new PagedResult<EnrollmentRequestListItem>(new List<EnrollmentRequestListItem>(), page.Page, page.PageSize, 0);
            }

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .ThenBy(r => r.Id)
                .Skip(page.Skip)
                .Take(page.PageSize)
                .Select(r => new EnrollmentRequestListItem(
                    r.Id,
                    r.StudentId,
                    r.Student.FullName,
                    r.GroupId,
                    r.Group.Name,
                    r.Type,
                    r.Status,
                    r.CreatedAt,
                    _context.GroupEnrollments
                        .Where(e => e.StudentId == r.StudentId && e.Status == EnrollmentStatus.Active)
                        .Select(e => e.Group.Name)
                        .FirstOrDefault()))
                .ToListAsync(cancellationToken);

            return new PagedResult<EnrollmentRequestListItem>(items, page.Page, page.PageSize, total);
        }

        // ---- للطالب عبر كل مدرسيه ----

        public Task<EnrollmentRequest?> GetOwnPendingRequestAsync(Guid requestId, Guid studentId, CancellationToken cancellationToken)
        {
            return _context.EnrollmentRequests
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    r => r.Id == requestId && r.StudentId == studentId && r.Status == EnrollmentRequestStatus.Pending,
                    cancellationToken);
        }

        public Task<List<MyEnrollmentItem>> GetMyEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return (from e in _context.GroupEnrollments.IgnoreQueryFilters().AsNoTracking()
                    join t in _context.Tenants.AsNoTracking() on e.TenantId equals t.Id
                    where e.StudentId == studentId && e.Status == EnrollmentStatus.Active
                    orderby e.EnrolledAt descending, e.Id
                    select new MyEnrollmentItem(e.Id, e.GroupId, e.Group.Name, t.Id, t.Name, t.Subject, e.EnrolledAt))
                .ToListAsync(cancellationToken);
        }

        public Task<PagedResult<MyRequestItem>> GetMyRequestsPageAsync(Guid studentId, PageRequest page, CancellationToken cancellationToken)
        {
            return (from r in _context.EnrollmentRequests.IgnoreQueryFilters().AsNoTracking()
                    where r.StudentId == studentId
                    orderby r.CreatedAt descending, r.Id
                    select new MyRequestItem(r.Id, r.GroupId, r.Group.Name, r.TenantId, r.Type, r.Status, r.CreatedAt, r.DecisionNote))
                .ToPagedAsync(page, cancellationToken);
        }
    }
}
