// جديد — TeacherERP.Application/Features/Tenants/UpdateTenantSettings/UpdateTenantSettingsCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.UpdateTenantSettings
{
    public record TenantSettingsDto(bool IsListedInSearch, bool AutoApproveJoinRequests);

    // ظهور المدرس في بحث الطلاب وقبول طلبات الانضمام تلقائياً (للمدرس فقط عبر الـ Policy)
    public record UpdateTenantSettingsCommand(bool IsListedInSearch, bool AutoApproveJoinRequests)
        : IRequest<Result<TenantSettingsDto>>;
}