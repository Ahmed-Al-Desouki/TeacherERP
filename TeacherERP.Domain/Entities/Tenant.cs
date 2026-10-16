using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// مساحة المدرس: الوحدة المعزولة في النظام. كل مدرس (SuperAdmin) له Tenant واحد
    /// بطلابه ومجموعاته وأمواله، لا يراها أي مدرس آخر حتى لو شاركه نفس السنتر.
    /// </summary>
    public class Tenant : BaseEntity
    {
        /// <summary>اسم المدرس الظاهر للطلاب</summary>
        public string Name { get; private set; }

        /// <summary>المدرس (SuperAdmin) صاحب المساحة</summary>
        public Guid OwnerId { get; private set; }

        /// <summary>مادة المدرس (مدرس واحد = مادة واحدة)، وتظهر في البحث. فارغة لحين ضبطها</summary>
        public string? Subject { get; private set; }

        /// <summary>المنطقة الزمنية (IANA مثل Africa/Cairo): مواعيد المجموعات أوقات حائط بتوقيته</summary>
        public string TimeZoneId { get; private set; }

        public TenantStatus Status { get; private set; }

        /// <summary>هل يظهر المدرس في بحث الطلاب؟ (اختياري، الافتراضي لا)</summary>
        public bool IsListedInSearch { get; private set; }

        /// <summary>قبول طلبات انضمام الطلاب للمجموعات تلقائياً (الافتراضي: تحتاج موافقة)</summary>
        public bool AutoApproveJoinRequests { get; private set; }

        private Tenant() { }

        public static Tenant Create(string name, Guid ownerId, string timeZoneId)
        {
            return new Tenant
            {
                Name = name.Trim(),
                OwnerId = ownerId,
                TimeZoneId = timeZoneId,
                Status = TenantStatus.Active,
                IsListedInSearch = false,
                AutoApproveJoinRequests = false
            };
        }

        /// <summary>استبدال كامل لاسم المدرس ومادته</summary>
        public void UpdateProfile(string name, string subject)
        {
            Name = name.Trim();
            Subject = subject.Trim();
            MarkAsUpdated();
        }
    }

    public enum TenantStatus { Active, Suspended }
}