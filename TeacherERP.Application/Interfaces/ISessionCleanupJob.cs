namespace TeacherERP.Application.Interfaces
{
    // مهمة دورية (Hangfire) لحذف الجلسات المنتهية
    public interface ISessionCleanupJob
    {
        Task RunAsync();
    }
}