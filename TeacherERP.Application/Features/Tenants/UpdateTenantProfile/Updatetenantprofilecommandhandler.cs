using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.UpdateTenantProfile
{
    public class UpdateTenantProfileCommandHandler : IRequestHandler<UpdateTenantProfileCommand, Result<bool>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTenantProfileCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateTenantProfileCommand request, CancellationToken cancellationToken)
        {
            // السنتر الحالي ثبّته الـ Policy (TenantOwner) بعد التحقق من العضوية والدور
            if (_currentTenant.Current is not { } current)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            var tenant = await _tenantRepository.GetByIdAsync(current.TenantId, cancellationToken);
            if (tenant is null)
            {
                return Result<bool>.Failure(TenantErrors.Forbidden);
            }

            tenant.UpdateProfile(request.Name, request.Subject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}