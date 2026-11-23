// جديد — TeacherERP.Domain/Entities/TeacherAccessRequest.cs
using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// طلب مدرس للانضمام من صفحة الدخول. يُحفظ بعد تأكيد ملكية الإيميل بالـ OTP فقط، ولا يُنشأ بسببه أي حساب.
    /// الحساب يُنشأ عند الموافقة (مدرس مسح الـ QR أو مدير النظام). السر الخام في الـ QR لا يُخزَّن، نخزّن بصمته فقط.
    /// غير معزول بالـ Tenant عمداً (صاحب الطلب ليس عضواً في أي مساحة بعد).
    /// المراجِع والحساب المُنشأ بلا Foreign Keys عمداً: سجل تاريخي لا يتأثر بحذف حساب لاحقاً.
    /// </summary>
    public class TeacherAccessRequest : BaseEntity
    {
        public string FullName { get; private set; } = string.Empty;

        /// <summary>الإيميل بحروف صغيرة (مطبَّع)</summary>
        public string Email { get; private set; } = string.Empty;

        /// <summary>مادة المدرس، للعرض على من يراجع الطلب (وتُضبط على مساحته عند الموافقة)</summary>
        public string Subject { get; private set; } = string.Empty;

        public string? Note { get; private set; }

        /// <summary>المنطقة الزمنية (IANA) التي تُنشأ بها مساحة المدرس عند الموافقة</summary>
        public string TimeZoneId { get; private set; } = string.Empty;

        /// <summary>SHA-256 (Hex) للسر الموجود في الـ QR</summary>
        public string TokenHash { get; private set; } = string.Empty;

        public TeacherAccessRequestStatus Status { get; private set; }

        public DateTime ExpiresAtUtc { get; private set; }

        public Guid? ReviewedByUserId { get; private set; }

        public DateTime? ReviewedAtUtc { get; private set; }

        /// <summary>سبب الرفض (يظهر لصاحب الطلب)</summary>
        public string? DecisionNote { get; private set; }

        /// <summary>الحساب الذي أُنشئ عند الموافقة</summary>
        public Guid? CreatedUserId { get; private set; }

        private TeacherAccessRequest() { }

        public static TeacherAccessRequest Create(
            string fullName, string email, string subject, string? note, string timeZoneId, string tokenHash, DateTime expiresAtUtc)
        {
            return new TeacherAccessRequest
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                Subject = subject.Trim(),
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
                TimeZoneId = timeZoneId,
                TokenHash = tokenHash,
                Status = TeacherAccessRequestStatus.Pending,
                ExpiresAtUtc = expiresAtUtc
            };
        }

        /// <summary>الحالة الفعلية: المعلّق الذي انتهت مدته يُعامل كمنتهٍ حتى لو لم يُحدَّث في الداتابيز</summary>
        public TeacherAccessRequestStatus GetEffectiveStatus(DateTime nowUtc)
        {
            return Status == TeacherAccessRequestStatus.Pending && ExpiresAtUtc <= nowUtc
                ? TeacherAccessRequestStatus.Expired
                : Status;
        }

        public void MarkExpired()
        {
            Status = TeacherAccessRequestStatus.Expired;
            MarkAsUpdated();
        }

        public void Approve(Guid reviewerUserId, DateTime nowUtc, Guid createdUserId)
        {
            Status = TeacherAccessRequestStatus.Approved;
            ReviewedByUserId = reviewerUserId;
            ReviewedAtUtc = nowUtc;
            CreatedUserId = createdUserId;
            MarkAsUpdated();
        }

        public void Reject(Guid reviewerUserId, DateTime nowUtc, string? reason)
        {
            Status = TeacherAccessRequestStatus.Rejected;
            ReviewedByUserId = reviewerUserId;
            ReviewedAtUtc = nowUtc;
            DecisionNote = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            MarkAsUpdated();
        }
    }

    public enum TeacherAccessRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Expired
    }
}
