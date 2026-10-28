// جديد — TeacherERP.Application/Interfaces/IAuditRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // ActorName فارغ = النظام (أو حساب اتمسح). الحفظ يتم عبر IUnitOfWork
    public record AuditLogItem(
        Guid Id,
        AuditAction Action,
        Guid? ActorUserId,
        string? ActorName,
        Guid? TargetUserId,
        string? TargetName,
        string? Reason,
        DateTime CreatedAt);

    // سجل التدقيق للمدرس الحالي (الفلتر العام يعمل)
    public interface IAuditRepository
    {
        void Add(AuditLog log);

        Task<List<AuditLogItem>> GetRecentAsync(Guid? targetUserId, int take, CancellationToken cancellationToken);
    }
}