using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Centers
{
    public record CenterDto(
        Guid Id,
        string Name,
        string? Address,
        double? Latitude,
        double? Longitude,
        int GeofenceRadiusMeters,
        bool IsOwner)
    {
        public static CenterDto From(Center center, Guid currentUserId)
            => new(
                center.Id,
                center.Name,
                center.Address,
                center.Latitude,
                center.Longitude,
                center.GeofenceRadiusMeters,
                center.OwnerUserId == currentUserId);
    }
}