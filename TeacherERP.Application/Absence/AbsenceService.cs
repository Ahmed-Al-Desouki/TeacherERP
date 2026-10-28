// جديد — TeacherERP.Application/Absence/AbsenceService.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Absence
{
    public class AbsenceService : IAbsenceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly INotificationService _notificationService;
        private readonly IAuditRepository _auditRepository;
        private readonly IUserRepository _userRepository;

        public AbsenceService(
            IAttendanceRepository attendanceRepository,
            INotificationService notificationService,
            IAuditRepository auditRepository,
            IUserRepository userRepository)
        {
            _attendanceRepository = attendanceRepository;
            _notificationService = notificationService;
            _auditRepository = auditRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAbsenceAsync(TenantMember member, Guid sessionId, CancellationToken cancellationToken)
        {
            _attendanceRepository.AddRecord(AttendanceRecord.RecordAbsence(member.TenantId, sessionId, member.UserId));
            member.IncrementAbsence();

            if (member.Status != MembershipStatus.Active
                || member.ConsecutiveAbsences < AbsencePolicy.AutoSuspendAfterConsecutiveAbsences)
            {
                return false;
            }

            member.Suspend();

            var student = await _userRepository.GetByIdAsync(member.UserId, cancellationToken);
            var name = student?.FullName ?? "A student";
            var count = member.ConsecutiveAbsences;

            await _notificationService.NotifyStaffAsync(
                member.TenantId,
                NotificationType.StudentAutoSuspended,
                $"{name} was suspended after {count} consecutive absences.",
                member.UserId,
                cancellationToken);

            _auditRepository.Add(AuditLog.Create(
                member.TenantId, null, AuditAction.StudentAutoSuspended, member.UserId, $"{count} consecutive absences"));

            return true;
        }
    }
}