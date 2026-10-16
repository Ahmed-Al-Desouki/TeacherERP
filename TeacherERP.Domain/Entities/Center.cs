using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// السنتر: المكان الفعلي الذي يضم مدرساً أو أكثر. ليس وحدة عزل للبيانات:
    /// طلاب كل مدرس وأمواله في مساحته (Tenant)، ولا يراها صاحب السنتر ولا المدرسون الآخرون.
    /// </summary>
    public class Center : BaseEntity
    {
        public const int DefaultGeofenceRadiusMeters = 50;

        public string Name { get; private set; }
        public string? Address { get; private set; }

        /// <summary>مؤسس السنتر وصاحب قرار قبول المدرسين فيه</summary>
        public Guid OwnerUserId { get; private set; }

        // --- الحماية الجغرافية (Geofencing): الموقع للمكان نفسه، حتى لو فيه أكتر من مدرس ---
        // اختيارية: السنتر الجديد لم يضبط موقعه بعد
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }

        /// <summary>النطاق المسموح به لتسجيل الحضور بالأمتار (مثلاً: 50 متر من السنتر)</summary>
        public int GeofenceRadiusMeters { get; private set; }

        private Center() { }

        public static Center Create(string name, Guid ownerUserId)
        {
            return new Center
            {
                Name = name.Trim(),
                OwnerUserId = ownerUserId,
                GeofenceRadiusMeters = DefaultGeofenceRadiusMeters
            };
        }

        /// <summary>استبدال كامل لبيانات السنتر (الإحداثيات اختيارية وتُرسل معاً)</summary>
        public void UpdateDetails(string name, string? address, double? latitude, double? longitude, int geofenceRadiusMeters)
        {
            Name = name.Trim();
            Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
            Latitude = latitude;
            Longitude = longitude;
            GeofenceRadiusMeters = geofenceRadiusMeters;
            MarkAsUpdated();
        }
    }
}