// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/StudentRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    // طلاب المدرس الحالي (الفلتر العام يعمل). بدون موبايل ولا ولي أمر ولا مالية عمداً
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<StudentListItem>> GetStudentsPageAsync(
            MembershipStatus? status, Guid? groupId, PageRequest page, CancellationToken cancellationToken)
        {
            var activeEnrollments = _context.GroupEnrollments
                .AsNoTracking()
                .Where(e => e.Status == EnrollmentStatus.Active);

            var query = from m in _context.TenantMembers.AsNoTracking()
                        join e in activeEnrollments on m.UserId equals e.StudentId into enrollments
                        from e in enrollments.DefaultIfEmpty()
                        where m.Role == TenantRole.Student
                              && (status == null || m.Status == status)
                              && (groupId == null || e.GroupId == groupId)
                        orderby m.User.FullName, m.UserId
                        select new
                        {
                            m.UserId,
                            m.User.FullName,
                            m.Status,
                            m.ConsecutiveAbsences,
                            GroupId = (Guid?)e.GroupId,
                            EnrolledAt = (DateTime?)e.EnrolledAt
                        };

            var rows = await query.ToPagedAsync(page, cancellationToken);

            // أسماء المجموعات لصفحة الطلاب الحالية فقط
            var groupIds = rows.Items.Where(r => r.GroupId.HasValue).Select(r => r.GroupId!.Value).Distinct().ToArray();
            var groupNames = groupIds.Length == 0
                ? new Dictionary<Guid, string>()
                : await _context.Groups
                    .AsNoTracking()
                    .Where(g => groupIds.Contains(g.Id))
                    .ToDictionaryAsync(g => g.Id, g => g.Name, cancellationToken);

            return rows.Map(r => new StudentListItem(
                r.UserId,
                r.FullName,
                r.Status,
                r.ConsecutiveAbsences,
                r.GroupId,
                r.GroupId.HasValue && groupNames.TryGetValue(r.GroupId.Value, out var name) ? name : null,
                r.EnrolledAt));
        }
    }
}
