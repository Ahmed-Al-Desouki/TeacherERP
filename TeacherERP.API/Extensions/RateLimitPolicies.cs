// موجود (تعديل) — TeacherERP.API/Extensions/RateLimitPolicies.cs
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

        /// <summary>رفع ملفات الحصص (بالمستخدم): كل طلب يرفع ملفاً لـ Cloudinary فنحميه من الإغراق</summary>
        public const string ContentUpload = "content-upload";

        /// <summary>إرسال طلب مدرس من صفحة الدخول (بالـ IP وبحد صارم بالساعة)</summary>
        public const string TeacherRequestSubmit = "teacher-request-submit";

        /// <summary>معاينة وقبول ورفض طلب مدرس (بالمستخدم)</summary>
        public const string TeacherRequestReview = "teacher-request-review";
    }
}
