// جديد — TeacherERP.Application/Features/Audit/AuditLogDto.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Audit
{
    // الأوقات UTC. ActorName فارغ = النظام (الإيقاف التلقائي). Action تظهر كنص
    public record AuditLogDto(
        Guid Id,
        AuditAction Action,
        Guid? ActorUserId,
        string? ActorName,
        Guid? TargetUserId,
        string? TargetName,
        string? Reason,
        DateTime CreatedAt)
    {
        public static AuditLogDto From(AuditLogItem item)
            => new(item.Id, item.Action, item.ActorUserId, item.ActorName, item.TargetUserId, item.TargetName, item.Reason, item.CreatedAt);
    }
}