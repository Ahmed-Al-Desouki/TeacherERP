// جديد — TeacherERP.Application/Features/Attendance/AdminScan/AdminScanCommandHandler.cs
using MediatR;
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

        public AdminScanCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository,
            IAttendanceRecorder recorder)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
            _recorder = recorder;
        }

        public async Task<Result<AttendanceResultDto>> Handle(AdminScanCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } staffId)
            {
                return Result<AttendanceResultDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<AttendanceResultDto>.Failure(TenantErrors.Required);
            }

            var studentId = await _attendanceRepository.GetStudentIdByQrAsync(request.StudentQrCode.Trim(), cancellationToken);
            if (studentId is null)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.StudentNotFound);
            }

            // المساعد يسجّل في حصص مجموعاته المخصصة له فقط (ربط الحصة بالمساعد)، والموقع لا يُفحص (الماسح حاضر بنفسه)
            return await _recorder.RecordAsync(
                new AttendanceRequest(
                    tenant.TenantId, studentId.Value, ScanMethod.AdminScan, null, null,
                    RecordedByUserId: staffId, PreferredGroupId: request.GroupId,
                    EnforceGeofence: false, RegisterImmediately: true,
                    RestrictToAssistantId: tenant.Role == TenantRole.Owner ? null : staffId),
                cancellationToken);
        }
    }
}