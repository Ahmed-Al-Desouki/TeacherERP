using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TeacherERP.Infrastructure.Settings;

namespace TeacherERP.API.HealthChecks;

public sealed class CoreConfigurationHealthCheck(IOptions<JwtSettings> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        var configured = !string.IsNullOrWhiteSpace(settings.Issuer)
            && !string.IsNullOrWhiteSpace(settings.Audience)
            && !string.IsNullOrWhiteSpace(settings.Secret)
            && System.Text.Encoding.UTF8.GetByteCount(settings.Secret) >= 32
            && settings.ExpiryMinutes > 0;

        return Task.FromResult(configured
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Required authentication configuration is incomplete."));
    }
}
