// موجود (تعديل) — TeacherERP.Application/Interfaces/ISessionGenerator.cs
namespace TeacherERP.Application.Interfaces
{
    // توليد الحصص الفعلية من قالب الجدول للمدرس الحالي (السنتر الحالي لازم يكون محدداً في الـ Accessor)
    public interface ISessionGenerator
    {
        // يرجع عدد الحصص الجديدة. آمن للتكرار: لا يكرر حصة موجودة ولا يولّد في الماضي
        // fromLocalDate: لا نولّد قبل هذا التاريخ (بتوقيت المدرس)، وهو الأقصى بين اليوم وهذا التاريخ
        Task<int> GenerateForGroupAsync(Guid groupId, CancellationToken cancellationToken, DateOnly? fromLocalDate = null);
        Task<int> GenerateForAllGroupsAsync(CancellationToken cancellationToken);
    }
}