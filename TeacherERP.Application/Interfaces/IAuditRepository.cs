// موجود (تعديل) — TeacherERP.Application/Interfaces/IAuditRepository.cs
using TeacherERP.Application.Common;
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

        // الأحدث أولاً، بصفحات
        Task<PagedResult<AuditLogItem>> GetPageAsync(Guid? targetUserId, PageRequest page, CancellationToken cancellationToken);
    }
}
