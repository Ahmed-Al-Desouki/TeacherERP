using System.Net.Sockets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Finance;
using TeacherERP.Infrastructure.Settings;

namespace TeacherERP.API.HealthChecks;

public sealed class SmtpEndpointHealthCheck(IOptions<MailSettings> options, IMemoryCache cache) : IHealthCheck
{
    private const string CacheKey = "health-check:smtp-endpoint";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue<HealthCheckResult>(CacheKey, out var cached))
            return cached;

        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.Host) || settings.Port is < 1 or > 65535
            || string.IsNullOrWhiteSpace(settings.Mail) || string.IsNullOrWhiteSpace(settings.Password))
            return Cache(HealthCheckResult.Degraded("SMTP configuration is incomplete."));

        using var client = new TcpClient();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(2));
        try
        {
            await client.ConnectAsync(settings.Host, settings.Port, timeout.Token);
            return Cache(HealthCheckResult.Healthy());
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (ex is SocketException or OperationCanceledException)
        {
            return Cache(HealthCheckResult.Degraded("SMTP endpoint is unreachable."));
        }
    }

    private HealthCheckResult Cache(HealthCheckResult result)
    {
        cache.Set(CacheKey, result, TimeSpan.FromSeconds(30));
        return result;
    }
}

public sealed class CloudinaryConfigurationHealthCheck(IOptions<CloudinarySettings> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        var configured = !string.IsNullOrWhiteSpace(settings.CloudName)
            && !string.IsNullOrWhiteSpace(settings.ApiKey)
            && !string.IsNullOrWhiteSpace(settings.ApiSecret);
        return Task.FromResult(configured
            ? HealthCheckResult.Healthy("Cloudinary configuration is present.")
            : HealthCheckResult.Degraded("Cloudinary configuration is incomplete."));
    }
}

public sealed class PaymobConfigurationHealthCheck(IOptions<PaymobSettings> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        var configured = Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out _)
            && !string.IsNullOrWhiteSpace(settings.ApiKey)
            && !string.IsNullOrWhiteSpace(settings.SecretKey)
            && !string.IsNullOrWhiteSpace(settings.HmacSecret)
            && settings.IntegrationIds is { Count: > 0 };
        return Task.FromResult(configured
            ? HealthCheckResult.Healthy("Paymob configuration is present.")
            : HealthCheckResult.Degraded("Paymob configuration is incomplete."));
    }
}

public sealed class GoogleAuthConfigurationHealthCheck(IOptions<GoogleAuthSettings> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(options.Value.ClientIds is { Count: > 0 }
            ? HealthCheckResult.Healthy("Google authentication configuration is present.")
            : HealthCheckResult.Degraded("Google authentication configuration is incomplete."));
}
