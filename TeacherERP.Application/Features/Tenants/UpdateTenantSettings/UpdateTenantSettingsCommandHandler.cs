// جديد — TeacherERP.Application/Features/Tenants/UpdateTenantSettings/UpdateTenantSettingsCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.UpdateTenantSettings
{
    public class UpdateTenantSettingsCommandHandler : IRequestHandler<UpdateTenantSettingsCommand, Result<TenantSettingsDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTenantSettingsCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<TenantSettingsDto>> Handle(UpdateTenantSettingsCommand request, CancellationToken cancellationToken)
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

            tenant.UpdateSettings(request.IsListedInSearch, request.AutoApproveJoinRequests);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<TenantSettingsDto>.Success(new TenantSettingsDto(tenant.IsListedInSearch, tenant.AutoApproveJoinRequests));
        }
    }
}