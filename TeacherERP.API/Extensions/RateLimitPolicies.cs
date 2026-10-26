// جديد — TeacherERP.API/Extensions/RateLimitPolicies.cs
namespace TeacherERP.API.Extensions
{
    // أسماء الـ Policies (الأرقام في RateLimitingExtensions)
    public static class RateLimitPolicies
    {
        /// <summary>تسجيل الدخول (باسورد وGoogle)</summary>
        public const string AuthLogin = "auth-login";

        /// <summary>endpoints بتبعت كود على الإيميل</summary>
        public const string AuthOtpSend = "auth-otp-send";

        /// <summary>endpoints بتتحقق من كود أو توكن إعادة التعيين</summary>
        public const string AuthOtpVerify = "auth-otp-verify";

        /// <summary>تجديد الجلسة وتسجيل الخروج</summary>
        public const string AuthSession = "auth-session";

        /// <summary>مسح الحضور (بالمستخدم مش بالـ IP: طلاب السنتر على نفس الواي فاي)</summary>
        public const string AttendanceScan = "attendance-scan";
    }
}