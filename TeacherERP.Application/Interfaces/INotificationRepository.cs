// جديد — TeacherERP.Application/Interfaces/INotificationRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public record NotificationItem(Guid Id, Guid TenantId, NotificationType Type, string Message, Guid? RelatedId, bool IsRead, DateTime CreatedAt);

    public interface INotificationRepository
    {
        void Add(Notification notification);

        // مدرس المساحة، ومساعدو المجموعة المخصصون لها (لو حُددت)
        Task<List<Guid>> GetStaffRecipientIdsAsync(Guid tenantId, Guid? groupId, CancellationToken cancellationToken);

        Task<List<NotificationItem>> GetMineAsync(Guid userId, bool unreadOnly, CancellationToken cancellationToken);
        Task<Notification?> GetOwnTrackedAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken);
        Task<List<Notification>> GetOwnUnreadTrackedAsync(Guid userId, CancellationToken cancellationToken);
    }
}