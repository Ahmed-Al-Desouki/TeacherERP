using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// يمثل المجموعة الدراسية، وهي الكيان الذي يربط الطلاب بمواعيدهم الثابتة ومساعديهم
    /// </summary>
    public class Group : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public string Name { get; private set; }
        public string GradeLevel { get; private set; }

        /// <summary>رابط الانضمام لمجموعة الواتساب ليستخدم في الـ Deep Linking</summary>
        public string WhatsAppGroupLink { get; private set; }

        // --- إعدادات الجدولة التلقائية ---
        /// <summary>الأيام الثابتة لانعقاد المجموعة، مفصولة بفاصلة (مثال: "Monday,Wednesday")</summary>
        public string RecurringScheduleDays { get; private set; }

        /// <summary>أوقات حائط بتوقيت السنتر (Tenant.TimeZoneId)، وتُحوَّل لـ UTC عند توليد الحصص</summary>
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }

        /// <summary>المساعدون المسموح لهم بإدارة هذه المجموعة وتظهر لهم في التطبيق</summary>
        public ICollection<ApplicationUser> AssignedAdmins { get; private set; }
        public ICollection<Session> Sessions { get; private set; }

        private Group() { }

        public static Group Create(Guid tenantId, string name, string gradeLevel, string whatsappLink, string days, TimeSpan start, TimeSpan end)
        {
            return new Group
            {
                TenantId = tenantId,
                Name = name,
                GradeLevel = gradeLevel,
                WhatsAppGroupLink = whatsappLink,
                RecurringScheduleDays = days,
                StartTime = start,
                EndTime = end,
                AssignedAdmins = new List<ApplicationUser>(),
                Sessions = new List<Session>(),
            };
        }

        public void AddAdmin(ApplicationUser admin)
        {
            AssignedAdmins.Add(admin);
            MarkAsUpdated();
        }
    }
}