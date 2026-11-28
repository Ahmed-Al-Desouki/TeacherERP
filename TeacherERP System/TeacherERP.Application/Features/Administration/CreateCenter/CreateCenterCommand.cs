using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration.CreateCenter
{
    // مدير النظام ينشئ سنتراً لمدرس موجود. الموقع مطلوب ويصبح المدرس مؤسسه النشط.
    public record CreateCenterCommand(
        string Name, Guid OwnerUserId, string? Address, double? Latitude, double? Longitude,
        int GeofenceRadiusMeters = 50) : IRequest<Result<AdminCenterDto>>;
}
