using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.GetCurrentTenant
{
    public class GetCurrentTenantQueryHandler : IRequestHandler<GetCurrentTenantQuery, Result<TenantDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;

        public GetCurrentTenantQueryHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
        }

        public async Task<Result<TenantDto>> Handle(GetCurrentTenantQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<TenantDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<TenantDto>.Failure(TenantErrors.Required);
            }

            var membership = await _tenantRepository.GetMembershipAsync(userId, tenant.TenantId, cancellationToken);
            if (membership is null)
            {
                return Result<TenantDto>.Failure(TenantErrors.Forbidden);
            }

            return Result<TenantDto>.Success(TenantDto.From(membership));
        }
    }
}