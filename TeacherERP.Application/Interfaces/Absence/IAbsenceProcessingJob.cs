// جديد — TeacherERP.Application/Interfaces/IAbsenceProcessingJob.cs
using Hangfire;

namespace TeacherERP.Application.Interfaces
{
    // مهمة دورية (Hangfire) لمعالجة غياب كل المدرسين النشطين. تشغيل واحد في الوقت (والـ Index الفريد خط دفاع ثانٍ)
    public interface IAbsenceProcessingJob
    {
        [DisableConcurrentExecution(timeoutInSeconds: 600)]
        Task RunAsync();
    }
}