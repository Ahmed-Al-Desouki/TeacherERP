// جديد — TeacherERP.Application/Features/Attendance/ScanAttendance/ScanAttendanceCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.ScanAttendance
{
    public class ScanAttendanceCommandHandler : IRequestHandler<ScanAttendanceCommand, Result<AttendanceResultDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceCodeService _codeService;
        private readonly IAttendanceRecorder _recorder;
        private readonly TimeProvider _timeProvider;

        public ScanAttendanceCommandHandler(
            ICurrentUserService currentUser,
            IAttendanceRepository attendanceRepository,
            IAttendanceCodeService codeService,
            IAttendanceRecorder recorder,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _attendanceRepository = attendanceRepository;
            _codeService = codeService;
            _recorder = recorder;
            _timeProvider = timeProvider;
        }

        public async Task<Result<AttendanceResultDto>> Handle(ScanAttendanceCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } studentId)
            {
                return Result<AttendanceResultDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            // الطالب ليس عضواً بعد، فالقراءة بالـ teacherId صراحةً. غير موجود أو موقوف أو كود غلط: نفس الرد
            var settings = await _attendanceRepository.GetSettingsAsync(request.TeacherId, cancellationToken);
            if (settings is null || settings.TenantStatus != TenantStatus.Active)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.InvalidCode);
            }

            if (settings.Mode == AttendanceMode.AdminOnly)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.SelfScanDisabled);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            if (!_codeService.IsValid(settings.Mode, settings.TenantId, settings.QrSecret, request.Code, now))
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.InvalidCode);
            }

            // الـ Flag يرسله التطبيق (لا يمكن التحقق منه من السيرفر)، فنأخذه كخط دفاع أول لا وحيد
            if (settings.RejectMockLocations && request.IsMockLocation)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.MockLocation);
            }

            return await _recorder.RecordAsync(
                new AttendanceRequest(
                    settings.TenantId, studentId, ScanMethod.SelfScan, request.Latitude, request.Longitude,
                    RecordedByUserId: null, PreferredGroupId: request.GroupId,
                    EnforceGeofence: true, RegisterImmediately: settings.AutoRegisterOnScan, RestrictToAssistantId: null),
                cancellationToken);
        }
    }
}