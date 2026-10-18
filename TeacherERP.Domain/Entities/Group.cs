// موجود (تعديل) — TeacherERP.Domain/Entities/Group.cs
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

        // يرجع الحصة المضافة ليتتبعها الـ Context صراحةً (المفتاح معيّن مسبقاً فلا يكفي الاكتشاف من الـ Collection)
        public GroupScheduleSlot AddSlot(DayOfWeek day, TimeSpan startTime, int durationMinutes)
        {
            var slot = GroupScheduleSlot.Create(TenantId, Id, day, startTime, durationMinutes);
            Slots.Add(slot);
            MarkAsUpdated();
            return slot;
        }

        public void RemoveSlot(GroupScheduleSlot slot)
        {
            Slots.Remove(slot);
            MarkAsUpdated();
        }

        /// <summary>استبدال كامل لبيانات المجموعة (الجدول له تعديله المنفصل)</summary>
        public void UpdateDetails(string name, string gradeLevel, Guid centerId, string? room, int? capacity, string? whatsappLink)
        {
            Name = name.Trim();
            GradeLevel = gradeLevel.Trim();
            CenterId = centerId;
            Room = Clean(room);
            Capacity = capacity;
            WhatsAppGroupLink = Clean(whatsappLink);
            MarkAsUpdated();
        }

        /// <summary>أرشفة المجموعة (لا تولّد حصصاً جديدة) أو إعادتها</summary>
        public void SetActive(bool isActive)
        {
            IsActive = isActive;
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