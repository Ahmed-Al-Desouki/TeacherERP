// جديد — TeacherERP.Application/Interfaces/INotificationService.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface INotificationService
    {
        // يضيف إشعاراً لموظفي المدرس (بدون حفظ: يُحفظ مع باقي تغييرات الطلب)
        Task NotifyStaffAsync(
            Guid tenantId, Guid? groupId, NotificationType type, string message, Guid? relatedId, CancellationToken cancellationToken);
    }
}