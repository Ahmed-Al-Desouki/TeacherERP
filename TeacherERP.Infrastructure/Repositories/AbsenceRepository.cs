// جديد — TeacherERP.Infrastructure/Repositories/AbsenceRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    // داخل المدرس الحالي: الفلتر العام يعمل على كل الجداول (السنتر محدد من الـ Job أو الـ Policy)
    public class AbsenceRepository : IAbsenceRepository
    {
        private readonly AppDbContext _context;

        public AbsenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Session>> GetUnprocessedEndedSessionsAsync(DateTime cutoffUtc, int take, CancellationToken cancellationToken)
        {
            return _context.Sessions
                .Where(s => s.AbsencesProcessedAt == null && s.EndTime <= cutoffUtc)
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public Task<List<Guid>> GetAbsentCandidateIdsAsync(Session session, CancellationToken cancellationToken)
        {
            var sessionId = session.Id;
            var groupId = session.GroupId;
            var startTime = session.StartTime;

            // مسجّل في المجموعة وقت الحصة (المجموعة المؤرشفة لا يُحسب لها غياب)، بلا سجل في الحصة نفسها،
            // وبلا تعويض معلّق أو معتمد يغطيها (التعويض المرفوض لا يغطي)
            return _context.GroupEnrollments
                .AsNoTracking()
                .Where(e => e.GroupId == groupId
                            && e.Group.IsActive
                            && e.EnrolledAt <= startTime
                            && (e.EndedAt == null || e.EndedAt > startTime))
                .Where(e => !_context.AttendanceRecords.Any(a =>
                    a.StudentId == e.StudentId
                    && (a.SessionId == sessionId
                        || (a.CompensatesSessionId == sessionId
                            && (a.Status == AttendanceStatus.PendingCompensation || a.Status == AttendanceStatus.Present)))))
                .Select(e => e.StudentId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public Task<List<Guid>> GetPresentStudentIdsAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            return _context.AttendanceRecords
                .AsNoTracking()
                .Where(a => a.SessionId == sessionId && a.Status == AttendanceStatus.Present)
                .Select(a => a.StudentId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public Task<List<TenantMember>> GetTrackedStudentMembersAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken)
        {
            var ids = studentIds.ToArray();

            return _context.TenantMembers
                .Where(m => m.Role == TenantRole.Student && ids.Contains(m.UserId))
                .ToListAsync(cancellationToken);
        }
    }
}