// موجود (تعديل) — TeacherERP.Application/Features/Groups/GetGroups/GetGroupsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.GetGroups
{
    public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, Result<PagedResult<GroupDto>>>
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

        public async Task<Result<PagedResult<GroupDto>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { })
            {
                return Result<PagedResult<GroupDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<GroupDto>>.Failure(TenantErrors.Required);
            }

            var page = await _groupRepository.GetPageWithSlotsAsync(PageRequest.From(request), cancellationToken);

            return Result<PagedResult<GroupDto>>.Success(page.Map(GroupDto.From));
        }
    }
}
