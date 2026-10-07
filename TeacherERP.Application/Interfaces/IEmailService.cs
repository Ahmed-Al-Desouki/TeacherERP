namespace TeacherERP.Application.Interfaces
{
    // خدمة إرسال الإيميلات (تُنفَّذ في طبقة Infrastructure)
    public interface IEmailService
    {
        // تستدعى في الـ Background (Hangfire/FireAndForget) حتى لا تبطئ الـ API
        Task SendOtpEmailAsync(string toEmail, string otpCode);
        Task SendNewLoginAlertAsync(string toEmail, string deviceInfo, DateTime loginTime);
        Task SendPasswordResetOtpAsync(string toEmail, string otpCode);
        Task SendPasswordChangedAlertAsync(string toEmail, DateTime changedAt);
    }
}