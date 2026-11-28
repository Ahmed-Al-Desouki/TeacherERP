// موجود (تعديل) — TeacherERP.Domain/Entities/GroupScheduleSlot.cs
using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// حصة واحدة في القالب الأسبوعي الثابت للمجموعة (مثال: السبت 16:00 لمدة 120 دقيقة).
    /// الوقت وقت حائط بتوقيت المدرس (Tenant.TimeZoneId)، ويُحوَّل لـ UTC عند توليد الحصص الفعلية.
    /// </summary>
    public class GroupScheduleSlot : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid GroupId { get; private set; }
        public Group Group { get; private set; }

        public DayOfWeek Day { get; private set; }

        /// <summary>وقت البداية (داخل اليوم الواحد)</summary>
        public TimeSpan StartTime { get; private set; }

        /// <summary>مدة الحصة بالدقائق (تدعم الساعة ونصف وغيرها)</summary>
        public int DurationMinutes { get; private set; }

        private GroupScheduleSlot() { }

        public void SetDuration(int durationMinutes)
        {
            DurationMinutes = durationMinutes;
            MarkAsUpdated();
        }

        public static GroupScheduleSlot Create(Guid tenantId, Guid groupId, DayOfWeek day, TimeSpan startTime, int durationMinutes)
        {
            return new GroupScheduleSlot
            {
                TenantId = tenantId,
                GroupId = groupId,
                Day = day,
                StartTime = startTime,
                DurationMinutes = durationMinutes
            };
        }
    }
}