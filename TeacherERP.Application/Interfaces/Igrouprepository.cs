using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // كل الاستعلامات تتفلتر تلقائياً بالمدرس الحالي (الفلتر العام)
    public interface IGroupRepository
    {
        void Add(Group group);

        // للعرض فقط (بدون تتبع) مع حصص القالب. seesAll = المدرس، وغيره (مساعد) يرى المخصصة له فقط
        Task<List<Group>> GetVisibleAsync(Guid userId, bool seesAll, CancellationToken cancellationToken);
        Task<Group?> GetVisibleByIdAsync(Guid groupId, Guid userId, bool seesAll, CancellationToken cancellationToken);

        // لتوليد الحصص
        Task<List<Group>> GetActiveWithSlotsAsync(CancellationToken cancellationToken);
        Task<Group?> GetWithSlotsAsync(Guid groupId, CancellationToken cancellationToken);

        Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken);
    }
}