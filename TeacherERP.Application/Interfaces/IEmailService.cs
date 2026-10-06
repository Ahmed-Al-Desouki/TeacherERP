using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Application.Interfaces
{

    // 3. خدمة إرسال الإيميلات (سيتم تنفيذها في طبقة Infrastructure)
    public interface IEmailService
    {
        // تستدعى في الـ Background (Hangfire/FireAndForget) حتى لا تبطئ الـ API
        Task SendOtpEmailAsync(string toEmail, string otpCode);
        Task SendNewLoginAlertAsync(string toEmail, string deviceInfo, DateTime loginTime);
    }
}
