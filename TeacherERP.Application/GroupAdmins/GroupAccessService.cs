// جديد — TeacherERP.Application/GroupAdmins/GroupAccessService.cs
using TeacherERP.Application.Features.GroupAdmins;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.GroupAdmins
{
    public class GroupAccessService : IGroupAccessService
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GroupAccessService(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IEnrollmentRepository enrollmentRepository)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<Result<GroupAccessLevel>> EnsureCanManageGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<GroupAccessLevel>.Failure(TenantErrors.Required);
            }

            // المدرس والمساعد على كل المجموعات (الحالة تحققها الـ Policy)
            if (tenant.Role is TenantRole.Owner or TenantRole.Assistant)
            {
                return Result<GroupAccessLevel>.Success(GroupAccessLevel.Staff);
            }

            var adminGroup = await GetAdminGroupIdAsync(cancellationToken);

            return adminGroup.IsSuccess && adminGroup.Value == groupId
                ? Result<GroupAccessLevel>.Success(GroupAccessLevel.GroupAdmin)
                : Result<GroupAccessLevel>.Failure(GroupAdminErrors.NotAllowed);
        }

        public async Task<Result<Guid>> GetAdminGroupIdAsync(CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<Guid>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<Guid>.Failure(TenantErrors.Required);
            }

            // الطالب الموقوف (غياب) لا يمارس صلاحياته كمسؤول
            if (tenant.Role != TenantRole.Student || tenant.MemberStatus != MembershipStatus.Active)
            {
                return Result<Guid>.Failure(GroupAdminErrors.NotAllowed);
            }

            var enrollment = await _enrollmentRepository.GetActiveEnrollmentAsync(tenant.TenantId, userId, cancellationToken);

            return enrollment is { IsGroupAdmin: true }
                ? Result<Guid>.Success(enrollment.GroupId)
                : Result<Guid>.Failure(GroupAdminErrors.NotAllowed);
        }
    }
}