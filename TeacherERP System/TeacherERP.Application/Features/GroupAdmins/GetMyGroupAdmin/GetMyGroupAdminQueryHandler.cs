// جديد — TeacherERP.Application/Features/GroupAdmins/GetMyGroupAdmin/GetMyGroupAdminQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin
{
    public class GetMyGroupAdminQueryHandler : IRequestHandler<GetMyGroupAdminQuery, Result<GroupAdminMeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupAccessService _groupAccess;
        private readonly IGroupRepository _groupRepository;

        public GetMyGroupAdminQueryHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupAccessService groupAccess,
            IGroupRepository groupRepository)
        {
            _currentTenant = currentTenant;
            _groupAccess = groupAccess;
            _groupRepository = groupRepository;
        }

        public async Task<Result<GroupAdminMeDto>> Handle(GetMyGroupAdminQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<GroupAdminMeDto>.Failure(TenantErrors.Required);
            }

            var group = await _groupAccess.GetAdminGroupIdAsync(cancellationToken);
            if (group.IsFailure)
            {
                // غير مسؤول (أو موقوف أو موظف): رد عادي وليس خطأ
                return Result<GroupAdminMeDto>.Success(new GroupAdminMeDto(false, null, null));
            }

            var entity = await _groupRepository.GetWithSlotsAsync(group.Value, cancellationToken);

            return Result<GroupAdminMeDto>.Success(new GroupAdminMeDto(true, group.Value, entity?.Name));
        }
    }
}