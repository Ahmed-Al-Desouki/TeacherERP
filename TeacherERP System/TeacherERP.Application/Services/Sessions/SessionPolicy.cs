namespace TeacherERP.Application.Sessions
{
    // قواعد عمر الجلسة (مدة الـ Access Token تُضبط من JwtSettings:ExpiryMinutes)
    public static class SessionPolicy
    {
        /// <summary>الـ Refresh يتجدد 30 يوم مع كل تجديد (Sliding)</summary>
        public static readonly TimeSpan RefreshLifetime = TimeSpan.FromDays(30);

        /// <summary>الحد الأقصى المطلق لعمر الجلسة مهما تجدد</summary>
        public static readonly TimeSpan AbsoluteLifetime = TimeSpan.FromDays(90);

        /// <summary>
        /// فترة سماح بعد التجديد: استخدام التوكن القديم خلالها (طلبات تجديد متزامنة من التطبيق)
        /// يُرفض فقط ولا يُلغي الجلسة. بعدها يُعتبر إعادة استخدام (سرقة محتملة).
        /// </summary>
        public static readonly TimeSpan ReuseGracePeriod = TimeSpan.FromSeconds(10);

        /// <summary>الحد الأقصى للأجهزة النشطة لكل مستخدم</summary>
        public const int MaxActiveSessions = 3;

        /// <summary>مدة الاحتفاظ بالجلسات المنتهية أو الملغاة قبل حذفها (للمراجعة الأمنية)</summary>
        public static readonly TimeSpan EndedSessionRetention = TimeSpan.FromDays(30);
    }
}