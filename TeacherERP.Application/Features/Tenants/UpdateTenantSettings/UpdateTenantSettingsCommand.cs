// موجود (تعديل) — TeacherERP.Application/Features/Tenants/UpdateTenantSettings/UpdateTenantSettingsCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants.UpdateTenantSettings
{
    public record TenantSettingsDto(bool IsListedInSearch, bool AutoApproveJoinRequests, bool AutoApproveGroupAdminChanges);

    // ظهور المدرس في بحث الطلاب، وقبول طلبات الانضمام تلقائياً، وقبول تغييرات الطلاب المسؤولين من المساعد تلقائياً
    // (للمدرس فقط عبر الـ Policy). استبدال كامل: الحقل الناقص يرجع false
    public record UpdateTenantSettingsCommand(
        bool IsListedInSearch,
        bool AutoApproveJoinRequests,
        bool AutoApproveGroupAdminChanges = false)
        : IRequest<Result<TenantSettingsDto>>;
}