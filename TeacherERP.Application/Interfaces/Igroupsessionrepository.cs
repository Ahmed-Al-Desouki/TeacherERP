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
        Guid? SlotId);

    // حصص المجموعات (ليس جلسات الدخول: تلك ISessionRepository)
    public interface IGroupSessionRepository
    {
        void AddRange(IEnumerable<Session> sessions);

        // أوقات بداية حصص المجموعة الموجودة في النطاق (لمنع تكرار التوليد)
        Task<HashSet<DateTime>> GetStartTimesAsync(Guid groupId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);

        Task<List<SessionListItem>> GetByRangeAsync(
            DateTime fromUtc, DateTime toUtc, Guid? groupId, Guid userId, bool seesAll, CancellationToken cancellationToken);
    }
}