using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.API.HealthChecks;

public sealed class DatabaseHealthCheck(AppDbContext context) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext healthContext, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await context.Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("Database is unavailable.");

            await context.Users.AsNoTracking().Select(user => user.Id).Take(1).ToListAsync(cancellationToken);
            await context.LoginSessions.AsNoTracking().Select(session => session.Id).Take(1).ToListAsync(cancellationToken);
            await context.TransientAuthStates.AsNoTracking().Select(state => state.Id).Take(1).ToListAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy("Database is unavailable.");
        }
    }
}
