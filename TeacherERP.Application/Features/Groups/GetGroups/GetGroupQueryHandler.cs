// موجود (تعديل) — TeacherERP.Application/Features/Groups/GetGroup/GetGroupQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.GetGroup
{
    public class GetGroupQueryHandler : IRequestHandler<GetGroupQuery, Result<GroupDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupRepository _groupRepository;

        public GetGroupQueryHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupRepository groupRepository)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _groupRepository = groupRepository;
        }

        public async Task<Result<GroupDto>> Handle(GetGroupQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<GroupDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<GroupDto>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن المجموعة تخص المدرس الحالي فقط
            var group = await _groupRepository.GetWithSlotsAsync(request.GroupId, cancellationToken);

            return group is null
                ? Result<GroupDto>.Failure(GroupErrors.NotFound)
                : Result<GroupDto>.Success(GroupDto.From(group));
        }
    }
}