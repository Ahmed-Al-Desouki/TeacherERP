using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// الإعدادات العامة للمركز التعليمي (السنتر)، يتحكم بها المدرس لتعريف سياسات الحضور
    /// </summary>
    public class TenantSettings : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        /// <summary>وضع الحضور الافتراضي (كود ثابت للكل، متغير للشاشات، أو من طرف المساعد فقط)</summary>
        public AttendanceMode Mode { get; private set; }

        // --- إعدادات الحماية الجغرافية (Geofencing) ---
        // اختيارية: السنتر الجديد لم يضبط موقعه بعد
        public double? CenterLatitude { get; private set; }
        public double? CenterLongitude { get; private set; }

        /// <summary>النطاق المسموح به لتسجيل الحضور بالأمتار (مثلاً: 50 متر من المركز)</summary>
        public int GeofenceRadiusMeters { get; private set; }

        /// <summary>رفض محاولات تسجيل الحضور من برامج تزييف الموقع (Mock Locations)</summary>
        public bool RejectMockLocations { get; private set; }

        private TenantSettings() { }

        /// <summary>
        /// إعدادات مبدئية آمنة لسنتر جديد: الحضور من طرف المساعد فقط، حتى يضبط المدرس موقع السنتر.
        /// </summary>
        public static TenantSettings Create(Guid tenantId)
        {
            return new TenantSettings
            {
                TenantId = tenantId,
                Mode = AttendanceMode.AdminOnly,
                GeofenceRadiusMeters = 50,
                RejectMockLocations = true,
            };
        }

        public void UpdateLocation(double lat, double lon)
        {
            CenterLatitude = lat;
            CenterLongitude = lon;
            MarkAsUpdated();
        }
    }

    public enum AttendanceMode { StaticGlobalQR, DynamicScreenQR, AdminOnly }
}