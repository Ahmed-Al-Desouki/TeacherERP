// موجود (تعديل) — TeacherERP.Application/Interfaces/INotificationRepository.cs
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public record NotificationItem(Guid Id, Guid TenantId, NotificationType Type, string Message, Guid? RelatedId, bool IsRead, DateTime CreatedAt);
    public record NotificationShareItem(
        SessionAttachmentKind Kind, string GroupName, string FileName, string? Caption,
        string AttachmentUrl, string? WhatsAppGroupLink);

    public interface INotificationRepository
    {
        void Add(Notification notification);

        // مدرس المساحة وكل مساعديه النشطين
        Task<List<Guid>> GetStaffRecipientIdsAsync(Guid tenantId, CancellationToken cancellationToken);

        // الطلاب ذوو التسجيل والعضوية النشطين في المجموعة، بما فيهم رافع الملف
        Task<List<Guid>> GetGroupStudentRecipientIdsAsync(
            Guid tenantId, Guid groupId, CancellationToken cancellationToken);

        // مالك مساحة المدرس فقط، لإشعارات القرارات الموجهة إليه حصراً
        Task<Guid> GetTeacherRecipientIdAsync(Guid tenantId, CancellationToken cancellationToken);

        // إشعاراتي بصفحات، الأحدث أولاً
        Task<PagedResult<NotificationItem>> GetMinePageAsync(
            Guid userId, bool unreadOnly, PageRequest page, CancellationToken cancellationToken);

        Task<Notification?> GetOwnTrackedAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken);
        Task<List<Notification>> GetOwnUnreadTrackedAsync(Guid userId, CancellationToken cancellationToken);

        // تفاصيل المشاركة لجروب المستخدم؛ لا تُرجع إلا لإشعار ملف يملكه المستخدم نفسه
        Task<NotificationShareItem?> GetOwnSessionAttachmentShareAsync(
            Guid notificationId, Guid userId, CancellationToken cancellationToken);
    }
}
