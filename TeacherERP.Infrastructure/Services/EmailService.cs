using System.Net;
using System.Net.Mail;
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

        public async Task SendNewLoginAlertAsync(string toEmail, string deviceInfo, DateTime loginTime)
        {
            var subject = "New Login Alert";
            var body = $"<p>A new login was detected on your account.</p><p>Device: {deviceInfo}</p><p>Time: {loginTime:yyyy-MM-dd HH:mm:ss UTC}</p>";
            await SendEmailAsync(toEmail, subject, body);
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
                    Body = body,
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