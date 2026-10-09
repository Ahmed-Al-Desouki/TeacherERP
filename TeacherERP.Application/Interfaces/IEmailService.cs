namespace TeacherERP.Application.Interfaces
{
    // خدمة إرسال الإيميلات (تُنفَّذ في طبقة Infrastructure)
    // الأوقات تُمرَّر بـ UTC مع المنطقة الزمنية للمستخدم (إن وُجدت) ليُحوَّل الوقت عند صياغة الإيميل
    public interface IEmailService
    {
        // تستدعى في الـ Background (Hangfire/FireAndForget) حتى لا تبطئ الـ API
        Task SendOtpEmailAsync(string toEmail, string otpCode);
        Task SendNewLoginAlertAsync(string toEmail, string deviceInfo, DateTime loginTime, string? timeZoneId);
        Task SendPasswordResetOtpAsync(string toEmail, string otpCode);
        Task SendPasswordChangedAlertAsync(string toEmail, DateTime changedAt, string? timeZoneId);
        Task SendSessionEvictedAlertAsync(string toEmail, string evictedDeviceName, string newDeviceName, DateTime occurredAt, string? timeZoneId);
    }
}