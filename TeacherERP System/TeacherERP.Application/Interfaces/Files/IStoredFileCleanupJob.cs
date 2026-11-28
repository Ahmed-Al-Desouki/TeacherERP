// جديد — TeacherERP.Application/Interfaces/IStoredFileCleanupJob.cs
namespace TeacherERP.Application.Interfaces
{
    // مهمة خلفية (Hangfire) لحذف ملف من التخزين الخارجي، وتُعاد تلقائياً لو فشلت
    public interface IStoredFileCleanupJob
    {
        Task DeleteAsync(string publicId);
    }
}