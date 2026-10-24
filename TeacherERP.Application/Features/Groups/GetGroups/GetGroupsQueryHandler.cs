// موجود (تعديل) — TeacherERP.Application/Features/Groups/GetGroups/GetGroupsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.GetGroups
{
    public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, Result<List<GroupDto>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupRepository _groupRepository;

        public GetGroupsQueryHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupRepository groupRepository)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _groupRepository = groupRepository;
        }

        public async Task<Result<List<GroupDto>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<GroupDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<List<GroupDto>>.Failure(TenantErrors.Required);
            }

            var groups = await _groupRepository.GetAllWithSlotsAsync(cancellationToken);

            return Result<List<GroupDto>>.Success(groups.Select(GroupDto.From).ToList());
        }
    }
}