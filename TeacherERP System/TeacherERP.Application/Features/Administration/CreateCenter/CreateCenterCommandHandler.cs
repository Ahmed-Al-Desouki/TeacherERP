using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration.CreateCenter
{
    public class CreateCenterCommandHandler : IRequestHandler<CreateCenterCommand, Result<AdminCenterDto>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICenterProvisioningService _centerProvisioning;

        public CreateCenterCommandHandler(
            ITenantRepository tenantRepository,
            ICenterProvisioningService centerProvisioning)
        {
            _tenantRepository = tenantRepository;
            _centerProvisioning = centerProvisioning;
        }

        public async Task<Result<AdminCenterDto>> Handle(CreateCenterCommand request, CancellationToken cancellationToken)
        {
            var ownerTenant = await _tenantRepository.GetByOwnerIdAsync(request.OwnerUserId, cancellationToken);
            if (ownerTenant is null)
            {
                return Result<AdminCenterDto>.Failure(AdministrationErrors.TeacherNotFound);
            }

            var created = await _centerProvisioning.CreateAsync(
                request.OwnerUserId, request.Name.Trim(), request.Address, request.Latitude, request.Longitude,
                request.GeofenceRadiusMeters, cancellationToken);
            if (created.IsFailure)
            {
                return Result<AdminCenterDto>.Failure(created.Error);
            }

            var center = created.Value!;
            return Result<AdminCenterDto>.Success(new AdminCenterDto(center.Id, center.Name, center.OwnerUserId));
        }
    }
}
