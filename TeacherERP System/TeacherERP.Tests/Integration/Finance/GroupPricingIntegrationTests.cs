using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class GroupPricingIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GroupPricingIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateReadAndUpdateGroupPricing_PreservesRecordedTransactionAmount()
    {
        SeededTeacher teacher = null!;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
        });
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var created = await client.PostAsJsonAsync("/api/Groups", new
        {
            name = "Priced group",
            grade = new { stage = "Primary", number = 5 },
            centerId = teacher.CenterId,
            room = "Price room",
            capacity = 25,
            whatsAppGroupLink = (string?)null,
            slots = new[] { new { day = "Wednesday", startTime = "12:00:00", durationMinutes = 60 } },
            perSessionPrice = 125.50m,
            monthlyPrice = 500m
        });

        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        var createdDto = await created.Content.ReadFromJsonAsync<JsonElement>();
        var groupId = createdDto.GetProperty("id").GetGuid();
        Assert.Equal(125.50m, createdDto.GetProperty("perSessionPrice").GetDecimal());
        Assert.Equal(500m, createdDto.GetProperty("monthlyPrice").GetDecimal());

        Guid studentId = default;
        Guid paymentId = default;
        await _factory.SeedAsync(async db =>
        {
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, groupId);
            var sessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, groupId, DateTime.UtcNow.AddHours(-2));
            var payment = Transaction.CreatePayment(
                teacher.TenantId, studentId, teacher.OwnerId, 125.50m, PaymentType.PerSession,
                sessionId, "https://example.test/receipt.png");
            db.Transactions.Add(payment);
            await db.SaveChangesAsync();
            paymentId = payment.Id;
        });

        var updated = await client.PutAsJsonAsync($"/api/Groups/{groupId}", new
        {
            name = "Priced group",
            grade = new { stage = "Primary", number = 5 },
            centerId = teacher.CenterId,
            room = "Price room",
            capacity = 25,
            whatsAppGroupLink = (string?)null,
            isActive = true,
            perSessionPrice = 150.75m,
            monthlyPrice = 650m
        });

        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var read = await client.GetAsync($"/api/Groups/{groupId}");
        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        var readDto = await read.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(150.75m, readDto.GetProperty("perSessionPrice").GetDecimal());
        Assert.Equal(650m, readDto.GetProperty("monthlyPrice").GetDecimal());
        Assert.Equal(125.50m, await _factory.QueryAsync(db => db.Transactions.IgnoreQueryFilters()
            .Where(x => x.Id == paymentId).Select(x => x.Amount).SingleAsync()));
    }

    [Fact]
    public async Task GroupPricing_RejectsNegativeAmounts_AndAllowsUnsetOrFreePrices()
    {
        SeededTeacher teacher = null!;
        await _factory.SeedAsync(async db => teacher = await TestData.CreateTeacherAsync(db));
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var invalidCreate = await client.PostAsJsonAsync("/api/Groups", new
        {
            name = "Negative price group",
            grade = new { stage = "Primary", number = 2 },
            centerId = teacher.CenterId,
            room = (string?)null,
            capacity = (int?)null,
            whatsAppGroupLink = (string?)null,
            slots = new[] { new { day = "Thursday", startTime = "12:00:00", durationMinutes = 60 } },
            perSessionPrice = -1m,
            monthlyPrice = 0m
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidCreate.StatusCode);

        var invalidUpdate = await client.PutAsJsonAsync($"/api/Groups/{teacher.GroupId}", new
        {
            name = "Group",
            grade = new { stage = "Primary", number = 1 },
            centerId = teacher.CenterId,
            room = (string?)null,
            capacity = (int?)null,
            whatsAppGroupLink = (string?)null,
            isActive = true,
            perSessionPrice = 1.001m,
            monthlyPrice = 0m
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidUpdate.StatusCode);

        var freeOrUnset = await client.PutAsJsonAsync($"/api/Groups/{teacher.GroupId}", new
        {
            name = "Group",
            grade = new { stage = "Primary", number = 1 },
            centerId = teacher.CenterId,
            room = (string?)null,
            capacity = (int?)null,
            whatsAppGroupLink = (string?)null,
            isActive = true,
            perSessionPrice = (decimal?)null,
            monthlyPrice = 0m
        });

        Assert.Equal(HttpStatusCode.OK, freeOrUnset.StatusCode);
        var prices = await _factory.QueryAsync(db => db.Groups.IgnoreQueryFilters()
            .Where(g => g.Id == teacher.GroupId)
            .Select(g => new { g.PerSessionPrice, g.MonthlyPrice }).SingleAsync());
        Assert.Null(prices.PerSessionPrice);
        Assert.Equal(0m, prices.MonthlyPrice);
    }
}
