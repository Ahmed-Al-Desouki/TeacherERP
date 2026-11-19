using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class PaymobPaymentIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string HmacSecret = "test-paymob-hmac-secret";
    private const int VpcIntegrationId = 6068264;
    private readonly CustomWebApplicationFactory _factory;

    private sealed record Scenario(SeededTeacher Teacher, Guid StudentId, Guid SessionId, Guid ObligationId);

    public PaymobPaymentIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    private async Task<Scenario> SeedAsync(decimal debt = 75m, decimal perSessionPrice = 75m,
        decimal? monthlyPrice = null)
    {
        Scenario? scenario = null;
        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().SingleAsync(x => x.Id == teacher.GroupId);
            group.UpdatePricing(perSessionPrice, monthlyPrice);
            var studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var sessionTime = DateTime.UtcNow.AddMinutes(-5);
            var sessionId = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId,
                sessionTime, TimeSpan.FromMinutes(45));
            db.AttendanceRecords.Add(AttendanceRecord.RecordPresence(teacher.TenantId, sessionId,
                studentId, ScanMethod.SelfScan, 30.0561, 31.3301));
            var obligation = FinancialObligation.Create(teacher.TenantId, studentId,
                teacher.GroupId, sessionId, "Paymob test group", sessionTime, debt);
            db.FinancialObligations.Add(obligation);
            var member = await db.TenantMembers.IgnoreQueryFilters().SingleAsync(x =>
                x.TenantId == teacher.TenantId && x.UserId == studentId);
            member.DeductBalance(debt);
            await db.SaveChangesAsync();
            scenario = new Scenario(teacher, studentId, sessionId, obligation.Id);
        });
        return scenario!;
    }

    [Fact]
    public async Task CreateIntent_IsIdempotentAndRejectsSameKeyWithDifferentRequest()
    {
        var scenario = await SeedAsync();
        const string key = "paymob-idem-key-001";
        var initialGatewayCount = _factory.PaymobGateway.CreateCount;

        var first = await CreateIntentAsync(scenario, key, 75m);
        var firstBody = await first.Content.ReadFromJsonAsync<JsonElement>();
        var second = await CreateIntentAsync(scenario, key, 75m);
        var secondBody = await second.Content.ReadFromJsonAsync<JsonElement>();
        var changedBody = await CreateIntentAsync(scenario, key, 50m);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(firstBody.GetProperty("transactionId").GetGuid(), secondBody.GetProperty("transactionId").GetGuid());
        Assert.Equal(firstBody.GetProperty("clientSecret").GetString(), secondBody.GetProperty("clientSecret").GetString());
        Assert.Equal(initialGatewayCount + 1, _factory.PaymobGateway.CreateCount);
        Assert.Equal(HttpStatusCode.Conflict, changedBody.StatusCode);
        Assert.Equal(VpcIntegrationId, _factory.PaymobGateway.LastRequest!.IntegrationId);
        Assert.Equal("EGP", firstBody.GetProperty("currency").GetString());
        Assert.Equal(TransactionStatus.PaymobPending.ToString(), firstBody.GetProperty("status").GetString());
    }

    [Fact]
    public async Task IntentRejectsNonStudentAndShopifyIntegration()
    {
        var scenario = await SeedAsync();
        var initialGatewayCount = _factory.PaymobGateway.CreateCount;
        var ownerClient = _factory.CreateClientFor(scenario.Teacher.OwnerId, scenario.Teacher.TenantId);
        using var ownerRequest = IntentRequest("owner-idempotency-001", scenario, 75m, "VpcOnlineNonShopify");
        Assert.Equal(HttpStatusCode.BadRequest, (await ownerClient.SendAsync(ownerRequest)).StatusCode);

        var student = _factory.CreateClientFor(scenario.StudentId, scenario.Teacher.TenantId, "Student");
        using var shopifyRequest = IntentRequest("shopify-idempotency-01", scenario, 75m, "VpcShopify");
        var shopify = await student.SendAsync(shopifyRequest);
        Assert.Equal(HttpStatusCode.BadRequest, shopify.StatusCode);
        Assert.Equal(initialGatewayCount, _factory.PaymobGateway.CreateCount);
    }

    [Fact]
    public async Task CashIntegrationUsesConfiguredIdAndRequiresNoRedirectUrl()
    {
        var scenario = await SeedAsync();
        var response = await CreateIntentAsync(scenario, "cash-idempotency-0001", 75m, "CashCashChannel");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(6068288, _factory.PaymobGateway.LastRequest!.IntegrationId);
        Assert.Null(_factory.PaymobGateway.LastRequest.RedirectionUrl);
        Assert.Equal(TransactionStatus.PaymobPending.ToString(), body.GetProperty("status").GetString());
        Assert.True(await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .AnyAsync(x => x.Id == body.GetProperty("transactionId").GetGuid()
                && x.Status == TransactionStatus.PaymobPending)));
    }

    [Fact]
    public async Task OnlineIntentRequiresConfiguredRedirectionUrlBeforeCallingPaymob()
    {
        var scenario = await SeedAsync();
        var previousUrl = _factory.PaymobSettings.RedirectionUrl;
        var initialGatewayCount = _factory.PaymobGateway.CreateCount;
        _factory.PaymobSettings.RedirectionUrl = string.Empty;

        try
        {
            var response = await CreateIntentAsync(scenario, "missing-redirect-key-01", 75m);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();

            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            Assert.Equal("Finance.PaymobRedirectionUrlRequired", body.GetProperty("errorCode").GetString());
            Assert.Equal(initialGatewayCount, _factory.PaymobGateway.CreateCount);
        }
        finally
        {
            _factory.PaymobSettings.RedirectionUrl = previousUrl;
        }
    }

    [Fact]
    public async Task BrowserReturn_IsPublicAndDoesNotTrustProviderQueryParameters()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/api/paymob/return?success=true&status=paid");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("تأكيد الدفع النهائي يعتمد على إشعار Paymob الآمن", html);
        Assert.DoesNotContain("success=true", html);
    }

    [Fact]
    public async Task UncertainProviderFailureKeepsAttemptPendingAndSameKeyDoesNotCreateAnotherIntentImmediately()
    {
        var scenario = await SeedAsync();
        const string key = "ambiguous-provider-key-01";
        var initialGatewayCount = _factory.PaymobGateway.CreateCount;
        _factory.PaymobGateway.FailNextWithCode = "Finance.PaymobUnavailable";

        var failedResponse = await CreateIntentAsync(scenario, key, 75m);
        var retryResponse = await CreateIntentAsync(scenario, key, 75m);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, failedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, retryResponse.StatusCode);
        Assert.Equal(initialGatewayCount + 1, _factory.PaymobGateway.CreateCount);
        var payment = await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .SingleAsync(x => x.IdempotencyKey == key));
        Assert.Equal(TransactionStatus.PaymobPending, payment.Status);
        Assert.Equal(scenario.SessionId, payment.PaymentContextSessionId);
    }

    [Fact]
    public async Task InvalidHmacDoesNotChangeTransactionOrLedger()
    {
        var scenario = await SeedAsync();
        var intent = await CreateIntentAsync(scenario, "hmac-idempotency-001", 75m);
        var body = await intent.Content.ReadFromJsonAsync<JsonElement>();
        using var callback = Callback(body, scenario, "810001", success: true, pending: false);
        var paymentId = body.GetProperty("transactionId").GetGuid();
        var eventCountBefore = await _factory.QueryAsync(db => db.PaymentEvents.IgnoreQueryFilters()
            .CountAsync(x => x.TransactionId == paymentId));
        var response = await SendCallbackAsync(callback, "not-a-valid-hmac");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(TransactionStatus.PaymobPending, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == body.GetProperty("transactionId").GetGuid()).Select(x => x.Status).SingleAsync()));
        Assert.Equal(0m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == scenario.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
        Assert.Equal(eventCountBefore, await _factory.QueryAsync(db => db.PaymentEvents.IgnoreQueryFilters()
            .CountAsync(x => x.TransactionId == paymentId)));
    }

    [Fact]
    public async Task VerifiedCallbackAppliesLedgerOnceAndDuplicateIsNoOp()
    {
        var scenario = await SeedAsync();
        var intent = await CreateIntentAsync(scenario, "success-idempotency-01", 75m);
        var body = await intent.Content.ReadFromJsonAsync<JsonElement>();
        using var callback = Callback(body, scenario, "810002", success: true, pending: false);
        var hmac = Sign(callback.RootElement.GetProperty("obj"));

        var first = await SendCallbackAsync(callback, hmac);
        var duplicate = await SendCallbackAsync(callback, hmac);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, duplicate.StatusCode);
        var paymentId = body.GetProperty("transactionId").GetGuid();
        Assert.Equal(TransactionStatus.Successful, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == paymentId).Select(x => x.Status).SingleAsync()));
        Assert.Equal(75m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == scenario.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
        Assert.Equal(1, await _factory.QueryAsync(db => db.PaymentEvents.IgnoreQueryFilters()
            .CountAsync(x => x.TransactionId == paymentId && x.ProviderEventId == "810002")));
        Assert.Equal(1, await _factory.QueryAsync(db => db.PlatformCommissions.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == scenario.Teacher.TenantId && x.StudentId == scenario.StudentId
                && x.SessionId == scenario.SessionId && x.Amount == 1m)));
        Assert.Equal(1, await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == scenario.Teacher.TenantId
                && x.Action == AuditAction.PaymobPaymentSucceeded)));

        var now = DateTime.UtcNow;
        var reportResponse = await _factory.CreateClientFor(
            scenario.Teacher.OwnerId, scenario.Teacher.TenantId).GetAsync(
            $"/api/Finance/monthly-collections?year={now.Year}&month={now.Month}");
        Assert.Equal(HttpStatusCode.OK, reportResponse.StatusCode);
        var report = await reportResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(75m, report.GetProperty("totals").GetProperty("successfulAmount").GetDecimal());
        Assert.Equal(0m, report.GetProperty("totals").GetProperty("pendingAmount").GetDecimal());
        Assert.Contains(report.GetProperty("byMethodAndType").EnumerateArray(), item =>
            item.GetProperty("method").GetString() == "Paymob"
            && item.GetProperty("type").GetString() == "PerSession"
            && item.GetProperty("successfulAmount").GetDecimal() == 75m);
    }

    [Fact]
    public async Task DeclineAndOutOfOrderPendingEventsNeverRevertSuccessfulPayment()
    {
        var scenario = await SeedAsync();
        var intent = await CreateIntentAsync(scenario, "order-idempotency-0001", 75m);
        var body = await intent.Content.ReadFromJsonAsync<JsonElement>();
        using var success = Callback(body, scenario, "810003", success: true, pending: false);
        using var decline = Callback(body, scenario, "810004", success: false, pending: false);
        using var pending = Callback(body, scenario, "810005", success: false, pending: true);

        Assert.Equal(HttpStatusCode.OK, (await SendCallbackAsync(success, Sign(success.RootElement.GetProperty("obj")))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await SendCallbackAsync(decline, Sign(decline.RootElement.GetProperty("obj")))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await SendCallbackAsync(pending, Sign(pending.RootElement.GetProperty("obj")))).StatusCode);

        var paymentId = body.GetProperty("transactionId").GetGuid();
        Assert.Equal(TransactionStatus.Successful, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == paymentId).Select(x => x.Status).SingleAsync()));
        Assert.Equal(75m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == scenario.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
    }

    [Fact]
    public async Task SecondSuccessfulProviderTransactionRequiresReviewInsteadOfApplyingLedgerTwice()
    {
        var scenario = await SeedAsync();
        var intent = await CreateIntentAsync(scenario, "double-success-key-01", 75m);
        var body = await intent.Content.ReadFromJsonAsync<JsonElement>();
        using var first = Callback(body, scenario, "810006", success: true, pending: false);
        using var second = Callback(body, scenario, "810007", success: true, pending: false);

        await SendCallbackAsync(first, Sign(first.RootElement.GetProperty("obj")));
        await SendCallbackAsync(second, Sign(second.RootElement.GetProperty("obj")));

        var paymentId = body.GetProperty("transactionId").GetGuid();
        Assert.Equal(TransactionStatus.PaymobAdjustmentReview, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == paymentId).Select(x => x.Status).SingleAsync()));
        Assert.Equal(75m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == scenario.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
        Assert.Equal(1, await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == scenario.Teacher.TenantId
                && x.Action == AuditAction.PaymobAdjustmentNeedsReview)));
    }

    [Fact]
    public async Task RefundCallbackMovesPaymentToReviewWithoutSilentlyReversingLedger()
    {
        var scenario = await SeedAsync();
        var intent = await CreateIntentAsync(scenario, "refund-review-key-01", 75m);
        var body = await intent.Content.ReadFromJsonAsync<JsonElement>();
        using var success = Callback(body, scenario, "810008", success: true, pending: false);
        using var refund = Callback(body, scenario, "810009", success: true, pending: false, isRefunded: true, refundedAmountCents: 7500);

        await SendCallbackAsync(success, Sign(success.RootElement.GetProperty("obj")));
        await SendCallbackAsync(refund, Sign(refund.RootElement.GetProperty("obj")));

        var paymentId = body.GetProperty("transactionId").GetGuid();
        Assert.Equal(TransactionStatus.PaymobAdjustmentReview, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == paymentId).Select(x => x.Status).SingleAsync()));
        Assert.Equal(scenario.SessionId, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == paymentId).Select(x => x.PaymentContextSessionId!.Value).SingleAsync()));
        Assert.Equal(75m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == scenario.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
    }

    private Task<HttpResponseMessage> CreateIntentAsync(Scenario scenario, string key, decimal amount,
        string integration = "VpcOnlineNonShopify")
    {
        var client = _factory.CreateClientFor(scenario.StudentId, scenario.Teacher.TenantId, "Student");
        return client.SendAsync(IntentRequest(key, scenario, amount, integration));
    }

    private static HttpRequestMessage IntentRequest(string key, Scenario scenario, decimal amount,
        string integration = "VpcOnlineNonShopify")
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/StudentPayments/paymob/intents")
        {
            Content = JsonContent.Create(new
            {
                sessionId = scenario.SessionId,
                type = "PerSession",
                amount,
                integration
            })
        };
        request.Headers.Add("Idempotency-Key", key);
        return request;
    }

    private static JsonDocument Callback(JsonElement intent, Scenario scenario, string providerEventId,
        bool success, bool pending, bool isRefunded = false, long refundedAmountCents = 0)
    {
        var callback = new
        {
            type = "TRANSACTION",
            obj = new
            {
                id = long.Parse(providerEventId, CultureInfo.InvariantCulture),
                pending,
                amount_cents = 7500L,
                success,
                is_auth = false,
                is_capture = false,
                is_standalone_payment = true,
                is_voided = false,
                is_refunded = isRefunded,
                is_3d_secure = true,
                integration_id = VpcIntegrationId,
                profile_id = 111,
                has_parent_transaction = false,
                order = new { id = long.Parse(intent.GetProperty("orderId").GetString()!, CultureInfo.InvariantCulture) },
                merchant_order_id = intent.GetProperty("transactionId").GetGuid().ToString("D"),
                created_at = "2026-11-19T12:00:00.000000Z",
                currency = "EGP",
                error_occured = !success,
                refunded_amount_cents = refundedAmountCents,
                owner = 111,
                source_data = new { pan = "2346", sub_type = "MasterCard", type = "card" }
            }
        };
        return JsonDocument.Parse(JsonSerializer.Serialize(callback));
    }

    private static string Sign(JsonElement obj)
    {
        var orderId = obj.GetProperty("order").GetProperty("id").GetRawText();
        var source = obj.GetProperty("source_data");
        var input = string.Concat(
            obj.GetProperty("amount_cents").GetRawText(),
            obj.GetProperty("created_at").GetString(),
            obj.GetProperty("currency").GetString(),
            obj.GetProperty("error_occured").GetBoolean() ? "true" : "false",
            obj.GetProperty("has_parent_transaction").GetBoolean() ? "true" : "false",
            obj.GetProperty("id").GetRawText(),
            obj.GetProperty("integration_id").GetRawText(),
            obj.GetProperty("is_3d_secure").GetBoolean() ? "true" : "false",
            obj.GetProperty("is_auth").GetBoolean() ? "true" : "false",
            obj.GetProperty("is_capture").GetBoolean() ? "true" : "false",
            obj.GetProperty("is_refunded").GetBoolean() ? "true" : "false",
            obj.GetProperty("is_standalone_payment").GetBoolean() ? "true" : "false",
            obj.GetProperty("is_voided").GetBoolean() ? "true" : "false",
            orderId,
            obj.GetProperty("owner").GetRawText(),
            obj.GetProperty("pending").GetBoolean() ? "true" : "false",
            source.GetProperty("pan").GetString(),
            source.GetProperty("sub_type").GetString(),
            source.GetProperty("type").GetString(),
            obj.GetProperty("success").GetBoolean() ? "true" : "false");
        return Convert.ToHexString(HMACSHA512.HashData(Encoding.UTF8.GetBytes(HmacSecret), Encoding.UTF8.GetBytes(input)))
            .ToLowerInvariant();
    }

    private async Task<HttpResponseMessage> SendCallbackAsync(JsonDocument callback, string hmac)
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/paymob/webhooks/transaction?hmac={hmac}")
        {
            Content = JsonContent.Create(callback.RootElement)
        };
        return await client.SendAsync(request);
    }
}
