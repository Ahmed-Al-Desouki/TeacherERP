// جديد — TeacherERP.Application/Interfaces/IAbsenceProcessor.cs
namespace TeacherERP.Application.Interfaces
{
    // معالجة غياب الحصص المنتهية للمدرس الحالي (السنتر لازم يكون محدداً في الـ Accessor). يرجّع عدد الغيابات المسجّلة
    public interface IAbsenceProcessor
    {
        Task<int> ProcessAsync(CancellationToken cancellationToken);
    }
}