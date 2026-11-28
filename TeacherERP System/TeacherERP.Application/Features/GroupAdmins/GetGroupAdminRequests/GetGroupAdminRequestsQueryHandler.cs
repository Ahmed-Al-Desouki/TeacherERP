// جديد — TeacherERP.Application/Features/GroupAdmins/GetGroupAdminRequests/GetGroupAdminRequestsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests
{
    public class GetGroupAdminRequestsQueryHandler
        : IRequestHandler<GetGroupAdminRequestsQuery, Result<PagedResult<GroupAdminRequestDto>>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupAdminRepository _groupAdminRepository;

        public GetGroupAdminRequestsQueryHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupAdminRepository groupAdminRepository)
        {
            _currentTenant = currentTenant;
            _groupAdminRepository = groupAdminRepository;
        }

        public async Task<Result<PagedResult<GroupAdminRequestDto>>> Handle(
            GetGroupAdminRequestsQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<GroupAdminRequestDto>>.Failure(TenantErrors.Required);
            }

            var page = await _groupAdminRepository.GetRequestsPageAsync(
                request.Status, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<GroupAdminRequestDto>>.Success(page.Map(GroupAdminRequestDto.From));
        }
    }
}