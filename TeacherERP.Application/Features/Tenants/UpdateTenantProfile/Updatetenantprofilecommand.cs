using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.UpdateTenantProfile
{
    // اسم المدرس ومادته (استبدال كامل)، للمدرس (Owner) فقط عبر الـ Policy
    public record UpdateTenantProfileCommand(string Name, string Subject) : IRequest<Result<bool>>;
}