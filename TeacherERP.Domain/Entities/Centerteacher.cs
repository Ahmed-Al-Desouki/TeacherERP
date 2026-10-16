using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// وجود مدرس (بمساحته Tenant) في سنتر. ليس كياناً معزولاً بالـ Tenant عمداً:
    /// صاحب السنتر يرى مدرسي سنتره عبر مساحات مختلفة، فلا يصح أن يتفلتر بالسنتر الحالي.
    /// </summary>
    public class CenterTeacher : BaseEntity
    {
        public Guid CenterId { get; private set; }
        public Center Center { get; private set; }

        public Guid TenantId { get; private set; }

        public CenterTeacherStatus Status { get; private set; }
        public CenterTeacherOrigin Origin { get; private set; }

        private CenterTeacher() { }

        /// <summary>المدرس مؤسس السنتر: نشط من اللحظة الأولى</summary>
        public static CenterTeacher CreateFounder(Guid centerId, Guid tenantId)
        {
            return new CenterTeacher
            {
                CenterId = centerId,
                TenantId = tenantId,
                Status = CenterTeacherStatus.Active,
                Origin = CenterTeacherOrigin.Founder
            };
        }

        public void Activate()
        {
            Status = CenterTeacherStatus.Active;
            MarkAsUpdated();
        }

        public void Remove()
        {
            Status = CenterTeacherStatus.Removed;
            MarkAsUpdated();
        }
    }

    /// <summary>حالة المدرس في السنتر (معلّق بانتظار الدعوة أو الموافقة، نشط، أو أُزيل)</summary>
    public enum CenterTeacherStatus { Pending, Active, Removed }

    /// <summary>كيف دخل المدرس السنتر (مؤسس، بدعوة من صاحب السنتر، أو بطلب منه وافق عليه صاحب السنتر)</summary>
    public enum CenterTeacherOrigin { Founder, Invitation, Request }
}