// موجود (تعديل) — TeacherERP.Application/Features/Attendance/AdminScan/AdminScanCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Attendance;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.AdminScan
{
    public class AdminScanCommandHandler : IRequestHandler<AdminScanCommand, Result<AttendanceResultDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceRecorder _recorder;
        private readonly ILogger<AdminScanCommandHandler> _logger;

        public AdminScanCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository,
            IAttendanceRecorder recorder,
            ILogger<AdminScanCommandHandler> logger)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
            _recorder = recorder;
            _logger = logger;
        }

        public async Task<Result<AttendanceResultDto>> Handle(AdminScanCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } staffId)
            {
                return Reject(new Error("Auth.Unauthorized", "You must be logged in."), null, null, null, request.GroupId);
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Reject(TenantErrors.Required, null, staffId, null, request.GroupId);
            }

            var studentId = await _attendanceRepository.GetStudentIdByQrAsync(request.StudentQrCode.Trim(), cancellationToken);
            if (studentId is null)
            {
                return Reject(AttendanceErrors.StudentNotFound, tenant.TenantId, staffId, null, request.GroupId);
            }

            // المدرس ومساعدوه يسجّلون في كل الحصص الجارية، والموقع لا يُفحص (الماسح حاضر بنفسه)
            return await _recorder.RecordAsync(
                new AttendanceRequest(
                    tenant.TenantId, studentId.Value, ScanMethod.AdminScan, null, null,
                    RecordedByUserId: staffId, PreferredGroupId: request.GroupId,
                    EnforceGeofence: false, RegisterImmediately: true,
                    RestrictToGroupId: null),
                cancellationToken);
        }

        private Result<AttendanceResultDto> Reject(
            Error error, Guid? tenantId, Guid? actorUserId, Guid? studentId, Guid? groupId)
        {
            AttendanceScanLogging.Rejected(
                _logger, error, tenantId, actorUserId, studentId, groupId, null, ScanMethod.AdminScan);
            return Result<AttendanceResultDto>.Failure(error);
        }
    }
}
