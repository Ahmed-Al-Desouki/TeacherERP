// جديد — TeacherERP.Infrastructure/Repositories/GroupAdminRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    // داخل المدرس الحالي (الفلتر العام يعمل على الجداول)
    public class GroupAdminRepository : IGroupAdminRepository
    {
        private readonly AppDbContext _context;

        public GroupAdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<int> CountAdminsAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return _context.GroupEnrollments.CountAsync(
                e => e.GroupId == groupId && e.Status == EnrollmentStatus.Active && e.IsGroupAdmin,
                cancellationToken);
        }

        public Task<List<GroupAdminItem>> GetAdminsAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return _context.GroupEnrollments
                .AsNoTracking()
                .Where(e => e.GroupId == groupId && e.Status == EnrollmentStatus.Active && e.IsGroupAdmin)
                .OrderBy(e => e.GroupAdminSince)
                .ThenBy(e => e.StudentId)
                .Select(e => new GroupAdminItem(e.StudentId, e.Student.FullName, e.GroupAdminSince))
                .ToListAsync(cancellationToken);
        }

        public void AddRequest(GroupAdminRequest request) => _context.GroupAdminRequests.Add(request);

        public Task<GroupAdminRequest?> GetTrackedRequestAsync(Guid requestId, CancellationToken cancellationToken)
        {
            return _context.GroupAdminRequests.FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
        }

        public Task<bool> HasPendingRequestAsync(Guid groupId, Guid studentId, CancellationToken cancellationToken)
        {
            return _context.GroupAdminRequests.AnyAsync(
                r => r.GroupId == groupId && r.StudentId == studentId && r.Status == GroupAdminRequestStatus.Pending,
                cancellationToken);
        }

        public Task<PagedResult<GroupAdminRequestItem>> GetRequestsPageAsync(
            GroupAdminRequestStatus? status, PageRequest page, CancellationToken cancellationToken)
        {
            // اسم الطالب والمجموعة من العلاقات، واسم المساعد بـ Left Join (الحساب ممكن يتمسح)
            var query = from r in _context.GroupAdminRequests.AsNoTracking()
                        join u in _context.Users.AsNoTracking() on r.RequestedByUserId equals u.Id into requesters
                        from u in requesters.DefaultIfEmpty()
                        where status == null || r.Status == status
                        orderby r.CreatedAt descending, r.Id
                        select new GroupAdminRequestItem(
                            r.Id,
                            r.GroupId,
                            r.Group.Name,
                            r.StudentId,
                            r.Student.FullName,
                            r.Type,
                            r.Status,
                            r.RequestedByUserId,
                            u == null ? null : u.FullName,
                            r.CreatedAt,
                            r.DecisionNote);

            return query.ToPagedAsync(page, cancellationToken);
        }
    }
}