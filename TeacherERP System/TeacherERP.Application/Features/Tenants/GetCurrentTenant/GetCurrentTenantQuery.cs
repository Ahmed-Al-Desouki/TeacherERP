using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.GetCurrentTenant
{
    // المدرس الذي حدده الطلب الحالي (صريحاً بالـ Header أو تلقائياً لو عضوية واحدة)
    public record GetCurrentTenantQuery : IRequest<Result<TenantDto>>;
}