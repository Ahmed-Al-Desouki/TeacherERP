// جديد — TeacherERP.Application/Features/Tenants/GetTenantSettings/GetTenantSettingsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants.UpdateTenantSettings;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.GetTenantSettings
{
    public class GetTenantSettingsQueryHandler : IRequestHandler<GetTenantSettingsQuery, Result<TenantSettingsDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;

        public GetTenantSettingsQueryHandler(ICurrentTenantAccessor currentTenant, ITenantRepository tenantRepository)
        {
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
        }

        public async Task<Result<TenantSettingsDto>> Handle(GetTenantSettingsQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } current)
            {
                return Result<TenantSettingsDto>.Failure(TenantErrors.Required);
            }

            var tenant = await _tenantRepository.GetByIdAsync(current.TenantId, cancellationToken);
            if (tenant is null)
            {
                return Result<TenantSettingsDto>.Failure(TenantErrors.Forbidden);
            }

            return Result<TenantSettingsDto>.Success(
                new TenantSettingsDto(tenant.IsListedInSearch, tenant.AutoApproveJoinRequests, tenant.AutoApproveGroupAdminChanges));
        }
    }
}