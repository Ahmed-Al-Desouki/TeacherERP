namespace TeacherERP.Application.Interfaces
{
    // توليد الحصص الفعلية من قالب الجدول للمدرس الحالي (السنتر الحالي لازم يكون محدداً في الـ Accessor)
    public interface ISessionGenerator
    {
        // يرجع عدد الحصص الجديدة. آمن للتكرار: لا يكرر حصة موجودة ولا يولّد في الماضي
        Task<int> GenerateForGroupAsync(Guid groupId, CancellationToken cancellationToken);
        Task<int> GenerateForAllGroupsAsync(CancellationToken cancellationToken);
    }
}