using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.CreateCenter
{
    public record CreateMyCenterCommand(
        string Name, string? Address, double? Latitude, double? Longitude,
        int GeofenceRadiusMeters = 50) : IRequest<Result<CenterDto>>;
}
