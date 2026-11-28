// جديد — TeacherERP.Application/Features/Tenants/GetTenantSettings/GetTenantSettingsQuery.cs
using MediatR;
using TeacherERP.Application.Features.Tenants.UpdateTenantSettings;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.GetTenantSettings
{
    // قيم سياسات المدرس الحالية (للمدرس فقط عبر الـ Policy)
    public record GetTenantSettingsQuery : IRequest<Result<TenantSettingsDto>>;
}