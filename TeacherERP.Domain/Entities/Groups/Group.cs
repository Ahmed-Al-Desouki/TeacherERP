// موجود (تعديل) — TeacherERP.Domain/Entities/Group.cs
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TeacherERP.Domain.ValueObjects;

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
        public EducationStage? EducationStage { get; private set; }
        public int? GradeNumber { get; private set; }
        public SchoolGrade? SchoolGrade => EducationStage is { } stage && GradeNumber is { } number
            ? new SchoolGrade(stage, number)
            : null;

        /// <summary>السنتر الذي تنعقد فيه المجموعة (منه الموقع الجغرافي للحضور)</summary>
        public Guid CenterId { get; private set; }

        /// <summary>القاعة (اختيارية)</summary>
        public string? Room { get; private set; }

        /// <summary>السعة القصوى للمجموعة (اختيارية)</summary>
        public int? Capacity { get; private set; }

        /// <summary>سعر الحصة الواحدة، فارغ إذا لم يُحدَّد بعد</summary>
        public decimal? PerSessionPrice { get; private set; }

        /// <summary>سعر الاشتراك الشهري، فارغ إذا لم يُحدَّد بعد</summary>
        public decimal? MonthlyPrice { get; private set; }

        /// <summary>رابط الانضمام لمجموعة الواتساب ليستخدم في الـ Deep Linking (اختياري)</summary>
        public string? WhatsAppGroupLink { get; private set; }

        /// <summary>كود الانضمام الثابت للمجموعة، يُعرض للمدرس فقط</summary>
        public string? StaticJoinCode { get; private set; }

        /// <summary>سر اشتقاق كود الانضمام المتغير، لا يُعرض عبر الـ API</summary>
        public string? DynamicJoinSecret { get; private set; }

        public bool StaticJoinEnabled { get; private set; } = true;
        public bool DynamicJoinEnabled { get; private set; } = true;

        /// <summary>المجموعة المؤرشفة لا تولّد حصصاً جديدة</summary>
        public bool IsActive { get; private set; }

        /// <summary>قالب الجدول الأسبوعي الثابت (يوم وبداية ومدة لكل حصة)</summary>
        public ICollection<GroupScheduleSlot> Slots { get; private set; }

        public ICollection<Session> Sessions { get; private set; }

        private Group() { }

        public static Group Create(
            Guid tenantId,
            string name,
            SchoolGrade grade,
            Guid centerId,
            string? room,
            int? capacity,
            string? whatsappLink,
            decimal? perSessionPrice = null,
            decimal? monthlyPrice = null)
        {
            ValidatePricing(perSessionPrice, monthlyPrice);
            return new Group
            {
                TenantId = tenantId,
                Name = name.Trim(),
                GradeLevel = grade.ToDisplayName(),
                EducationStage = grade.Stage,
                GradeNumber = grade.Number,
                CenterId = centerId,
                Room = Clean(room),
                Capacity = capacity,
                PerSessionPrice = perSessionPrice,
                MonthlyPrice = monthlyPrice,
                WhatsAppGroupLink = Clean(whatsappLink),
                StaticJoinCode = NewStaticJoinCode(),
                DynamicJoinSecret = NewDynamicJoinSecret(),
                IsActive = true,
                Slots = new List<GroupScheduleSlot>(),
                Sessions = new List<Session>(),
            };
        }

        // Legacy factory retained for existing records and fixtures; API commands use the structured overload.
        public static Group Create(
            Guid tenantId,
            string name,
            string gradeLevel,
            Guid centerId,
            string? room,
            int? capacity,
            string? whatsappLink,
            decimal? perSessionPrice = null,
            decimal? monthlyPrice = null)
        {
            ValidatePricing(perSessionPrice, monthlyPrice);
            return new Group
            {
                TenantId = tenantId,
                Name = name.Trim(),
                GradeLevel = gradeLevel.Trim(),
                EducationStage = null,
                GradeNumber = null,
                CenterId = centerId,
                Room = Clean(room),
                Capacity = capacity,
                PerSessionPrice = perSessionPrice,
                MonthlyPrice = monthlyPrice,
                WhatsAppGroupLink = Clean(whatsappLink),
                StaticJoinCode = NewStaticJoinCode(),
                DynamicJoinSecret = NewDynamicJoinSecret(),
                IsActive = true,
                Slots = new List<GroupScheduleSlot>(),
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
            EducationStage = null;
            GradeNumber = null;
            CenterId = centerId;
            Room = Clean(room);
            Capacity = capacity;
            WhatsAppGroupLink = Clean(whatsappLink);
            MarkAsUpdated();
        }

        public void UpdateDetails(string name, SchoolGrade grade, Guid centerId, string? room, int? capacity, string? whatsappLink)
        {
            Name = name.Trim();
            GradeLevel = grade.ToDisplayName();
            EducationStage = grade.Stage;
            GradeNumber = grade.Number;
            CenterId = centerId;
            Room = Clean(room);
            Capacity = capacity;
            WhatsAppGroupLink = Clean(whatsappLink);
            MarkAsUpdated();
        }

        /// <summary>تغيير الأسعار الحالية دون المساس بالمعاملات المسجلة سابقاً</summary>
        public void UpdatePricing(decimal? perSessionPrice, decimal? monthlyPrice)
        {
            ValidatePricing(perSessionPrice, monthlyPrice);

            if (PerSessionPrice == perSessionPrice && MonthlyPrice == monthlyPrice)
            {
                return;
            }

            PerSessionPrice = perSessionPrice;
            MonthlyPrice = monthlyPrice;
            MarkAsUpdated();
        }

        /// <summary>أرشفة المجموعة (لا تولّد حصصاً جديدة) أو إعادتها</summary>
        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            MarkAsUpdated();
        }

        public bool EnsureJoinCodes()
        {
            var changed = false;
            if (string.IsNullOrWhiteSpace(StaticJoinCode))
            {
                StaticJoinCode = NewStaticJoinCode();
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(DynamicJoinSecret))
            {
                DynamicJoinSecret = NewDynamicJoinSecret();
                changed = true;
            }

            if (changed)
            {
                MarkAsUpdated();
            }

            return changed;
        }

        public void UpdateJoinCodeSettings(bool staticEnabled, bool dynamicEnabled)
        {
            if (DynamicJoinEnabled && !dynamicEnabled)
            {
                DynamicJoinSecret = NewDynamicJoinSecret();
            }

            StaticJoinEnabled = staticEnabled;
            DynamicJoinEnabled = dynamicEnabled;
            MarkAsUpdated();
        }

        public void RotateStaticJoinCode()
        {
            StaticJoinCode = NewStaticJoinCode();
            MarkAsUpdated();
        }

        public void RotateDynamicJoinCode()
        {
            DynamicJoinSecret = NewDynamicJoinSecret();
            MarkAsUpdated();
        }

        private static void ValidatePricing(decimal? perSessionPrice, decimal? monthlyPrice)
        {
            if (perSessionPrice is < 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(perSessionPrice), "The per-session price cannot be negative.");
            }

            if (monthlyPrice is < 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(monthlyPrice), "The monthly price cannot be negative.");
            }
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static string NewStaticJoinCode()
            => Convert.ToHexString(RandomNumberGenerator.GetBytes(10));

        private static string NewDynamicJoinSecret()
            => Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
    }
}
