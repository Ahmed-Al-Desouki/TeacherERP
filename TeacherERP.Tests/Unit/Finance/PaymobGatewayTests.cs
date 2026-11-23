using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Finance;
using TeacherERP.Application.Interfaces;
using TeacherERP.Infrastructure.Services;

namespace TeacherERP.Tests.Unit;

public sealed class PaymobGatewayTests
{
    [Fact]
    public async Task CreateIntent_LogsStructuredProviderRejectionWithoutBillingDataOrSecret()
    {
        const string email = "student@example.test";
        const string phone = "+201000000001";
        const string secret = "sk_test_do_not_log";
        var handler = new StubHandler(new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
        {
            Content = new StringContent(
                $$"""{"detail":"Integration is not enabled for currency EGP","email":"{{email}}","phone":"{{phone}}"}""",
                Encoding.UTF8, "application/json")
        });
        var logger = new RecordingLogger<PaymobGateway>();
        var gateway = new PaymobGateway(new HttpClient(handler), Options.Create(new PaymobSettings
        {
            BaseUrl = "https://accept.paymob.com",
            SecretKey = secret
        }), logger);

        var result = await gateway.CreateIntentAsync(new PaymobIntentCommandData(
            100m, "EGP", 1800, 6068264, "internal-transaction-reference",
            "https://smart-teachererp.runasp.net/api/paymob/webhooks/transaction", null,
            "Test Student", email, phone, "Session payment", "TeacherERP PerSession payment"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Finance.PaymobRequestRejected", result.Error.Code);
        var log = Assert.Single(logger.Messages);
        Assert.Contains("HTTP 422", log);
        Assert.Contains("Integration is not enabled for currency EGP", log);
        Assert.DoesNotContain(email, log);
        Assert.DoesNotContain(phone, log);
        Assert.DoesNotContain(secret, log);
    }

    [Fact]
    public async Task CreateIntent_UsesSanitizedBodyWhenProviderErrorSchemaIsUnknown()
    {
        const string email = "student@example.test";
        const string phone = "+201000000001";
        const string secret = "sk_test_do_not_log";
        var handler = new StubHandler(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                $$"""{"reason":"Invalid merchant integration","customer":"{{email}} {{phone}}"}""",
                Encoding.UTF8, "application/json")
        });
        var logger = new RecordingLogger<PaymobGateway>();
        var gateway = new PaymobGateway(new HttpClient(handler), Options.Create(new PaymobSettings
        {
            BaseUrl = "https://accept.paymob.com",
            SecretKey = secret
        }), logger);

        var result = await gateway.CreateIntentAsync(new PaymobIntentCommandData(
            100m, "EGP", 1800, 6068264, "internal-transaction-reference",
            "https://smart-teachererp.runasp.net/api/paymob/webhooks/transaction", null,
            "Test Student", email, phone, "Session payment", "TeacherERP PerSession payment"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        var log = Assert.Single(logger.Messages);
        Assert.Contains("Invalid merchant integration", log);
        Assert.DoesNotContain(email, log);
        Assert.DoesNotContain(phone, log);
        Assert.DoesNotContain(secret, log);
    }

    private sealed class StubHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) => Task.FromResult(response);
    }

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
