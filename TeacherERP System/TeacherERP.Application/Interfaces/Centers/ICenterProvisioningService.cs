using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    public interface ICenterProvisioningService
    {
        Task<Result<Center>> CreateAsync(
            Guid ownerUserId, string name, string? address, double? latitude, double? longitude,
            int geofenceRadiusMeters, CancellationToken cancellationToken);
    }
}
