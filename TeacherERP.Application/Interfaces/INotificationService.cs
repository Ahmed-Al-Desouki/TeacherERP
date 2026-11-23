// موجود (تعديل) — TeacherERP.Application/Interfaces/INotificationService.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface INotificationService
    {
        // إشعار لمستخدم محدد، يُحفظ مع بقية تغييرات العملية في وحدة العمل نفسها
        Task NotifyUserAsync(
            Guid tenantId, Guid recipientUserId, NotificationType type,
            string message, Guid? relatedId, CancellationToken cancellationToken);

        // يضيف إشعاراً للمدرس ومساعديه (بدون حفظ: يُحفظ مع باقي تغييرات الطلب)
        Task NotifyStaffAsync(
            Guid tenantId, NotificationType type, string message, Guid? relatedId, CancellationToken cancellationToken);

        // يضيف إشعاراً لمالك مساحة المدرس فقط، بدون حفظ مستقل
        Task NotifyTeacherAsync(
            Guid tenantId, NotificationType type, string message, Guid? relatedId, CancellationToken cancellationToken);

        // إشعار الطلاب النشطين في المجموعة عند رفع ملف؛ يُحفظ مع المرفق في وحدة العمل نفسها
        Task NotifyGroupStudentsAsync(
            Guid tenantId, Guid groupId, NotificationType type,
            string message, Guid relatedId, CancellationToken cancellationToken);
    }
}
