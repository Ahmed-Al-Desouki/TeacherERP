using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.UpdateCenter
{
    // لصاحب السنتر فقط (يُتحقق في الـ Handler)
    public record UpdateCenterCommand(
        Guid CenterId,
        string Name,
        string? Address,
        double? Latitude,
        double? Longitude,
        int GeofenceRadiusMeters) : IRequest<Result<CenterDto>>;
}