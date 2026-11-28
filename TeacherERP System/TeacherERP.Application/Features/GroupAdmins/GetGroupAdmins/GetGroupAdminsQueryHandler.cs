// جديد — TeacherERP.Application/Features/GroupAdmins/GetGroupAdmins/GetGroupAdminsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GetGroupAdmins
{
    public class GetGroupAdminsQueryHandler : IRequestHandler<GetGroupAdminsQuery, Result<List<GroupAdminDto>>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupAdminRepository _groupAdminRepository;

        public GetGroupAdminsQueryHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupRepository groupRepository,
            IGroupAdminRepository groupAdminRepository)
        {
            _currentTenant = currentTenant;
            _groupRepository = groupRepository;
            _groupAdminRepository = groupAdminRepository;
        }

        public async Task<Result<List<GroupAdminDto>>> Handle(GetGroupAdminsQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<List<GroupAdminDto>>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن المجموعة تخص المدرس الحالي فقط
            if (await _groupRepository.GetWithSlotsAsync(request.GroupId, cancellationToken) is null)
            {
                return Result<List<GroupAdminDto>>.Failure(GroupErrors.NotFound);
            }

            var items = await _groupAdminRepository.GetAdminsAsync(request.GroupId, cancellationToken);

            return Result<List<GroupAdminDto>>.Success(items.Select(GroupAdminDto.From).ToList());
        }
    }
}