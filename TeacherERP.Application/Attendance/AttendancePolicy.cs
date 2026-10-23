// جديد — TeacherERP.Application/Attendance/AttendancePolicy.cs
namespace TeacherERP.Application.Attendance
{
    public static class AttendancePolicy
    {
        /// <summary>يُسمح بتسجيل الحضور قبل بداية الحصة بهذه الدقائق، وحتى نهايتها</summary>
        public const int EarlyCheckInMinutes = 15;

        /// <summary>عمر الكود المتغير بالثواني (يُقبل الحالي والسابق)</summary>
        public const int DynamicCodeWindowSeconds = 30;

        /// <summary>نافذة التعويض حول الحصة الأصلية بالأيام (قبلها أو بعدها)</summary>
        public const int CompensationWindowDays = 7;
    }
}