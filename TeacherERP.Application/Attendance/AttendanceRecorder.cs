// جديد — TeacherERP.Application/Attendance/AttendanceRecorder.cs
using TeacherERP.Application.Features.Attendance;
using TeacherERP.Application.Features.Enrollment;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Attendance
{
    public class AttendanceRecorder : IAttendanceRecorder
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IEnrollmentService _enrollmentService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public AttendanceRecorder(
            IAttendanceRepository attendanceRepository,
            IEnrollmentRepository enrollmentRepository,
            IEnrollmentService enrollmentService,
            INotificationService notificationService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _attendanceRepository = attendanceRepository;
            _enrollmentRepository = enrollmentRepository;
            _enrollmentService = enrollmentService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<AttendanceResultDto>> RecordAsync(AttendanceRequest request, CancellationToken cancellationToken)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            var running = await _attendanceRepository.GetRunningSessionsAsync(
                request.TenantId, now, AttendancePolicy.EarlyCheckInMinutes, request.RestrictToAssistantId, cancellationToken);
            if (running.Count == 0)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.NoSessionNow);
            }

            // حالة الطالب عند هذا المدرس: المحظور مرفوض، والموقوف (غياب) لا يحضر حتى يفك المدرس إيقافه
            var member = await _enrollmentRepository.GetMemberAsync(request.TenantId, request.StudentId, cancellationToken);
            if (member is { Status: MembershipStatus.Blacklisted })
            {
                return Result<AttendanceResultDto>.Failure(EnrollmentErrors.NotAllowed);
            }

            if (member is { Status: MembershipStatus.Suspended })
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.Suspended);
            }

            var active = await _enrollmentRepository.GetActiveEnrollmentAsync(request.TenantId, request.StudentId, cancellationToken);

            // 1) اختيار الحصة (بدون أي أثر جانبي)
            var session = active is null ? null : running.FirstOrDefault(s => s.GroupId == active.GroupId);
            var isOwnGroup = session is not null;

            if (session is null)
            {
                if (request.PreferredGroupId is { } preferred)
                {
                    session = running.FirstOrDefault(s => s.GroupId == preferred);
                    if (session is null)
                    {
                        return Result<AttendanceResultDto>.Failure(AttendanceErrors.NoSessionNow);
                    }
                }
                else if (running.Count == 1)
                {
                    session = running[0];
                }
                else
                {
                    return Result<AttendanceResultDto>.Failure(AttendanceErrors.ChooseGroup);
                }
            }

            // 2) الحماية الجغرافية (من موقع سنتر المجموعة)
            if (request.EnforceGeofence)
            {
                if (session.Latitude is not { } centerLat || session.Longitude is not { } centerLon)
                {
                    return Result<AttendanceResultDto>.Failure(AttendanceErrors.CenterLocationNotSet);
                }

                if (request.Latitude is not { } lat || request.Longitude is not { } lon)
                {
                    return Result<AttendanceResultDto>.Failure(AttendanceErrors.LocationRequired);
                }

                if (GeoDistance.Meters(lat, lon, centerLat, centerLon) > session.GeofenceRadiusMeters)
                {
                    return Result<AttendanceResultDto>.Failure(AttendanceErrors.OutOfRange);
                }
            }

            // 3) تكرار المسح ينجح بدون تغيير (Idempotent)
            var existing = await _attendanceRepository.GetRecordAsync(session.SessionId, request.StudentId, cancellationToken);
            if (existing is not null)
            {
                return Result<AttendanceResultDto>.Success(
                    new AttendanceResultDto(AttendanceOutcome.AlreadyRecorded, session.SessionId, session.GroupName, existing.Status));
            }

            var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
            var studentName = student?.FullName ?? "A student";

            // 4) طالب غير مسجل (Walk-in): تسجيل فوري لو مسموح، وإلا طلب معلّق ينتظر الموافقة
            if (active is null)
            {
                if (!request.RegisterImmediately)
                {
                    if (!await _enrollmentRepository.HasPendingRequestAsync(request.TenantId, request.StudentId, cancellationToken))
                    {
                        var pending = EnrollmentRequest.Create(request.TenantId, request.StudentId, session.GroupId, EnrollmentRequestType.Join);
                        _enrollmentRepository.AddRequest(pending);

                        await _notificationService.NotifyStaffAsync(
                            request.TenantId, session.GroupId, NotificationType.EnrollmentRequested,
                            $"{studentName} asked to join {session.GroupName} by scanning the code.", pending.Id, cancellationToken);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }

                    return Result<AttendanceResultDto>.Success(
                        new AttendanceResultDto(AttendanceOutcome.PendingApproval, session.SessionId, session.GroupName, null));
                }

                var joinRequest = EnrollmentRequest.Create(request.TenantId, request.StudentId, session.GroupId, EnrollmentRequestType.Join);
                _enrollmentRepository.AddRequest(joinRequest);

                var approved = await _enrollmentService.ApproveAsync(joinRequest, request.RecordedByUserId, cancellationToken);
                if (approved.IsFailure)
                {
                    return Result<AttendanceResultDto>.Failure(approved.Error);
                }

                if (request.Method == ScanMethod.SelfScan)
                {
                    await _notificationService.NotifyStaffAsync(
                        request.TenantId, session.GroupId, NotificationType.StudentAutoRegistered,
                        $"{studentName} was registered in {session.GroupName} by scanning the code.", joinRequest.Id, cancellationToken);
                }
            }

            // 5) حضور عادي لمجموعته، أو تعويض (معلّق) لو الحصة لمجموعة أخرى عند نفس المدرس
            var status = AttendanceStatus.Present;
            Guid? compensates = null;

            if (active is not null && !isOwnGroup)
            {
                status = AttendanceStatus.PendingCompensation;
                compensates = await _attendanceRepository.FindCompensableSessionAsync(
                    request.TenantId, active.GroupId, request.StudentId, now, AttendancePolicy.CompensationWindowDays, cancellationToken);
            }

            var record = AttendanceRecord.RecordPresence(
                request.TenantId, session.SessionId, request.StudentId, request.Method,
                request.Latitude, request.Longitude, request.RecordedByUserId, status, compensates);
            _attendanceRepository.AddRecord(record);

            if (status == AttendanceStatus.Present)
            {
                // الحضور يصفّر عداد الغياب المتتالي عند هذا المدرس
                var tracked = await _enrollmentRepository.GetMemberAsync(request.TenantId, request.StudentId, cancellationToken);
                if (tracked is { ConsecutiveAbsences: > 0 })
                {
                    tracked.ResetAbsence();
                }
            }
            else
            {
                await _notificationService.NotifyStaffAsync(
                    request.TenantId, session.GroupId, NotificationType.CompensationRequested,
                    $"{studentName} attended {session.GroupName} as a compensation session.", record.Id, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var outcome = status == AttendanceStatus.Present ? AttendanceOutcome.Recorded : AttendanceOutcome.PendingCompensation;
            return Result<AttendanceResultDto>.Success(new AttendanceResultDto(outcome, session.SessionId, session.GroupName, status));
        }
    }
}