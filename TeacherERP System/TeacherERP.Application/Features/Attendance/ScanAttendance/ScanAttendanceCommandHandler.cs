// موجود (تعديل) — TeacherERP.Application/Features/Attendance/ScanAttendance/ScanAttendanceCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Attendance;
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
        private readonly ILogger<ScanAttendanceCommandHandler> _logger;

        public ScanAttendanceCommandHandler(
            ICurrentUserService currentUser,
            IAttendanceRepository attendanceRepository,
            IAttendanceCodeService codeService,
            IAttendanceRecorder recorder,
            TimeProvider timeProvider,
            ILogger<ScanAttendanceCommandHandler> logger)
        {
            _currentUser = currentUser;
            _attendanceRepository = attendanceRepository;
            _codeService = codeService;
            _recorder = recorder;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task<Result<AttendanceResultDto>> Handle(ScanAttendanceCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } studentId)
            {
                return Reject(request, new Error("Auth.Unauthorized", "You must be logged in."), null);
            }

            // الطالب ليس عضواً بعد، فالقراءة بالـ teacherId صراحةً. غير موجود أو موقوف أو كود غلط: نفس الرد
            var settings = await _attendanceRepository.GetSettingsAsync(request.TeacherId, cancellationToken);
            if (settings is null || settings.TenantStatus != TenantStatus.Active)
            {
                return Reject(request, AttendanceErrors.InvalidCode, studentId);
            }

            if (settings.Mode == AttendanceMode.AdminOnly)
            {
                return Reject(request, AttendanceErrors.SelfScanDisabled, studentId);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            if (!_codeService.IsValid(settings.Mode, settings.TenantId, settings.QrSecret, request.Code, now))
            {
                return Reject(request, AttendanceErrors.InvalidCode, studentId);
            }

            if (request.AccuracyMeters is not { } accuracy)
            {
                return Reject(request, AttendanceErrors.LocationAccuracyRequired, studentId);
            }

            if (accuracy > AttendancePolicy.MaximumLocationAccuracyMeters)
            {
                return Reject(request, AttendanceErrors.LocationAccuracyInsufficient, studentId);
            }

            // الـ Flag يرسله التطبيق (لا يمكن التحقق منه من السيرفر)، فنأخذه كخط دفاع أول لا وحيد
            if (settings.RejectMockLocations && request.IsMockLocation)
            {
                return Reject(request, AttendanceErrors.MockLocation, studentId);
            }

            return await _recorder.RecordAsync(
                new AttendanceRequest(
                    settings.TenantId, studentId, ScanMethod.SelfScan, request.Latitude, request.Longitude,
                    RecordedByUserId: null, PreferredGroupId: request.GroupId,
                    EnforceGeofence: true, RegisterImmediately: settings.AutoRegisterOnScan, RestrictToGroupId: null,
                    AccuracyMeters: accuracy),
                cancellationToken);
        }

        private Result<AttendanceResultDto> Reject(
            ScanAttendanceCommand request, Error error, Guid? studentId)
        {
            AttendanceScanLogging.Rejected(
                _logger, error, request.TeacherId, studentId, studentId, request.GroupId,
                sessionId: null, method: ScanMethod.SelfScan);
            return Result<AttendanceResultDto>.Failure(error);
        }
    }
}
