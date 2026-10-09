using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// يمثل الكيان الأساسي للمستخدم في النظام، ويرث من IdentityUser لدمج ميزات الحماية والمصادقة.
    /// يستخدم لتمثيل كل من: المدرس (SuperAdmin)، المساعدين (Admins)، والطلاب (Students).
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>عدد الحقول الإلزامية لإكمال بروفايل الطالب (موبايله + اسم وموبايل وصلة ولي الأمر)</summary>
        private const int RequiredProfileFieldsCount = 4;

        /// <summary>الاسم الرباعي أو الكامل للمستخدم</summary>
        public string FullName { get; private set; }

        // خصائص خاصة بالطلاب (Student Specific)
        /// <summary>حالة الطالب الحالية (نشط، موقوف بسبب الغياب، أو في القائمة السوداء)</summary>
        public StudentStatus Status { get; private set; }

        /// <summary>عداد الغياب المتتالي، يستخدم لتطبيق قاعدة الإيقاف التلقائي بعد 4 حصص</summary>
        public int ConsecutiveAbsences { get; private set; }

        /// <summary>الرصيد المالي للطالب (الموجب يعني رصيد متبقي، والسالب يعني مديونية)</summary>
        public decimal CurrentBalance { get; private set; }

        /// <summary>كود الاستجابة السريعة الثابت الخاص بالطالب (يستخدم في حالة غياب الإنترنت أو الهواتف الذكية)</summary>
        public string StaticQrCode { get; private set; }

        /// <summary>الجنس (اختياري، لا يدخل في نسبة اكتمال البروفايل)</summary>
        public Gender? Gender { get; private set; }

        /// <summary>تاريخ الميلاد (اختياري، لا يدخل في نسبة اكتمال البروفايل)</summary>
        public DateOnly? DateOfBirth { get; private set; }

        /// <summary>بيانات ولي الأمر (إلزامية لاكتمال البروفايل)</summary>
        public GuardianInfo? Guardian { get; private set; }

        // خصائص خاصة بالمساعدين (Admin Specific)

        /// <summary>يحدد ما إذا كان المساعد يمتلك صلاحية استلام الأموال والاطلاع على المديونيات</summary>
        public bool HasFinancialDelegation { get; private set; }

        // --- العلاقات (Navigation Properties) ---
        public ICollection<AttendanceRecord> Attendances { get; private set; }
        public ICollection<Transaction> Transactions { get; private set; }
        public ICollection<LoginSession> LoginSessions { get; private set; }

        // --- اكتمال البروفايل (محسوب، غير مخزّن) ---

        /// <summary>الحقول الإلزامية الناقصة (بنفس أسماء حقول طلب إكمال البروفايل)</summary>
        public IReadOnlyList<string> MissingProfileFields
        {
            get
            {
                var missing = new List<string>();

                if (string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    missing.Add("phoneNumber");
                }

                if (Guardian is null)
                {
                    missing.Add("guardianName");
                    missing.Add("guardianPhoneNumber");
                    missing.Add("guardianRelation");
                }

                return missing;
            }
        }

        public bool IsProfileComplete => MissingProfileFields.Count == 0;

        public int ProfileCompletionPercentage
            => (RequiredProfileFieldsCount - MissingProfileFields.Count) * 100 / RequiredProfileFieldsCount;

        /// <summary>منشئ فارغ مطلوب من قِبل Entity Framework Core</summary>
        private ApplicationUser() { }

        /// <summary>
        /// مصنع (Factory Method) لإنشاء طالب جديد بطريقة آمنة وبحالة مبدئية صحيحة.
        /// رقم الموبايل وبيانات ولي الأمر تُستكمل لاحقاً من خلال إكمال البروفايل.
        /// </summary>
        public static ApplicationUser CreateStudent(string fullName, string email)
        {
            return new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                UserName = email,
                Status = StudentStatus.Active,
                CurrentBalance = 0,
                StaticQrCode = Guid.NewGuid().ToString() // يمكن استبدالها بخوارزمية توليد أكواد أقصر
            };
        }

        /// <summary>
        /// مصنع (Factory Method) لإنشاء مساعد (Admin) جديد وتحديد صلاحياته المالية
        /// </summary>
        public static ApplicationUser CreateAdmin(string fullName, string email, string phoneNumber, bool hasFinancialDelegation)
        {
            return new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                UserName = email,
                PhoneNumber = phoneNumber,
                HasFinancialDelegation = hasFinancialDelegation
            };
        }

        // --- دوال التحكم في الحالة (Behavior Methods) ---

        /// <summary>
        /// إكمال/تحديث بيانات البروفايل (استبدال كامل). الأرقام تصل هنا بالصيغة الدولية بعد التحقق منها.
        /// القاعدة: موبايل ولي الأمر لازم يختلف عن موبايل الطالب.
        /// </summary>
        public Result<bool> CompleteProfile(string phoneNumber, GuardianInfo guardian, Gender? gender, DateOnly? dateOfBirth)
        {
            if (guardian.PhoneNumber == phoneNumber)
            {
                return Result<bool>.Failure(new Error(
                    "Profile.GuardianPhoneSameAsStudent",
                    "The guardian phone number must be different from the student phone number."));
            }

            if (PhoneNumber != phoneNumber)
            {
                PhoneNumber = phoneNumber;
                PhoneNumberConfirmed = false;
            }

            Guardian = guardian;
            Gender = gender;
            DateOfBirth = dateOfBirth;

            return Result<bool>.Success(true);
        }

        /// <summary>إضافة أموال لرصيد الطالب</summary>
        public void AddBalance(decimal amount) => CurrentBalance += amount;

        /// <summary>خصم أموال من رصيد الطالب (يسجل كمديونية إذا تخطى الرصيد المتاح)</summary>
        public void DeductBalance(decimal amount) => CurrentBalance -= amount;

        /// <summary>زيادة عداد الغياب المتتالي للطالب بخطوة واحدة</summary>
        public void IncrementAbsence() => ConsecutiveAbsences++;

        /// <summary>تصفير عداد الغياب بمجرد حضور الطالب لحصة واحدة</summary>
        public void ResetAbsence() => ConsecutiveAbsences = 0;

        /// <summary>إيقاف حساب الطالب (يحدث غالباً بشكل آلي عند تخطي الحد الأقصى للغياب)</summary>
        public void Suspend() => Status = StudentStatus.Suspended;

        /// <summary>إدراج الطالب في القائمة السوداء (منع نهائي من دخول النظام)</summary>
        public void Blacklist() => Status = StudentStatus.Blacklisted;

        /// <summary>إعادة تفعيل حساب الطالب</summary>
        public void Activate() => Status = StudentStatus.Active;
    }

    public enum StudentStatus { Active, Suspended, Blacklisted }

}