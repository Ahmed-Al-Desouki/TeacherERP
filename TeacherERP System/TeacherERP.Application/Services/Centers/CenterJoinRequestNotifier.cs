using Hangfire;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Centers
{
    public class CenterJoinRequestNotifier : ICenterJoinRequestNotifier
    {
        private readonly IUserRepository _users;
        private readonly ITenantRepository _tenants;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly ILogger<CenterJoinRequestNotifier> _logger;

        public CenterJoinRequestNotifier(
            IUserRepository users,
            ITenantRepository tenants,
            IBackgroundJobClient backgroundJobs,
            ILogger<CenterJoinRequestNotifier> logger)
        {
            _users = users;
            _tenants = tenants;
            _backgroundJobs = backgroundJobs;
            _logger = logger;
        }

        public async Task<bool> NotifyOwnerAsync(
            Guid centerOwnerUserId, Guid teacherTenantId, string centerName, bool isReminder, CancellationToken cancellationToken)
        {
            var owner = await _users.GetByIdAsync(centerOwnerUserId, cancellationToken);
            var tenant = await _tenants.GetByIdAsync(teacherTenantId, cancellationToken);
            var teacher = tenant is null ? null : await _users.GetByIdAsync(tenant.OwnerId, cancellationToken);
            if (string.IsNullOrWhiteSpace(owner?.Email) || teacher is null)
            {
                _logger.LogWarning("Could not queue center join request email because the owner or teacher account is missing.");
                return false;
            }

            try
            {
                _backgroundJobs.Enqueue<IEmailService>(email => email.SendCenterJoinRequestEmailAsync(
                    owner.Email!, owner.FullName, teacher.FullName, centerName, isReminder));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not queue center join request email for owner {OwnerUserId}.", centerOwnerUserId);
                return false;
            }
        }
    }
}
