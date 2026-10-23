// جديد — TeacherERP.Domain/Entities/Notification.cs
using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// إشعار داخل التطبيق لمستخدم. ليس معزولاً بالـ Tenant عمداً: المستخدم يرى إشعاراته من كل مدرسيه بحسب المستلم فقط.
    /// </summary>
    public class Notification : BaseEntity
    {
        public Guid RecipientUserId { get; private set; }

        /// <summary>مساحة المدرس صاحبة الإشعار (ليعرف التطبيق أي مدرس يفتح)</summary>
        public Guid TenantId { get; private set; }

        public NotificationType Type { get; private set; }

        /// <summary>نص احتياطي بالإنجليزية، والتطبيق يترجم حسب النوع</summary>
        public string Message { get; private set; }

        /// <summary>الكيان المرتبط (طلب أو سجل حضور) لفتح الشاشة المناسبة</summary>
        public Guid? RelatedId { get; private set; }

        public bool IsRead { get; private set; }

        private Notification() { }

        public static Notification Create(Guid recipientUserId, Guid tenantId, NotificationType type, string message, Guid? relatedId)
        {
            return new Notification
            {
                RecipientUserId = recipientUserId,
                TenantId = tenantId,
                Type = type,
                Message = message,
                RelatedId = relatedId
            };
        }

        public void MarkRead()
        {
            if (IsRead)
            {
                return;
            }

            IsRead = true;
            MarkAsUpdated();
        }
    }

    public enum NotificationType { StudentAutoRegistered, EnrollmentRequested, CompensationRequested }
}