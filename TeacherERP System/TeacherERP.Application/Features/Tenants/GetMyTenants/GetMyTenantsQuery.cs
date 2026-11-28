using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.GetMyTenants
{
    // كل مساحات المدرسين التي المستخدم عضو فيها (لا تحتاج مدرس محدد)
    public record GetMyTenantsQuery : IRequest<Result<List<TenantDto>>>;
}