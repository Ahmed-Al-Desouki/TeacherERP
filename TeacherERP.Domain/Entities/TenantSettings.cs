// موجود (تعديل) — TeacherERP.Domain/Entities/TenantSettings.cs
using System;
using System.Security.Cryptography;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// إعدادات المدرس لسياسات الحضور. موقع المكان ونطاقه الجغرافي يخصان السنتر (Center) لا المدرس.
    /// </summary>
    public class TenantSettings : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        /// <summary>وضع الحضور (كود ثابت للكل، متغير للشاشات، أو من طرف المساعد فقط)</summary>
        public AttendanceMode Mode { get; private set; }

        /// <summary>رفض محاولات تسجيل الحضور من برامج تزييف الموقع (Mock Locations)</summary>
        public bool RejectMockLocations { get; private set; }

        /// <summary>سر المدرس: هو الكود الثابت، ومنه يُشتق الكود المتغير. لا يُعرض إلا للمدرس</summary>
        public string QrSecret { get; private set; }

        /// <summary>طالب غير مسجل يمسح الكود فيتسجل في الحصة الجارية تلقائياً ويُبلَّغ المدرس (يقدر يقفله)</summary>
        public bool AutoRegisterOnScan { get; private set; }

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
                QrSecret = NewSecret(),
                AutoRegisterOnScan = true
            };
        }

        public void UpdateAttendance(AttendanceMode mode, bool rejectMockLocations, bool autoRegisterOnScan)
        {
            Mode = mode;
            RejectMockLocations = rejectMockLocations;
            AutoRegisterOnScan = autoRegisterOnScan;
            MarkAsUpdated();
        }

        /// <summary>تغيير السر يبطل الكود الثابت المطبوع والمتغير الحالي فوراً (لو تسرّبت صورة الكود)</summary>
        public void RegenerateQrSecret()
        {
            QrSecret = NewSecret();
            MarkAsUpdated();
        }

        private static string NewSecret() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
    }

    public enum AttendanceMode { StaticGlobalQR, DynamicScreenQR, AdminOnly }
}