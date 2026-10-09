using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;
using TeacherERP.Infrastructure.Settings;

namespace TeacherERP.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            var subject = "Your OTP for Smart Teacher ERP";
            var body = $"<h1>Welcome!</h1><p>Your verification code is: <strong>{otpCode}</strong></p><p>This code is valid for 10 minutes.</p>";
            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendNewLoginAlertAsync(string toEmail, string deviceInfo, DateTime loginTime, string? timeZoneId)
        {
            await SendEmailAsync(
                toEmail,
                "New login detected | تسجيل دخول جديد",
                EmailTemplates.NewLoginAlert(deviceInfo, loginTime, timeZoneId));
        }

        public async Task SendPasswordResetOtpAsync(string toEmail, string otpCode)
        {
            await SendEmailAsync(toEmail, "Reset your password | استرجاع كلمة المرور", EmailTemplates.PasswordResetOtp(otpCode));
        }

        public async Task SendPasswordChangedAlertAsync(string toEmail, DateTime changedAt, string? timeZoneId)
        {
            await SendEmailAsync(
                toEmail,
                "Your password was changed | تم تغيير كلمة المرور",
                EmailTemplates.PasswordChanged(changedAt, timeZoneId));
        }

        public async Task SendSessionEvictedAlertAsync(string toEmail, string evictedDeviceName, string newDeviceName, DateTime occurredAt, string? timeZoneId)
        {
            await SendEmailAsync(
                toEmail,
                "Signed out of a device | تم تسجيل خروجك من جهاز",
                EmailTemplates.SessionEvicted(evictedDeviceName, newDeviceName, occurredAt, timeZoneId));
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
                {
                    Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                    Subject = subject,
                    SubjectEncoding = Encoding.UTF8,
                    Body = body,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                throw; // سيتم التقاطها بواسطة الـ Background Job (Hangfire) ولن تعطل الـ API
            }
        }
    }
}