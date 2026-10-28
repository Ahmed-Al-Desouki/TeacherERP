// جديد — TeacherERP.Application/Absence/AbsencePolicy.cs
namespace TeacherERP.Application.Absence
{
    public static class AbsencePolicy
    {
        /// <summary>عدد الغيابات المتتالية الذي يوقف الطالب تلقائياً عند المدرس</summary>
        public const int AutoSuspendAfterConsecutiveAbsences = 4;

        /// <summary>لا نعالج الحصة إلا بعد نهايتها بهذه الدقائق (فارق الساعة وآخر مسح)</summary>
        public const int ProcessingGraceMinutes = 5;

        /// <summary>أقصى عدد حصص يعالجها الـ Job لكل مدرس في الدورة الواحدة (الباقي في الدورة التالية)</summary>
        public const int MaxSessionsPerRun = 200;
    }
}