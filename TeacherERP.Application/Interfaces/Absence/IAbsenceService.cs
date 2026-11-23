// جديد — TeacherERP.Application/Interfaces/IAbsenceService.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // مصدر واحد لقاعدة الغياب: سجل Absent + زيادة العداد + الإيقاف التلقائي (يستخدمه الـ Job ورفض التعويض)
    public interface IAbsenceService
    {
        // بدون حفظ: المستدعي يحفظ. يرجّع true لو الطالب اتوقف بسبب وصول العداد للحد
        Task<bool> RegisterAbsenceAsync(TenantMember member, Guid sessionId, CancellationToken cancellationToken);
    }
}