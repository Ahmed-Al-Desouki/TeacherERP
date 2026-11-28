// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/StudentRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    // بيانات الطلاب تظل محكومة بعضوية الطالب في مساحة العمل الحالية.
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
                            m.User.ProfileImageUrl,
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
                r.ProfileImageUrl,
                r.Status,
                r.ConsecutiveAbsences,
                r.GroupId,
                r.GroupId.HasValue && groupNames.TryGetValue(r.GroupId.Value, out var name) ? name : null,
                r.EnrolledAt));
        }

        public async Task<StudentDetailsItem?> GetStudentDetailsAsync(
            Guid studentId, CancellationToken cancellationToken)
        {
            var student = await _context.TenantMembers
                .AsNoTracking()
                .Where(member => member.UserId == studentId && member.Role == TenantRole.Student)
                .Select(member => new
                {
                    member.UserId,
                    member.Status,
                    member.ConsecutiveAbsences,
                    member.User.FullName,
                    member.User.ProfileImageUrl,
                    member.User.Email,
                    member.User.PhoneNumber,
                    member.User.Gender,
                    member.User.DateOfBirth,
                    GuardianName = member.User.Guardian == null ? null : member.User.Guardian.Name,
                    GuardianPhoneNumber = member.User.Guardian == null ? null : member.User.Guardian.PhoneNumber,
                    GuardianRelation = member.User.Guardian == null
                    ? (TeacherERP.Domain.ValueObjects.GuardianRelation?)null
                        : member.User.Guardian.Relation
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (student is null)
            {
                return null;
            }

            var activeGroups = await (from enrollment in _context.GroupEnrollments.AsNoTracking()
                join studentGroup in _context.Groups.AsNoTracking()
                    on enrollment.GroupId equals studentGroup.Id
                                      where enrollment.StudentId == studentId
                                            && enrollment.Status == EnrollmentStatus.Active
                orderby studentGroup.Name, enrollment.EnrolledAt
                                      select new StudentActiveGroupItem(
                    studentGroup.Id, studentGroup.Name, enrollment.EnrolledAt))
                .ToListAsync(cancellationToken);

            var completedProfileFields = 0;
            if (!string.IsNullOrWhiteSpace(student.PhoneNumber)) completedProfileFields++;
            if (!string.IsNullOrWhiteSpace(student.GuardianName)) completedProfileFields++;
            if (!string.IsNullOrWhiteSpace(student.GuardianPhoneNumber)) completedProfileFields++;
            if (student.GuardianRelation.HasValue) completedProfileFields++;

            return new StudentDetailsItem(
                student.UserId,
                student.FullName,
                student.ProfileImageUrl,
                student.Email,
                student.PhoneNumber,
                student.Gender,
                student.DateOfBirth,
                student.Status,
                student.ConsecutiveAbsences,
                student.GuardianName,
                student.GuardianPhoneNumber,
                student.GuardianRelation,
                completedProfileFields * 100 / 4,
                activeGroups);
        }
    }
}
