// جديد — TeacherERP.Application/GroupAdmins/GroupAdminService.cs
using System.Data;
using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Features.GroupAdmins;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.GroupAdmins
{
    public class GroupAdminService : IGroupAdminService
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupRepository _groupRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IGroupAdminRepository _groupAdminRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public GroupAdminService(
            ICurrentTenantAccessor currentTenant,
            IGroupRepository groupRepository,
            IEnrollmentRepository enrollmentRepository,
            IGroupAdminRepository groupAdminRepository,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _groupRepository = groupRepository;
            _enrollmentRepository = enrollmentRepository;
            _groupAdminRepository = groupAdminRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public Task<Result<bool>> ApplyAsync(
            GroupAdminRequestType type,
            Guid groupId,
            Guid studentId,
            Guid actorUserId,
            string? reason,
            GroupAdminRequest? fulfills,
            CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Task.FromResult(Result<bool>.Failure(TenantErrors.Required));
            }

            // Serializable: فحص الحد والتعيين معاً بدون أن يسبقنا طلب متزامن على آخر مقعد
            return _unitOfWork.ExecuteInTransactionAsync(
                () => ApplyCoreAsync(tenant.TenantId, type, groupId, studentId, actorUserId, reason, fulfills, cancellationToken),
                IsolationLevel.Serializable,
                cancellationToken);
        }

        private async Task<Result<bool>> ApplyCoreAsync(
            Guid tenantId,
            GroupAdminRequestType type,
            Guid groupId,
            Guid studentId,
            Guid actorUserId,
            string? reason,
            GroupAdminRequest? fulfills,
            CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetWithSlotsAsync(groupId, cancellationToken);
            if (group is null)
            {
                return Result<bool>.Failure(GroupErrors.NotFound);
            }

            if (!group.IsActive)
            {
                return Result<bool>.Failure(GroupErrors.Inactive);
            }

            var enrollment = await _enrollmentRepository.GetActiveEnrollmentAsync(tenantId, studentId, cancellationToken);
            if (enrollment is null || enrollment.GroupId != groupId)
            {
                return Result<bool>.Failure(GroupAdminErrors.NotEnrolled);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            if (type == GroupAdminRequestType.Appoint)
            {
                if (enrollment.IsGroupAdmin)
                {
                    return Result<bool>.Failure(GroupAdminErrors.AlreadyAdmin);
                }

                var member = await _enrollmentRepository.GetMemberAsync(tenantId, studentId, cancellationToken);
                if (member is not { Status: MembershipStatus.Active, Role: TenantRole.Student })
                {
                    return Result<bool>.Failure(GroupAdminErrors.StudentNotActive);
                }

                if (await _groupAdminRepository.CountAdminsAsync(groupId, cancellationToken) >= GroupAdminPolicy.MaxPerGroup)
                {
                    return Result<bool>.Failure(GroupAdminErrors.LimitReached);
                }

                enrollment.AppointAsGroupAdmin(now);
                _auditRepository.Add(AuditLog.Create(tenantId, actorUserId, AuditAction.GroupAdminAppointed, studentId, reason));
            }
            else
            {
                if (!enrollment.IsGroupAdmin)
                {
                    return Result<bool>.Failure(GroupAdminErrors.NotAdmin);
                }

                enrollment.RemoveGroupAdmin();
                _auditRepository.Add(AuditLog.Create(tenantId, actorUserId, AuditAction.GroupAdminRemoved, studentId, reason));
            }

            // اعتماد الطلب في نفس الـ Transaction (لا يبقى التغيير منفَّذاً والطلب معلّقاً)
            fulfills?.Approve(actorUserId, now);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}