// جديد — TeacherERP.Application/SessionContent/SessionContentPolicy.cs
namespace TeacherERP.Application.SessionContent
{
    public static class SessionContentPolicy
    {
        /// <summary>أقصى حجم للملف الواحد</summary>
        public const long MaxFileBytes = 10 * 1024 * 1024;

        /// <summary>أقصى حجم للطلب كله (الملف + حقول الفورم)</summary>
        public const long MaxRequestBytes = MaxFileBytes + 1024 * 1024;

        public const int MaxFilesPerSession = 30;

        /// <summary>الطالب المسؤول يرفع من قبل بداية الحصة بهذه الدقائق</summary>
        public const int AdminUploadBeforeStartMinutes = 15;

        /// <summary>... وحتى هذه الساعات بعد نهايتها</summary>
        public const int AdminUploadAfterEndHours = 48;

        public static bool IsInAdminWindow(DateTime startUtc, DateTime endUtc, DateTime nowUtc)
            => nowUtc >= startUtc.AddMinutes(-AdminUploadBeforeStartMinutes)
               && nowUtc <= endUtc.AddHours(AdminUploadAfterEndHours);
    }
}