using TeacherERP.Application.Attendance;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Centers
{
    public static class CenterLocationPolicy
    {
        public const double DuplicateOwnerRadiusMeters = 50;

        public static bool HasNearbyOwnedCenter(
            IEnumerable<Center> ownedCenters, double latitude, double longitude, Guid? excludedCenterId = null)
            => ownedCenters.Any(center => center.Id != excludedCenterId
                && center.Latitude is { } existingLatitude
                && center.Longitude is { } existingLongitude
                && GeoDistance.Meters(latitude, longitude, existingLatitude, existingLongitude)
                    <= DuplicateOwnerRadiusMeters);
    }
}
