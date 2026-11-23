using System.Data;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Centers
{
    public class CenterProvisioningService : ICenterProvisioningService
    {
        public const double DuplicateLocationRadiusMeters = CenterLocationPolicy.DuplicateOwnerRadiusMeters;

        private readonly ITenantRepository _tenantRepository;
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CenterProvisioningService(ITenantRepository tenantRepository, ICenterRepository centerRepository, IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Center>> CreateAsync(
            Guid ownerUserId, string name, string? address, double? latitude, double? longitude,
            int geofenceRadiusMeters, CancellationToken cancellationToken)
        {
            if (latitude is null || longitude is null)
            {
                return Result<Center>.Failure(CenterErrors.LocationRequired);
            }

            if (!double.IsFinite(latitude.Value) || latitude.Value < -90 || latitude.Value > 90
                || !double.IsFinite(longitude.Value) || longitude.Value < -180 || longitude.Value > 180)
            {
                return Result<Center>.Failure(CenterErrors.LocationInvalid);
            }

            if (geofenceRadiusMeters is < 10 or > 500)
            {
                return Result<Center>.Failure(CenterErrors.GeofenceRadiusInvalid);
            }

            var tenant = await _tenantRepository.GetByOwnerIdAsync(ownerUserId, cancellationToken);
            if (tenant is not { Status: TenantStatus.Active })
            {
                return Result<Center>.Failure(CenterErrors.TeacherSpaceNotFound);
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var ownedCenters = await _centerRepository.GetOwnedByUserAsync(ownerUserId, cancellationToken);
                if (CenterLocationPolicy.HasNearbyOwnedCenter(ownedCenters, latitude.Value, longitude.Value))
                {
                    return Result<Center>.Failure(CenterErrors.LocationAlreadyOwned);
                }

                var center = Center.Create(name, ownerUserId, latitude.Value, longitude.Value, address, geofenceRadiusMeters);
                _centerRepository.Add(center);
                _centerRepository.AddCenterTeacher(CenterTeacher.CreateFounder(center.Id, tenant.Id));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<Center>.Success(center);
            }, IsolationLevel.Serializable, cancellationToken);
        }
    }
}
