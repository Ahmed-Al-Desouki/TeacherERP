// جديد — TeacherERP.Application/Features/GroupAdmins/GroupScan/GroupScanCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Attendance;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GroupScan
{
    public class GroupScanCommandHandler : IRequestHandler<GroupScanCommand, Result<AttendanceResultDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupAccessService _groupAccess;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceRecorder _recorder;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GroupScanCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupAccessService groupAccess,
            IAttendanceRepository attendanceRepository,
            IAttendanceRecorder recorder,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _groupAccess = groupAccess;
            _attendanceRepository = attendanceRepository;
            _recorder = recorder;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AttendanceResultDto>> Handle(GroupScanCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } adminId)
            {
                return Result<AttendanceResultDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<AttendanceResultDto>.Failure(TenantErrors.Required);
            }

            var group = await _groupAccess.GetAdminGroupIdAsync(cancellationToken);
            if (group.IsFailure)
            {
                return Result<AttendanceResultDto>.Failure(group.Error);
            }

            var studentId = await _attendanceRepository.GetStudentIdByQrAsync(request.StudentQrCode.Trim(), cancellationToken);
            if (studentId is null)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.StudentNotFound);
            }

            // الأدمن طالب عادي: ما يسجّلش حضور نفسه
            if (studentId == adminId)
            {
                return Result<AttendanceResultDto>.Failure(GroupAdminErrors.CannotRecordSelf);
            }

            var settings = await _attendanceRepository.GetSettingsAsync(tenant.TenantId, cancellationToken);
            if (settings is null)
            {
                return Result<AttendanceResultDto>.Failure(TenantErrors.Forbidden);
            }

            // الـ Flag يرسله التطبيق (لا يمكن التحقق منه من السيرفر)، فنأخذه كخط دفاع أول لا وحيد
            if (settings.RejectMockLocations && request.IsMockLocation)
            {
                return Result<AttendanceResultDto>.Failure(AttendanceErrors.MockLocation);
            }

            // الموافقة التلقائية (من إعدادات المدرس): الغير مسجّل يتسجّل فوراً، والتعويض يُعتمد فوراً. وإلا يبقوا معلّقين
            var autoApprove = settings.AutoApproveGroupAdminScans;

            var result = await _recorder.RecordAsync(
                new AttendanceRequest(
                    tenant.TenantId, studentId.Value, ScanMethod.AdminScan, request.Latitude, request.Longitude,
                    RecordedByUserId: adminId, PreferredGroupId: group.Value,
                    EnforceGeofence: true, RegisterImmediately: autoApprove,
                    RestrictToGroupId: group.Value, AutoApproveCompensation: autoApprove),
                cancellationToken);

            // كل مسح ناجح غير مكرر للأدمن يتسجل في الـ Audit (السجل نفسه فيه RecordedByUserId كمان)
            if (result.IsSuccess && result.Value!.Outcome != AttendanceOutcome.AlreadyRecorded)
            {
                _auditRepository.Add(AuditLog.Create(
                    tenant.TenantId, adminId, AuditAction.GroupAdminAttendanceRecorded, studentId,
                    $"{result.Value.Outcome} in {result.Value.GroupName}"));

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}