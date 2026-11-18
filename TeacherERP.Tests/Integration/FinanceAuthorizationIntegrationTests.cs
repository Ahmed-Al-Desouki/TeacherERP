using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class FinanceAuthorizationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public FinanceAuthorizationIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task FinanceEndpoints_AllowOwner_RejectUndelegatedAssistant_AndAllowDelegatedAssistant()
    {
        SeededTeacher teacher = null!;
        Guid assistantId = default;
        Guid delegatedAssistantId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var assistant = ApplicationUser.CreateAdmin("Assistant", $"assistant-{Guid.NewGuid():N}@test.local", $"+201{Random.Shared.Next(100_000_000, 1_000_000_000)}");
            var delegated = ApplicationUser.CreateAdmin("Delegated Assistant", $"finance-{Guid.NewGuid():N}@test.local", $"+201{Random.Shared.Next(100_000_000, 1_000_000_000)}");
            assistantId = assistant.Id;
            delegatedAssistantId = delegated.Id;
            db.Users.AddRange(assistant, delegated);
            db.TenantMembers.Add(TenantMember.Create(teacher.TenantId, assistant.Id, TenantRole.Assistant));
            var delegatedMembership = TenantMember.Create(teacher.TenantId, delegated.Id, TenantRole.Assistant);
            delegatedMembership.SetFinancialDelegation(true);
            db.TenantMembers.Add(delegatedMembership);
            await db.SaveChangesAsync();
        });

        var path = $"api/Finance/students/{Guid.NewGuid()}/sessions/{Guid.NewGuid()}/payment-options";
        var owner = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId).GetAsync(path);
        var assistant = await _factory.CreateClientFor(assistantId, teacher.TenantId, "Assistant").GetAsync(path);
        var delegatedAssistant = await _factory.CreateClientFor(delegatedAssistantId, teacher.TenantId, "Assistant").GetAsync(path);

        Assert.Equal(HttpStatusCode.BadRequest, owner.StatusCode); // نجح التفويض وفشل فقط لأن الطالب والحصة غير موجودين
        Assert.Equal(HttpStatusCode.Forbidden, assistant.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, delegatedAssistant.StatusCode);
    }

    [Fact]
    public async Task MonthlyCashPayment_CoversTodaysDebtAndPersistsOverpaymentAsWalletCredit()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid sessionId = default;
        var now = DateTime.UtcNow;
        var year = now.Year;
        var month = now.Month;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().SingleAsync(x => x.Id == teacher.GroupId);
            group.UpdatePricing(50m, 500m);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var session = Session.Create(teacher.TenantId, teacher.GroupId, now.AddMinutes(-10), now.AddMinutes(50));
            sessionId = session.Id;
            db.Sessions.Add(session);
            db.AttendanceRecords.Add(AttendanceRecord.RecordPresence(
                teacher.TenantId, session.Id, studentId, ScanMethod.AdminScan, null, null, teacher.OwnerId));
            var member = await db.TenantMembers.IgnoreQueryFilters()
                .SingleAsync(x => x.TenantId == teacher.TenantId && x.UserId == studentId);
            member.DeductBalance(50m);
            db.FinancialObligations.Add(FinancialObligation.Create(
                teacher.TenantId, studentId, teacher.GroupId, session.Id, group.Name, session.StartTime, 50m));
            await db.SaveChangesAsync();
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var before = await client.GetAsync($"api/Finance/students/{studentId}/sessions/{sessionId}/payment-options");
        Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        using (var beforeJson = System.Text.Json.JsonDocument.Parse(await before.Content.ReadAsStringAsync()))
        {
            Assert.Equal(50m, beforeJson.RootElement.GetProperty("totalOutstanding").GetDecimal());
            Assert.False(beforeJson.RootElement.GetProperty("hasMonthlySubscription").GetBoolean());
        }

        var payment = await client.PostAsJsonAsync("api/Finance/cash-payments", new
        {
            studentId,
            sessionId,
            paymentType = "MonthlySubscription",
            amount = 550m
        });
        Assert.Equal(HttpStatusCode.OK, payment.StatusCode);

        var persisted = await _factory.QueryAsync(async db => new
        {
            Subscription = await db.MonthlySubscriptions.IgnoreQueryFilters().SingleAsync(x =>
                x.TenantId == teacher.TenantId && x.StudentId == studentId && x.GroupId == teacher.GroupId
                && x.Year == year && x.Month == month),
            Transaction = await db.Transactions.IgnoreQueryFilters().SingleAsync(x =>
                x.TenantId == teacher.TenantId && x.StudentId == studentId && x.Type == PaymentType.MonthlySubscription),
            Obligation = await db.FinancialObligations.IgnoreQueryFilters().SingleAsync(x => x.SessionId == sessionId),
            Balance = await db.TenantMembers.IgnoreQueryFilters().Where(x => x.TenantId == teacher.TenantId && x.UserId == studentId)
                .Select(x => x.CurrentBalance).SingleAsync()
        });
        Assert.Equal(500m, persisted.Subscription.Amount);
        Assert.Equal(550m, persisted.Transaction.Amount);
        Assert.Equal(TransactionStatus.Successful, persisted.Transaction.Status);
        Assert.True(persisted.Obligation.IsSettled);
        Assert.Equal(50m, persisted.Balance);

        var after = await client.GetAsync($"api/Finance/students/{studentId}/sessions/{sessionId}/payment-options");
        Assert.Equal(HttpStatusCode.OK, after.StatusCode);
        using var afterJson = System.Text.Json.JsonDocument.Parse(await after.Content.ReadAsStringAsync());
        Assert.True(afterJson.RootElement.GetProperty("hasMonthlySubscription").GetBoolean());
        Assert.Equal(50m, afterJson.RootElement.GetProperty("walletCredit").GetDecimal());
    }

    [Fact]
    public async Task FinanceOptions_DoNotExposeAnotherTeachersStudentOrSession()
    {
        SeededTeacher firstTeacher = null!;
        SeededTeacher otherTeacher = null!;
        Guid studentId = default;
        Guid sessionId = default;
        await _factory.SeedAsync(async db =>
        {
            firstTeacher = await TestData.CreateTeacherAsync(db);
            otherTeacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, otherTeacher.TenantId, otherTeacher.GroupId);
            var now = DateTime.UtcNow;
            var group = await db.Groups.IgnoreQueryFilters().SingleAsync(x => x.Id == otherTeacher.GroupId);
            group.UpdatePricing(50m, 500m);
            var session = Session.Create(otherTeacher.TenantId, otherTeacher.GroupId, now.AddMinutes(-5), now.AddMinutes(55));
            sessionId = session.Id;
            db.Sessions.Add(session);
            db.AttendanceRecords.Add(AttendanceRecord.RecordPresence(
                otherTeacher.TenantId, session.Id, studentId, ScanMethod.AdminScan, null, null, otherTeacher.OwnerId));
            await db.SaveChangesAsync();
        });

        var response = await _factory.CreateClientFor(firstTeacher.OwnerId, firstTeacher.TenantId)
            .GetAsync($"api/Finance/students/{studentId}/sessions/{sessionId}/payment-options");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
