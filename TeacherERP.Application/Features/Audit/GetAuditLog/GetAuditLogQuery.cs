// جديد — TeacherERP.Application/Features/Audit/GetAuditLog/GetAuditLogQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Audit.GetAuditLog
{
    // آخر 100 إجراء في سجل التدقيق، مع تصفية اختيارية بالمستخدم المتأثر (للمدرس فقط عبر الـ Policy)
    public record GetAuditLogQuery(Guid? TargetUserId) : IRequest<Result<List<AuditLogDto>>>;
}