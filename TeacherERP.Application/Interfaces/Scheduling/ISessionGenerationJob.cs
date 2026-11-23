namespace TeacherERP.Application.Interfaces
{
    // مهمة دورية (Hangfire) لتوليد حصص كل المدرسين النشطين
    public interface ISessionGenerationJob
    {
        Task RunAsync();
    }
}