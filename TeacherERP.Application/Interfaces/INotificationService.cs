// موجود (تعديل) — TeacherERP.Application/Interfaces/INotificationService.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface INotificationService
    {
        // يضيف إشعاراً للمدرس ومساعديه (بدون حفظ: يُحفظ مع باقي تغييرات الطلب)
        Task NotifyStaffAsync(
            Guid tenantId, NotificationType type, string message, Guid? relatedId, CancellationToken cancellationToken);
    }
}