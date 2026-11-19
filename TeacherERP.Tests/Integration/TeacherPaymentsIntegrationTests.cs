using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class TeacherPaymentsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public TeacherPaymentsIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    private sealed record Scenario(SeededTeacher Teacher, Guid StudentId, Guid SessionId, Guid ObligationId, Guid? AssistantId);

    private async Task<Scenario> SeedAsync(decimal debt = 100m, decimal perSessionPrice = 75m,
        decimal? monthlyPrice = null, bool withPaymentSettings = true, bool withDelegatedAssistant = false)
    {
        Scenario? scenario = null;
        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().SingleAsync(x => x.Id == teacher.GroupId);
            group.UpdatePricing(perSessionPrice, monthlyPrice);
            var studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var sessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddMinutes(-5), TimeSpan.FromMinutes(45));
            db.AttendanceRecords.Add(AttendanceRecord.RecordPresence(teacher.TenantId, sessionId, studentId,
                ScanMethod.SelfScan, 30.0561, 31.3301));
            var obligation = FinancialObligation.Create(teacher.TenantId, studentId, teacher.GroupId, sessionId,
                "Test group", DateTime.UtcNow.AddMinutes(-5), debt);
            db.FinancialObligations.Add(obligation);
            var member = await db.TenantMembers.IgnoreQueryFilters()
                .SingleAsync(x => x.TenantId == teacher.TenantId && x.UserId == studentId);
            member.DeductBalance(debt);

            Guid? assistantId = null;
            if (withDelegatedAssistant)
            {
                var assistant = ApplicationUser.CreateAdmin("Finance Assistant", $"finance-{Guid.NewGuid():N}@test.local", "+201155500099");
                db.Users.Add(assistant);
                var assistantMembership = TenantMember.Create(teacher.TenantId, assistant.Id, TenantRole.Assistant);
                assistantMembership.SetFinancialDelegation(true);
                db.TenantMembers.Add(assistantMembership);
                assistantId = assistant.Id;
            }

            if (withPaymentSettings)
                db.TeacherPaymentSettings.Add(TeacherPaymentSettings.Create(teacher.TenantId,
                    "https://instapay.example/teacher", "https://storage.test/qr", $"test/qr/{Guid.NewGuid():N}"));

            await db.SaveChangesAsync();
            scenario = new Scenario(teacher, studentId, sessionId, obligation.Id, assistantId);
        });
        return scenario!;
    }

    private HttpClient Student(Scenario s) => _factory.CreateClientFor(s.StudentId, s.Teacher.TenantId, "Student");
    private static async Task<Guid> ReceiptIdAsync(HttpResponseMessage response)
        => (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("transactionId").GetGuid();
    private HttpClient Owner(Scenario s) => _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);

    private static MultipartFormDataContent ReceiptForm(Scenario s, PaymentType type, decimal amount, byte[]? image = null)
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(s.SessionId.ToString()), "sessionId");
        form.Add(new StringContent(type.ToString()), "type");
        form.Add(new StringContent(amount.ToString("0.00", CultureInfo.InvariantCulture)), "amount");
        var bytes = image ?? new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        var file = new ByteArrayContent(bytes);
        file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        form.Add(file, "receipt", "receipt.png");
        return form;
    }

    [Fact]
    public async Task Owner_ConfiguresInstaPay_StudentSeesDetailsAndSubmitsReceiptWithoutChangingLedger()
    {
        var s = await SeedAsync(withPaymentSettings: false, withDelegatedAssistant: true);
        var initialUploads = _factory.FileStorage.UploadCount;
        using var settingsForm = new MultipartFormDataContent();
        settingsForm.Add(new StringContent("https://instapay.example/teacher"), "instaPayLink");
        var qr = new ByteArrayContent(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
        qr.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        settingsForm.Add(qr, "qrImage", "teacher-qr.png");
        var configured = await Owner(s).PutAsync("/api/Finance/instapay-settings", settingsForm);

        Assert.Equal(HttpStatusCode.OK, configured.StatusCode);
        Assert.Equal(initialUploads + 1, _factory.FileStorage.UploadCount);

        using var linkOnlyUpdate = new MultipartFormDataContent();
        linkOnlyUpdate.Add(new StringContent("https://instapay.example/updated"), "instaPayLink");
        Assert.Equal(HttpStatusCode.OK, (await Owner(s).PutAsync("/api/Finance/instapay-settings", linkOnlyUpdate)).StatusCode);
        Assert.Equal(initialUploads + 1, _factory.FileStorage.UploadCount);

        var detailsResponse = await Student(s).GetAsync($"/api/StudentPayments/sessions/{s.SessionId}/payment-options");
        Assert.Equal(HttpStatusCode.OK, detailsResponse.StatusCode);
        var details = await detailsResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("https://instapay.example/updated", details.GetProperty("instaPay").GetProperty("instaPayLink").GetString());
        Assert.StartsWith("https://storage.test/", details.GetProperty("instaPay").GetProperty("qrImageUrl").GetString());

        var submitted = await Student(s).PostAsync("/api/StudentPayments/receipts", ReceiptForm(s, PaymentType.PerSession, 40m));
        var submittedId = await ReceiptIdAsync(submitted);
        var duplicate = await Student(s).PostAsync("/api/StudentPayments/receipts", ReceiptForm(s, PaymentType.PerSession, 40m));
        Assert.Equal(HttpStatusCode.OK, submitted.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);
        Assert.Equal(initialUploads + 2, _factory.FileStorage.UploadCount);
        Assert.Equal(0m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == s.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
        Assert.Equal(TransactionStatus.PendingApproval, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.TenantId == s.Teacher.TenantId && x.StudentId == s.StudentId)
            .Select(x => x.Status).SingleAsync()));
        Assert.Equal(HttpStatusCode.Forbidden, (await Student(s).GetAsync("/api/Finance/instapay-receipts/pending")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await _factory.CreateClientFor(s.AssistantId!.Value, s.Teacher.TenantId)
            .GetAsync("/api/Finance/instapay-receipts/pending")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await _factory.CreateClientFor(s.AssistantId.Value, s.Teacher.TenantId)
            .GetAsync("/api/Finance/instapay-settings")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await _factory.CreateClientFor(s.AssistantId.Value, s.Teacher.TenantId)
            .PostAsync($"/api/Finance/instapay-receipts/{submittedId}/approve", null)).StatusCode);
    }

    [Fact]
    public async Task ReceiptSubmission_RequiresTeacherSettingsBeforeUploading()
    {
        var s = await SeedAsync(withPaymentSettings: false);
        var initialUploads = _factory.FileStorage.UploadCount;
        var response = await Student(s).PostAsync("/api/StudentPayments/receipts",
            ReceiptForm(s, PaymentType.PerSession, 40m));
        var error = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Finance.PaymentSettingsRequired", error.GetProperty("errorCode").GetString());
        Assert.Equal(initialUploads, _factory.FileStorage.UploadCount);
        Assert.False(await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .AnyAsync(x => x.TenantId == s.Teacher.TenantId && x.StudentId == s.StudentId)));
    }

    [Fact]
    public async Task OwnerApproval_AppliesPerSessionPaymentOnceAndWritesAudit()
    {
        var s = await SeedAsync();
        var submitted = await Student(s).PostAsync("/api/StudentPayments/receipts", ReceiptForm(s, PaymentType.PerSession, 40m));
        Assert.Equal(HttpStatusCode.OK, submitted.StatusCode);
        var body = await ReceiptIdAsync(submitted);

        var approved = await Owner(s).PostAsync($"/api/Finance/instapay-receipts/{body}/approve", null);
        var repeated = await Owner(s).PostAsync($"/api/Finance/instapay-receipts/{body}/approve", null);

        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, repeated.StatusCode);
        var otherTeacher = await SeedAsync();
        Assert.Equal(HttpStatusCode.BadRequest, (await Owner(otherTeacher).PostAsync(
            $"/api/Finance/instapay-receipts/{body}/approve", null)).StatusCode);
        Assert.Equal(TransactionStatus.Successful, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == body).Select(x => x.Status).SingleAsync()));
        Assert.Equal(40m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == s.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
        Assert.True(await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters().AnyAsync(x =>
            x.TenantId == s.Teacher.TenantId && x.StudentId == s.StudentId && x.Status == TransactionStatus.Successful
            && x.ProcessedByAdminId == s.Teacher.OwnerId && x.ReceiptImagePublicId != null)));
        Assert.True(await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters().AnyAsync(x =>
            x.TenantId == s.Teacher.TenantId && x.ActorUserId == s.Teacher.OwnerId
            && x.TargetUserId == s.StudentId && x.Action == AuditAction.InstaPayPaymentApproved)));
    }

    [Fact]
    public async Task OwnerRejection_LeavesDebtUnchangedAndWritesAudit()
    {
        var s = await SeedAsync();
        var submitted = await Student(s).PostAsync("/api/StudentPayments/receipts", ReceiptForm(s, PaymentType.DebtSettlement, 20m));
        var id = await ReceiptIdAsync(submitted);

        var rejected = await Owner(s).PostAsync($"/api/Finance/instapay-receipts/{id}/reject", null);

        Assert.Equal(HttpStatusCode.OK, rejected.StatusCode);
        Assert.Equal(0m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == s.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
        Assert.Equal(TransactionStatus.Failed, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == id).Select(x => x.Status).SingleAsync()));
        Assert.True(await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters().AnyAsync(x =>
            x.TenantId == s.Teacher.TenantId && x.ActorUserId == s.Teacher.OwnerId
            && x.TargetUserId == s.StudentId && x.Action == AuditAction.InstaPayPaymentRejected)));

        var retried = await Student(s).PostAsync("/api/StudentPayments/receipts",
            ReceiptForm(s, PaymentType.DebtSettlement, 20m));
        var retryId = await ReceiptIdAsync(retried);
        var approved = await Owner(s).PostAsync($"/api/Finance/instapay-receipts/{retryId}/approve", null);
        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        Assert.Equal(20m, await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .Where(x => x.Id == s.ObligationId).Select(x => x.PaidAmount).SingleAsync()));
        Assert.True(await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters().AnyAsync(x =>
            x.Id == retryId && x.Status == TransactionStatus.Successful
            && x.RelatedSessionId == null && x.PaymentContextSessionId == null)));
    }

    [Fact]
    public async Task MonthlyReceiptApproval_CreatesSubscriptionAndUnsupportedImagesAreRejected()
    {
        var s = await SeedAsync(perSessionPrice: 75m, monthlyPrice: 200m);
        var invalid = await Student(s).PostAsync("/api/StudentPayments/receipts",
            ReceiptForm(s, PaymentType.MonthlySubscription, 200m, new byte[] { 1, 2, 3, 4 }));
        var oversizedBytes = new byte[10 * 1024 * 1024 + 1];
        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }.CopyTo(oversizedBytes, 0);
        var oversized = await Student(s).PostAsync("/api/StudentPayments/receipts",
            ReceiptForm(s, PaymentType.MonthlySubscription, 200m, oversizedBytes));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, oversized.StatusCode);
        Assert.Equal(0, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == s.Teacher.TenantId)));

        var submitted = await Student(s).PostAsync("/api/StudentPayments/receipts",
            ReceiptForm(s, PaymentType.MonthlySubscription, 200m));
        Assert.Equal(HttpStatusCode.OK, submitted.StatusCode);
        var id = await ReceiptIdAsync(submitted);
        var approved = await Owner(s).PostAsync($"/api/Finance/instapay-receipts/{id}/approve", null);

        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.MonthlySubscriptions.IgnoreQueryFilters()
            .AnyAsync(x => x.TenantId == s.Teacher.TenantId && x.StudentId == s.StudentId)));
    }
}
