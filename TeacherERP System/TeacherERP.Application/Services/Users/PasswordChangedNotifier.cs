using Hangfire;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Services.Users;

public sealed class PasswordChangedNotifier(
    IBackgroundJobClient backgroundJobClient,
    IUserTimeZoneAccessor userTimeZoneAccessor,
    TimeProvider timeProvider,
    ILogger<PasswordChangedNotifier> logger) : IPasswordChangedNotifier
{
    public void Enqueue(ApplicationUser user)
    {
        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email)) return;

        var changedAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        var requestTimeZoneId = userTimeZoneAccessor.TimeZoneId;
        var timeZoneId = string.IsNullOrWhiteSpace(requestTimeZoneId) ? user.TimeZoneId : requestTimeZoneId;
        try
        {
            backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendPasswordChangedAlertAsync(email, changedAtUtc, timeZoneId));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to enqueue a password-change alert.");
        }
    }
}
