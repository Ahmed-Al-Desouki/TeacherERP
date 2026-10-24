// موجود (تعديل) — TeacherERP.Application/Interfaces/IGroupSessionRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public record SessionListItem(
        Guid Id,
        Guid GroupId,
        string GroupName,
        DateTime StartTime,
        DateTime EndTime,
        bool IsCancelled,
        Guid? SlotId,
        string? CancelReason,
        bool IsModified);

    // حصص المجموعات (ليس جلسات الدخول: تلك ISessionRepository)
    public interface IGroupSessionRepository
    {
        void AddRange(IEnumerable<Session> sessions);
        void RemoveRange(IEnumerable<Session> sessions);

        // حصص متتبَّعة مولّدة من حصص قالب معيّنة (لتعديل القالب)
        Task<List<Session>> GetTrackedBySlotIdsAsync(IReadOnlyCollection<Guid> slotIds, CancellationToken cancellationToken);

        // أي من هذه الحصص عليها سجلات حضور (لا تُحذف أبداً)
        Task<HashSet<Guid>> GetSessionIdsWithAttendanceAsync(IReadOnlyCollection<Guid> sessionIds, CancellationToken cancellationToken);

        // أوقات بداية حصص المجموعة الموجودة في النطاق (لمنع تكرار التوليد)
        Task<HashSet<DateTime>> GetStartTimesAsync(Guid groupId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);

        // حصة متتبَّعة للتعديل. الحفظ يتم عبر IUnitOfWork
        Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken);

        // حصص متتبَّعة في نطاق للتعديل الجماعي (groupIds فارغة = كل المجموعات)
        Task<List<Session>> GetTrackedByRangeAsync(
            DateTime fromUtc, DateTime toUtc, IReadOnlyCollection<Guid>? groupIds, CancellationToken cancellationToken);

        // هل يتعارض الوقت مع حصة أخرى لنفس المجموعة؟ (نفس وقت البداية حتى لو ملغاة بسبب الـ Index الفريد، أو تداخل مع غير ملغاة)
        Task<bool> HasConflictAsync(
            Guid groupId, DateTime startUtc, DateTime endUtc, IReadOnlyCollection<Guid> excludeSessionIds, CancellationToken cancellationToken);

        Task<List<SessionListItem>> GetByRangeAsync(
            DateTime fromUtc, DateTime toUtc, Guid? groupId, CancellationToken cancellationToken);
    }
}