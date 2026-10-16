using System;
using System.Collections.Generic;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// المجموعة الدراسية عند مدرس: تربط الطلاب بمواعيدهم الثابتة (قالب أسبوعي) ومساعديهم.
    /// مواعيدها في GroupScheduleSlot، والحصص الفعلية (Session) تُولَّد منها.
    /// </summary>
    public class Group : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public string Name { get; private set; }
        public string GradeLevel { get; private set; }

        /// <summary>السنتر الذي تنعقد فيه المجموعة (منه الموقع الجغرافي للحضور)</summary>
        public Guid CenterId { get; private set; }

        /// <summary>القاعة (اختيارية)</summary>
        public string? Room { get; private set; }

        /// <summary>السعة القصوى للمجموعة (اختيارية)</summary>
        public int? Capacity { get; private set; }

        /// <summary>رابط الانضمام لمجموعة الواتساب ليستخدم في الـ Deep Linking (اختياري)</summary>
        public string? WhatsAppGroupLink { get; private set; }

        /// <summary>المجموعة المؤرشفة لا تولّد حصصاً جديدة</summary>
        public bool IsActive { get; private set; }

        /// <summary>قالب الجدول الأسبوعي الثابت (يوم وبداية ومدة لكل حصة)</summary>
        public ICollection<GroupScheduleSlot> Slots { get; private set; }

        /// <summary>المساعدون المسموح لهم بإدارة هذه المجموعة وتظهر لهم في التطبيق</summary>
        public ICollection<ApplicationUser> AssignedAdmins { get; private set; }
        public ICollection<Session> Sessions { get; private set; }

        private Group() { }

        public static Group Create(
            Guid tenantId,
            string name,
            string gradeLevel,
            Guid centerId,
            string? room,
            int? capacity,
            string? whatsappLink)
        {
            return new Group
            {
                TenantId = tenantId,
                Name = name.Trim(),
                GradeLevel = gradeLevel.Trim(),
                CenterId = centerId,
                Room = Clean(room),
                Capacity = capacity,
                WhatsAppGroupLink = Clean(whatsappLink),
                IsActive = true,
                Slots = new List<GroupScheduleSlot>(),
                AssignedAdmins = new List<ApplicationUser>(),
                Sessions = new List<Session>(),
            };
        }

        public void AddSlot(DayOfWeek day, TimeSpan startTime, int durationMinutes)
        {
            Slots.Add(GroupScheduleSlot.Create(TenantId, Id, day, startTime, durationMinutes));
            MarkAsUpdated();
        }

        public void AddAdmin(ApplicationUser admin)
        {
            AssignedAdmins.Add(admin);
            MarkAsUpdated();
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}