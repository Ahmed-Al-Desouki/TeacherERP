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
            var commissionSettings = await db.PlatformCommissionSettings.IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == PlatformCommissionSettings.GlobalSettingsId);
            if (commissionSettings is null)
                db.PlatformCommissionSettings.Add(PlatformCommissionSettings.CreateDefault());
            else
                commissionSettings.Update(PlatformCommissionSettings.DefaultPerSessionAmount,
                    PlatformCommissionSettings.DefaultMonthlySubscriptionAmount, teacher.OwnerId, DateTime.UtcNow);
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
        Assert.Empty(await _factory.QueryAsync(db => db.PlatformCommissions.IgnoreQueryFilters()
            .Where(x => x.TenantId == s.Teacher.TenantId).ToListAsync()));
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
        Assert.Empty(await _factory.QueryAsync(db => db.PlatformCommissions.IgnoreQueryFilters()
            .Where(x => x.TenantId == s.Teacher.TenantId).ToListAsync()));
    }

    [Fact]
    public async Task FullInstaPaySessionPayment_CreatesOneCommission_AndAdministratorCanSettleTheMonth()
    {
        var s = await SeedAsync(debt: 100m);
        var submitted = await Student(s).PostAsync("/api/StudentPayments/receipts",
            ReceiptForm(s, PaymentType.PerSession, 100m));
        var transactionId = await ReceiptIdAsync(submitted);
        Assert.Empty(await _factory.QueryAsync(db => db.PlatformCommissions.IgnoreQueryFilters()
            .Where(x => x.TenantId == s.Teacher.TenantId).ToListAsync()));

        var approved = await Owner(s).PostAsync($"/api/Finance/instapay-receipts/{transactionId}/approve", null);
        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        Assert.Equal(1, await _factory.QueryAsync(db => db.PlatformCommissions.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == s.Teacher.TenantId && x.StudentId == s.StudentId
                && x.SessionId == s.SessionId && x.Amount == 1m && x.SettledAtUtc == null)));

        await _factory.SeedAsync(async db =>
        {
            var otherTeacher = await TestData.CreateTeacherAsync(db);
            var otherStudentId = await TestData.AddStudentAsync(db, otherTeacher.TenantId, otherTeacher.GroupId);
            var otherPayment = Transaction.CreatePayment(otherTeacher.TenantId, otherStudentId, otherTeacher.OwnerId,
                75m, PaymentType.PerSession);
            db.Transactions.Add(otherPayment);
            db.PlatformCommissions.Add(PlatformCommission.ForSession(otherTeacher.TenantId, otherStudentId,
                otherTeacher.GroupId, otherPayment.Id, Guid.NewGuid(), 1m));
            await db.SaveChangesAsync();
        });

        var admin = _factory.CreateClientFor(Guid.NewGuid(), role: "Administrator");
        var now = DateTime.UtcNow;
        var page = await admin.GetAsync($"/api/administration/platform-commissions?year={now.Year}&month={now.Month}&unsettledOnly=true");
        Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        var summaryResponse = await admin.GetAsync($"/api/administration/platform-commissions/summary?year={now.Year}&month={now.Month}");
        Assert.Equal(HttpStatusCode.OK, summaryResponse.StatusCode);
        var summary = await summaryResponse.Content.ReadFromJsonAsync<JsonElement>();
        var teacherSummary = summary.EnumerateArray().Single(x =>
            x.GetProperty("tenantId").GetGuid() == s.Teacher.TenantId);
        Assert.Equal(1m, teacherSummary.GetProperty("outstandingAmount").GetDecimal());
        Assert.Equal(1, teacherSummary.GetProperty("outstandingCount").GetInt32());

        var teacherSummaryResponse = await Owner(s).GetAsync(
            $"/api/Finance/platform-commissions/summary?year={now.Year}&month={now.Month}");
        Assert.Equal(HttpStatusCode.OK, teacherSummaryResponse.StatusCode);
        var ownSummary = await teacherSummaryResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(s.Teacher.TenantId, ownSummary.GetProperty("tenantId").GetGuid());
        Assert.Equal(1m, ownSummary.GetProperty("outstandingAmount").GetDecimal());
        var ownDetailsResponse = await Owner(s).GetAsync(
            $"/api/Finance/platform-commissions?year={now.Year}&month={now.Month}");
        var ownDetails = await ownDetailsResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Single(ownDetails.GetProperty("items").EnumerateArray());
        Assert.Equal(s.Teacher.TenantId,
            ownDetails.GetProperty("items")[0].GetProperty("tenantId").GetGuid());
        Assert.Equal(HttpStatusCode.Forbidden, (await Student(s).GetAsync(
            $"/api/Finance/platform-commissions/summary?year={now.Year}&month={now.Month}")).StatusCode);

        var requested = await admin.PostAsJsonAsync("/api/administration/platform-commissions/request-month",
            new { tenantId = s.Teacher.TenantId, year = now.Year, month = now.Month });
        Assert.Equal(HttpStatusCode.OK, requested.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().AnyAsync(x =>
            x.RecipientUserId == s.Teacher.OwnerId && x.Type == NotificationType.PlatformCommissionPaymentRequested
            && x.Message.Contains("1.00 EGP") && x.Message.Contains("payment instructions")
            && x.Message.Contains("transfer reference"))));
        var teacherDetails = await Owner(s).GetAsync(
            $"/api/Finance/platform-commissions?year={now.Year}&month={now.Month}&unsettledOnly=true");
        var teacherItems = await teacherDetails.Content.ReadFromJsonAsync<JsonElement>();
        Assert.NotEqual(JsonValueKind.Null,
            teacherItems.GetProperty("items")[0].GetProperty("collectionRequestedAtUtc").ValueKind);
        var duplicateRequest = await admin.PostAsJsonAsync("/api/administration/platform-commissions/request-month",
            new { tenantId = s.Teacher.TenantId, year = now.Year, month = now.Month });
        Assert.Equal(HttpStatusCode.BadRequest, duplicateRequest.StatusCode);

        var settled = await admin.PostAsJsonAsync("/api/administration/platform-commissions/settle-month",
            new { tenantId = s.Teacher.TenantId, year = now.Year, month = now.Month, settlementReference = "TEST-TRANSFER-001" });
        Assert.Equal(HttpStatusCode.OK, settled.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.PlatformCommissions.IgnoreQueryFilters().AnyAsync(x =>
            x.TenantId == s.Teacher.TenantId && x.SessionId == s.SessionId && x.SettledAtUtc != null
            && x.SettlementReference == "TEST-TRANSFER-001")));
        Assert.True(await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters().AnyAsync(x =>
            x.TenantId == s.Teacher.TenantId && x.Action == AuditAction.PlatformCommissionSettled)));
        Assert.Equal(HttpStatusCode.Forbidden, (await Owner(s).GetAsync(
            $"/api/administration/platform-commissions?year={now.Year}&month={now.Month}")).StatusCode);
    }

    [Fact]
    public async Task MonthlyCollectionsReport_SeparatesPaymentMethodsStatusesAndAttendanceDebt()
    {
        var instaPayTeacher = await SeedAsync(debt: 100m, perSessionPrice: 100m);
        var otherMethodTeacher = await SeedAsync(debt: 75m, perSessionPrice: 75m);
        var now = DateTime.UtcNow;

        var receipt = await Student(instaPayTeacher).PostAsync("/api/StudentPayments/receipts",
            ReceiptForm(instaPayTeacher, PaymentType.PerSession, 100m));
        Assert.Equal(HttpStatusCode.OK, receipt.StatusCode);
        var receiptId = await ReceiptIdAsync(receipt);
        var manualPayment = await Owner(otherMethodTeacher).PostAsJsonAsync("/api/Finance/manual-payments",
            new
            {
                studentId = otherMethodTeacher.StudentId,
                sessionId = otherMethodTeacher.SessionId,
                paymentType = PaymentType.PerSession,
                amount = 75m,
                method = PaymentProvider.Other
            });
        Assert.Equal(HttpStatusCode.OK, manualPayment.StatusCode);

        var teacherReportResponse = await Owner(instaPayTeacher).GetAsync(
            $"/api/Finance/monthly-collections?year={now.Year}&month={now.Month}");
        Assert.Equal(HttpStatusCode.OK, teacherReportResponse.StatusCode);
        var teacherReport = await teacherReportResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0m, teacherReport.GetProperty("totals").GetProperty("successfulAmount").GetDecimal());
        Assert.Equal(100m, teacherReport.GetProperty("totals").GetProperty("pendingAmount").GetDecimal());
        Assert.Equal(100m, teacherReport.GetProperty("totals").GetProperty("outstandingDebtAmount").GetDecimal());
        Assert.Equal("InstaPay", teacherReport.GetProperty("transactions").GetProperty("items")[0]
            .GetProperty("method").GetString());
        Assert.Equal(HttpStatusCode.Forbidden, (await Student(instaPayTeacher).GetAsync(
            $"/api/Finance/monthly-collections?year={now.Year}&month={now.Month}")).StatusCode);

        var admin = _factory.CreateClientFor(Guid.NewGuid(), role: "Administrator");
        var otherTenantReportResponse = await admin.GetAsync(
            $"/api/administration/financial-reports/monthly-collections?year={now.Year}&month={now.Month}&tenantId={otherMethodTeacher.Teacher.TenantId}");
        Assert.Equal(HttpStatusCode.OK, otherTenantReportResponse.StatusCode);
        var otherTenantReport = await otherTenantReportResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(75m, otherTenantReport.GetProperty("totals").GetProperty("successfulAmount").GetDecimal());
        Assert.Equal(0m, otherTenantReport.GetProperty("totals").GetProperty("pendingAmount").GetDecimal());
        Assert.Equal(0m, otherTenantReport.GetProperty("totals").GetProperty("outstandingDebtAmount").GetDecimal());
        Assert.Contains(otherTenantReport.GetProperty("transactions").GetProperty("items").EnumerateArray(),
            x => x.GetProperty("method").GetString() == "Other"
                && x.GetProperty("studentId").GetGuid() == otherMethodTeacher.StudentId);

        var instaPayTenantReportResponse = await admin.GetAsync(
            $"/api/administration/financial-reports/monthly-collections?year={now.Year}&month={now.Month}&tenantId={instaPayTeacher.Teacher.TenantId}");
        Assert.Equal(HttpStatusCode.OK, instaPayTenantReportResponse.StatusCode);
        var instaPayTenantReport = await instaPayTenantReportResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0m, instaPayTenantReport.GetProperty("totals").GetProperty("successfulAmount").GetDecimal());
        Assert.Equal(100m, instaPayTenantReport.GetProperty("totals").GetProperty("pendingAmount").GetDecimal());
        Assert.Equal(100m, instaPayTenantReport.GetProperty("totals").GetProperty("outstandingDebtAmount").GetDecimal());
        Assert.Contains(instaPayTenantReport.GetProperty("transactions").GetProperty("items").EnumerateArray(),
            x => x.GetProperty("method").GetString() == "InstaPay"
                && x.GetProperty("studentId").GetGuid() == instaPayTeacher.StudentId);
        Assert.True(await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters().AnyAsync(x =>
            x.TenantId == otherMethodTeacher.Teacher.TenantId
            && x.Action == AuditAction.OtherPaymentRecorded)));

        Assert.Equal(HttpStatusCode.OK, (await Owner(instaPayTeacher).PostAsync(
            $"/api/Finance/instapay-receipts/{receiptId}/approve", null)).StatusCode);
        var settledReportResponse = await Owner(instaPayTeacher).GetAsync(
            $"/api/Finance/monthly-collections?year={now.Year}&month={now.Month}");
        var settledReport = await settledReportResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(100m, settledReport.GetProperty("totals").GetProperty("successfulAmount").GetDecimal());
        Assert.Equal(0m, settledReport.GetProperty("totals").GetProperty("pendingAmount").GetDecimal());
        Assert.Equal(0m, settledReport.GetProperty("totals").GetProperty("outstandingDebtAmount").GetDecimal());
    }

    [Fact]
    public async Task AbsentStudent_DoesNotCreatePerSessionDebtOrAllowPaymentForThatSession()
    {
        Scenario? scenario = null;
        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().SingleAsync(x => x.Id == teacher.GroupId);
            group.UpdatePricing(75m, null);
            var studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var sessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddHours(-2), TimeSpan.FromMinutes(45));
            db.AttendanceRecords.Add(AttendanceRecord.RecordAbsence(teacher.TenantId, sessionId, studentId));
            await db.SaveChangesAsync();
            scenario = new Scenario(teacher, studentId, sessionId, Guid.Empty, null);
        });

        var absent = scenario!;
        var payment = await Owner(absent).PostAsJsonAsync("/api/Finance/cash-payments", new
        {
            studentId = absent.StudentId,
            sessionId = absent.SessionId,
            paymentType = PaymentType.PerSession,
            amount = 75m
        });
        Assert.Equal(HttpStatusCode.BadRequest, payment.StatusCode);
        Assert.False(await _factory.QueryAsync(db => db.FinancialObligations.IgnoreQueryFilters()
            .AnyAsync(x => x.TenantId == absent.Teacher.TenantId && x.SessionId == absent.SessionId)));

        var now = DateTime.UtcNow;
        var report = await Owner(absent).GetAsync(
            $"/api/Finance/monthly-collections?year={now.Year}&month={now.Month}");
        Assert.Equal(HttpStatusCode.OK, report.StatusCode);
        var body = await report.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0, body.GetProperty("totals").GetProperty("outstandingDebtCount").GetInt32());
    }

    [Fact]
    public async Task AdministratorCanChangeCommissionSettings_AndExistingCommissionKeepsItsOriginalAmount()
    {
        var s = await SeedAsync(debt: 100m);
        var admin = _factory.CreateClientFor(Guid.NewGuid(), role: "Administrator");
        var teacher = Owner(s);

        var defaults = await admin.GetAsync("/api/administration/platform-commissions/settings");
        Assert.Equal(HttpStatusCode.OK, defaults.StatusCode);
        var defaultSettings = await defaults.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(1m, defaultSettings.GetProperty("perSessionAmount").GetDecimal());
        Assert.Equal(5m, defaultSettings.GetProperty("monthlySubscriptionAmount").GetDecimal());
        Assert.Equal(HttpStatusCode.Forbidden, (await teacher.GetAsync(
            "/api/administration/platform-commissions/settings")).StatusCode);

        var invalid = await admin.PutAsJsonAsync("/api/administration/platform-commissions/settings",
            new { perSessionAmount = -0.01m, monthlySubscriptionAmount = 7.25m });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var updated = await admin.PutAsJsonAsync("/api/administration/platform-commissions/settings",
            new { perSessionAmount = 2.50m, monthlySubscriptionAmount = 7.25m });
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var updatedSettings = await updated.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2.50m, updatedSettings.GetProperty("perSessionAmount").GetDecimal());
        Assert.Equal(7.25m, updatedSettings.GetProperty("monthlySubscriptionAmount").GetDecimal());

        var submitted = await Student(s).PostAsync("/api/StudentPayments/receipts",
            ReceiptForm(s, PaymentType.PerSession, 100m));
        var transactionId = await ReceiptIdAsync(submitted);
        Assert.Equal(HttpStatusCode.OK, (await teacher.PostAsync(
            $"/api/Finance/instapay-receipts/{transactionId}/approve", null)).StatusCode);

        var changedAgain = await admin.PutAsJsonAsync("/api/administration/platform-commissions/settings",
            new { perSessionAmount = 3.00m, monthlySubscriptionAmount = 8.00m });
        Assert.Equal(HttpStatusCode.OK, changedAgain.StatusCode);
        Assert.Equal(2.50m, await _factory.QueryAsync(db => db.PlatformCommissions.IgnoreQueryFilters()
            .Where(x => x.TenantId == s.Teacher.TenantId && x.SessionId == s.SessionId)
            .Select(x => x.Amount).SingleAsync()));
        Assert.True(await _factory.QueryAsync(db => db.PlatformCommissionSettings
            .AnyAsync(x => x.PerSessionAmount == 3m && x.MonthlySubscriptionAmount == 8m)));
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
        Assert.Equal(5m, await _factory.QueryAsync(db => db.PlatformCommissions.IgnoreQueryFilters()
            .Where(x => x.TenantId == s.Teacher.TenantId && x.Type == PlatformCommissionType.MonthlySubscription)
            .Select(x => x.Amount).SingleAsync()));
    }
}
