using Hangfire;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TeacherERP.API.HealthChecks;

public sealed class HangfireWorkerHealthCheck(TimeProvider timeProvider) : IHealthCheck
{
    private static readonly TimeSpan HeartbeatMaxAge = TimeSpan.FromMinutes(2);

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoff = timeProvider.GetUtcNow().UtcDateTime.Subtract(HeartbeatMaxAge);
            var hasLiveServer = JobStorage.Current.GetMonitoringApi().Servers()
                .Any(server => server.Heartbeat >= cutoff);

            return Task.FromResult(hasLiveServer
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("No active Hangfire worker is available."));
        }
        catch (Exception)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Hangfire storage is unavailable."));
        }
    }
}
