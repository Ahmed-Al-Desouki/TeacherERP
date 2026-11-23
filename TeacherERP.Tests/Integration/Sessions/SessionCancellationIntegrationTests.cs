using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace TeacherERP.Tests.Integration;

// F4: حصة (فردي أو جماعي) عليها تعويض معلّق لا تُلغى حتى يُبتّ فيه
public class SessionCancellationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string HasPendingCompensations = "Session.HasPendingCompensations";

    private readonly CustomWebApplicationFactory _factory;

    public SessionCancellationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    // مدرس + طالب مسجّل في مجموعته، وحصة مجموعته الأصلية، وحصة لمجموعة تانية حضرها الطالب تعويضاً (معلّق)
    private sealed record Scenario(
        SeededTeacher Teacher, Guid StudentId, Guid OriginalSessionId, Guid AttendedSessionId, Guid CompensationRecordId);

    private async Task<Scenario> SeedPendingCompensationAsync()
    {
        Scenario? scenario = null;

        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var otherGroupId = await TestData.AddGroupAsync(db, teacher);
            var studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);

            var original = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddDays(3));
            var attended = await TestData.AddSessionAsync(db, teacher.TenantId, otherGroupId, DateTime.UtcNow.AddDays(2));
            var record = await TestData.AddPendingCompensationAsync(db, teacher.TenantId, attended, studentId, original);

            scenario = new Scenario(teacher, studentId, original, attended, record);
        });

        return scenario!;
    }

    private static async Task<string?> ErrorCodeAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.TryGetProperty("errorCode", out var code) ? code.GetString() : null;
    }

    private async Task<bool> IsCancelledAsync(Guid sessionId)
        => await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters().Where(s => s.Id == sessionId).Select(s => s.IsCancelled).SingleAsync());

    [Fact]
    public async Task Cancel_SessionAttendedAsPendingCompensation_IsRejected()
    {
        var s = await SeedPendingCompensationAsync();
        var client = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);

        var response = await client.PostAsJsonAsync($"/api/Schedule/sessions/{s.AttendedSessionId}/cancel", new { reason = "holiday" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(HasPendingCompensations, await ErrorCodeAsync(response));
        Assert.False(await IsCancelledAsync(s.AttendedSessionId));
    }

    [Fact]
    public async Task Cancel_OriginalSessionOfAPendingCompensation_IsRejected()
    {
        var s = await SeedPendingCompensationAsync();
        var client = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);

        var response = await client.PostAsJsonAsync($"/api/Schedule/sessions/{s.OriginalSessionId}/cancel", new { reason = "holiday" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(HasPendingCompensations, await ErrorCodeAsync(response));
        Assert.False(await IsCancelledAsync(s.OriginalSessionId));
    }

    [Fact]
    public async Task Cancel_SucceedsAfterThePendingCompensationIsRejected()
    {
        var s = await SeedPendingCompensationAsync();
        var client = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);

        var blocked = await client.PostAsJsonAsync($"/api/Schedule/sessions/{s.AttendedSessionId}/cancel", new { reason = "holiday" });
        Assert.Equal(HttpStatusCode.BadRequest, blocked.StatusCode);

        var rejected = await client.PostAsync($"/api/Attendance/compensations/{s.CompensationRecordId}/reject", null);
        Assert.Equal(HttpStatusCode.OK, rejected.StatusCode);

        var cancelled = await client.PostAsJsonAsync($"/api/Schedule/sessions/{s.AttendedSessionId}/cancel", new { reason = "holiday" });

        Assert.Equal(HttpStatusCode.OK, cancelled.StatusCode);
        Assert.True(await IsCancelledAsync(s.AttendedSessionId));
    }

    [Fact]
    public async Task Cancel_SessionWithoutCompensations_Succeeds()
    {
        SeededTeacher teacher = null!;
        Guid sessionId = Guid.Empty;

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            sessionId = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddDays(2));
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var response = await client.PostAsJsonAsync($"/api/Schedule/sessions/{sessionId}/cancel", new { reason = "teacher is sick" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await IsCancelledAsync(sessionId));
    }

    [Fact]
    public async Task BulkCancel_WithAPendingCompensationInRange_CancelsNothing()
    {
        var s = await SeedPendingCompensationAsync();
        Guid freeSessionId = Guid.Empty;

        await _factory.SeedAsync(async db =>
        {
            freeSessionId = await TestData.AddSessionAsync(db, s.Teacher.TenantId, s.Teacher.GroupId, DateTime.UtcNow.AddDays(5));
        });

        var client = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);

        var response = await client.PostAsJsonAsync("/api/Schedule/sessions/bulk-cancel", new
        {
            from = DateTime.UtcNow.AddHours(1),
            to = DateTime.UtcNow.AddDays(10),
            groupIds = (Guid[]?)null,
            reason = "vacation"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(HasPendingCompensations, await ErrorCodeAsync(response));

        // إما يُلغى الكل أو لا شيء: حتى الحصة الحرة لم تُلغَ
        Assert.False(await IsCancelledAsync(freeSessionId));
        Assert.False(await IsCancelledAsync(s.AttendedSessionId));
        Assert.False(await IsCancelledAsync(s.OriginalSessionId));
    }

    [Fact]
    public async Task BulkCancel_WithoutPendingCompensations_CancelsAllSessionsInRange()
    {
        SeededTeacher teacher = null!;
        Guid first = Guid.Empty, second = Guid.Empty;

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            first = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddDays(2));
            second = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddDays(4));
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var response = await client.PostAsJsonAsync("/api/Schedule/sessions/bulk-cancel", new
        {
            from = DateTime.UtcNow.AddHours(1),
            to = DateTime.UtcNow.AddDays(10),
            groupIds = (Guid[]?)null,
            reason = "vacation"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2, body.GetProperty("affected").GetInt32());
        Assert.True(await IsCancelledAsync(first));
        Assert.True(await IsCancelledAsync(second));
    }
}
