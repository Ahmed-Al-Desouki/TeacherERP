using System.Text.Json;

namespace TeacherERP.Tests.Integration.Common;

public sealed class HealthChecksIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;

    public HealthChecksIntegrationTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();
    public void Dispose() => _client.Dispose();

    [Fact]
    public async Task LiveEndpoint_ReportsProcessWithoutCheckingDependencies()
    {
        using var response = await _client.GetAsync("/health/live");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        using var body = JsonDocument.Parse(content);
        Assert.Equal("Healthy", body.RootElement.GetProperty("status").GetString());
        Assert.Equal("Healthy", body.RootElement.GetProperty("checks").GetProperty("process").GetString());
    }

    [Fact]
    public async Task ReadyEndpoint_ChecksDatabaseAndCoreAuthenticationConfiguration()
    {
        using var response = await _client.GetAsync("/health/ready");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        using var body = JsonDocument.Parse(content);
        var checks = body.RootElement.GetProperty("checks");
        Assert.Equal("Healthy", checks.GetProperty("database").GetString());
        Assert.Equal("Healthy", checks.GetProperty("authentication-configuration").GetString());
        Assert.DoesNotContain("tests-only-secret", content, StringComparison.Ordinal);
    }
}
