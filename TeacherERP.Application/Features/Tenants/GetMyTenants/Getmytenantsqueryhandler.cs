using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.GetMyTenants
{
    public class GetMyTenantsQueryHandler : IRequestHandler<GetMyTenantsQuery, Result<List<TenantDto>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITenantRepository _tenantRepository;

        public GetMyTenantsQueryHandler(ICurrentUserService currentUser, ITenantRepository tenantRepository)
        {
            _currentUser = currentUser;
            _tenantRepository = tenantRepository;
        }

        public async Task<Result<List<TenantDto>>> Handle(GetMyTenantsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<TenantDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var memberships = await _tenantRepository.GetMembershipsAsync(userId, cancellationToken);

            return Result<List<TenantDto>>.Success(memberships.Select(TenantDto.From).ToList());
        }
    }
}