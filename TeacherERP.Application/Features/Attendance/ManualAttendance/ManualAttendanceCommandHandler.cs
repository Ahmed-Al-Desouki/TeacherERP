// جديد — TeacherERP.Application/Features/Attendance/ManualAttendance/ManualAttendanceCommandHandler.cs
using System.Data;
using MediatR;
using TeacherERP.Application.Absence;
using TeacherERP.Application.Attendance;
using TeacherERP.Application.Features.Enrollment;
using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Features.Schedule;
using TeacherERP.Application.Features.Students;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.ManualAttendance
{
    public class ManualAttendanceCommandHandler : IRequestHandler<ManualAttendanceCommand, Result<AttendanceResultDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceRecorder _recorder;
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public ManualAttendanceCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            IGroupRepository groupRepository,
            IEnrollmentRepository enrollmentRepository,
            IAttendanceRepository attendanceRepository,
            IAttendanceRecorder recorder,
            IAbsenceRepository absenceRepository,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _groupRepository = groupRepository;
            _enrollmentRepository = enrollmentRepository;
            _attendanceRepository = attendanceRepository;
            _recorder = recorder;
            _absenceRepository = absenceRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<AttendanceResultDto>> Handle(ManualAttendanceCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } staffId)
            {
                return Result<AttendanceResultDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<AttendanceResultDto>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن الحصة تخص المدرس الحالي فقط
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session is null)
            {
                return Result<AttendanceResultDto>.Failure(ScheduleErrors.NotFound);
            }

            if (session.IsCancelled)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.SessionCancelled);
            }

            // طالب من قائمتي فقط (غير المسجّل يستخدم admin-scan بكود الكارت)
            var member = await _enrollmentRepository.GetMemberAsync(tenant.TenantId, request.StudentId, cancellationToken);
            if (member is null || member.Role != TenantRole.Student)
            {
                return Result<AttendanceResultDto>.Failure(StudentErrors.NotFound);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            if (now < session.StartTime.AddMinutes(-AttendancePolicy.EarlyCheckInMinutes))
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.SessionNotStarted);
            }

            // حصة جارية: نفس قواعد التسجيل (الإيقاف، التعويض، التسجيل التلقائي، تصفير العداد) من غير فحص موقع
            if (now <= session.EndTime)
            {
                return await _recorder.RecordAsync(
                    new AttendanceRequest(
                        tenant.TenantId, request.StudentId, ScanMethod.ManualEntry, null, null,
                        RecordedByUserId: staffId, PreferredGroupId: session.GroupId,
                        EnforceGeofence: false, RegisterImmediately: true,
                        RestrictToGroupId: session.GroupId),
                    cancellationToken);
            }

            return await CorrectAsync(request, session, member, tenant.TenantId, staffId, now, cancellationToken);
        }

        // حصة انتهت: تحويل الغياب (أو الإدخال المتأخر) إلى حضور، مع Audit وإعادة حساب العداد
        private async Task<Result<AttendanceResultDto>> CorrectAsync(
            ManualAttendanceCommand request,
            Session session,
            TenantMember member,
            Guid tenantId,
            Guid staffId,
            DateTime now,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.ReasonRequired);
            }

            if (now > session.EndTime.AddHours(AttendancePolicy.ManualCorrectionWindowHours))
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.CorrectionWindowClosed);
            }

            if (member.Status == MembershipStatus.Blacklisted)
            {
                return Result<AttendanceResultDto>.Failure(EnrollmentErrors.NotAllowed);
            }

            var group = await _groupRepository.GetWithSlotsAsync(session.GroupId, cancellationToken);
            if (group is null)
            {
                return Result<AttendanceResultDto>.Failure(GroupErrors.NotFound);
            }

            if (!group.IsActive)
            {
                return Result<AttendanceResultDto>.Failure(GroupErrors.Inactive);
            }

            if (!await _absenceRepository.WasEnrolledAtAsync(session, request.StudentId, cancellationToken))
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.StudentNotInGroup);
            }

            var existing = await _attendanceRepository.GetRecordAsync(session.Id, request.StudentId, cancellationToken);

            // التكرار ينجح بدون تغيير (Idempotent)
            if (existing is { Status: AttendanceStatus.Present })
            {
                return Result<AttendanceResultDto>.Success(
                    new AttendanceResultDto(AttendanceOutcome.AlreadyRecorded, session.Id, group.Name, existing.Status));
            }

            if (existing is not null && existing.Status != AttendanceStatus.Absent)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.AlreadyHasRecord);
            }

            var reason = $"{request.Reason.Trim()} [session {session.Id}]";

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (existing is null)
                {
                    // إدخال متأخر لحصة لم يُحسب لها غياب بعد (أو الطالب كان موقوفاً وقتها)
                    _attendanceRepository.AddRecord(AttendanceRecord.RecordPresence(
                        tenantId, session.Id, request.StudentId, ScanMethod.ManualEntry, null, null, staffId));
                }
                else
                {
                    var tracked = await _attendanceRepository.GetTrackedRecordAsync(existing.Id, cancellationToken);
                    tracked!.CorrectToPresent(staffId);
                }

                // نحفظ أولاً، لتحسب إعادة الحساب بعدها الحضور الجديد
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var previous = member.ConsecutiveAbsences;
                var suspendedByAbsence = member.Status == MembershipStatus.Suspended
                                         && previous >= AbsencePolicy.AutoSuspendAfterConsecutiveAbsences;

                // العداد = الغيابات بعد آخر حضور، ولا يزيد عن قيمته الحالية أبداً
                var trailing = await _absenceRepository.CountTrailingAbsencesAsync(request.StudentId, cancellationToken);
                member.SetConsecutiveAbsences(Math.Min(trailing, previous));

                _auditRepository.Add(AuditLog.Create(tenantId, staffId, AuditAction.AttendanceCorrected, request.StudentId, reason));

                // الشرط اللي أوقفه (4 غيابات متتالية) لم يعد قائماً
                if (suspendedByAbsence && member.ConsecutiveAbsences < AbsencePolicy.AutoSuspendAfterConsecutiveAbsences)
                {
                    member.Activate();
                    _auditRepository.Add(AuditLog.Create(
                        tenantId, staffId, AuditAction.StudentReinstatedByCorrection, request.StudentId, reason));
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<AttendanceResultDto>.Success(
                    new AttendanceResultDto(AttendanceOutcome.Corrected, session.Id, group.Name, AttendanceStatus.Present));
            }, IsolationLevel.ReadCommitted, cancellationToken);
        }
    }
}