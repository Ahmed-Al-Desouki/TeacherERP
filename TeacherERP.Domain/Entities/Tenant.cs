using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// السنتر (نشاط المدرس): الوحدة المعزولة في النظام. كل مدرس (SuperAdmin) له Tenant واحد
    /// بطلابه ومجموعاته وأمواله، لا يراها أي مدرس آخر.
    /// </summary>
    public class Tenant : BaseEntity
    {
        public string Name { get; private set; }

        /// <summary>المدرس (SuperAdmin) صاحب السنتر</summary>
        public Guid OwnerId { get; private set; }

        /// <summary>المنطقة الزمنية للسنتر (IANA مثل Africa/Cairo): مواعيد المجموعات أوقات حائط بتوقيته</summary>
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
    }

    public enum TenantStatus { Active, Suspended }
}