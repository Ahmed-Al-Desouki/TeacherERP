namespace TeacherERP.Application.Sessions
{
    // قواعد عمر الجلسة (مدة الـ Access Token تُضبط من JwtSettings:ExpiryMinutes)
    public static class SessionPolicy
    {
        /// <summary>الـ Refresh يتجدد 30 يوم مع كل تجديد (Sliding)</summary>
        public static readonly TimeSpan RefreshLifetime = TimeSpan.FromDays(30);

        /// <summary>الحد الأقصى المطلق لعمر الجلسة مهما تجدد</summary>
        public static readonly TimeSpan AbsoluteLifetime = TimeSpan.FromDays(90);
    }
}