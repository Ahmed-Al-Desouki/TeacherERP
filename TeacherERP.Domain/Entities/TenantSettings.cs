using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// إعدادات المدرس لسياسات الحضور. موقع المكان ونطاقه الجغرافي يخصان السنتر (Center) لا المدرس.
    /// </summary>
    public class TenantSettings : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        /// <summary>وضع الحضور الافتراضي (كود ثابت للكل، متغير للشاشات، أو من طرف المساعد فقط)</summary>
        public AttendanceMode Mode { get; private set; }

        /// <summary>رفض محاولات تسجيل الحضور من برامج تزييف الموقع (Mock Locations)</summary>
        public bool RejectMockLocations { get; private set; }

        private TenantSettings() { }

        /// <summary>
        /// إعدادات مبدئية آمنة لمدرس جديد: الحضور من طرف المساعد فقط، حتى يضبط المدرس نظام الحضور.
        /// </summary>
        public static TenantSettings Create(Guid tenantId)
        {
            return new TenantSettings
            {
                TenantId = tenantId,
                Mode = AttendanceMode.AdminOnly,
                RejectMockLocations = true,
            };
        }
    }

    public enum AttendanceMode { StaticGlobalQR, DynamicScreenQR, AdminOnly }
}