using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class StudentLearningTimelineIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private sealed record Scenario(SeededTeacher Teacher, Guid StudentId, Guid GroupId,
        Guid PastSessionId, Guid UpcomingSessionId, Guid CancelledSessionId,
        Guid BeforeEnrollmentSessionId, Guid OtherGroupSessionId);

    private readonly CustomWebApplicationFactory _factory;

    public StudentLearningTimelineIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    private async Task<Scenario> SeedAsync()
    {
        Scenario? scenario = null;
        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var now = DateTime.UtcNow;
            var studentId = await TestData.AddStudentAsync(db, teacher.TenantId);
            db.GroupEnrollments.Add(GroupEnrollment.Create(teacher.TenantId, teacher.GroupId,
                studentId, now.AddDays(-50), null));

            var pastSessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, now.AddDays(-3));
            var upcomingSessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, now.AddDays(3));
            var cancelledSessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, now.AddDays(4));
            var cancelled = await db.Sessions.IgnoreQueryFilters()
                .SingleAsync(session => session.Id == cancelledSessionId);
            cancelled.Cancel("Teacher unavailable");
            var beforeEnrollmentSessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, now.AddDays(-60));

            var otherGroupId = await TestData.AddGroupAsync(db, teacher);
            var otherGroupStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, otherGroupId);
            var otherGroupSessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, otherGroupId, now.AddDays(-2));
            db.AttendanceRecords.Add(AttendanceRecord.RecordPresence(teacher.TenantId, pastSessionId,
                studentId, ScanMethod.AdminScan, null, null, teacher.OwnerId));
            db.AttendanceRecords.Add(AttendanceRecord.RecordAbsence(teacher.TenantId,
                otherGroupSessionId, otherGroupStudentId));

            db.SessionAttachments.Add(SessionAttachment.Create(teacher.TenantId, pastSessionId,
                SessionAttachmentKind.Explanation, "https://files.test/explanation.pdf",
                "test/timeline/explanation", "explanation.pdf", "application/pdf", 128, "Lesson notes", teacher.OwnerId));
            db.SessionAttachments.Add(SessionAttachment.Create(teacher.TenantId, pastSessionId,
                SessionAttachmentKind.Homework, "https://files.test/homework.pdf",
                "test/timeline/homework", "homework.pdf", "application/pdf", 256, null, teacher.OwnerId));
            db.SessionAttachments.Add(SessionAttachment.Create(teacher.TenantId, otherGroupSessionId,
                SessionAttachmentKind.Other, "https://files.test/private.pdf",
                "test/timeline/private", "private.pdf", "application/pdf", 512, null, teacher.OwnerId));
            await db.SaveChangesAsync();

            scenario = new Scenario(teacher, studentId, teacher.GroupId, pastSessionId,
                upcomingSessionId, cancelledSessionId, beforeEnrollmentSessionId, otherGroupSessionId);
        });

        return scenario!;
    }

    [Fact]
    public async Task Timeline_ReturnsOnlyCurrentStudentsSessionsWithAttendanceAndMaterials()
    {
        var scenario = await SeedAsync();
        var client = _factory.CreateClientFor(scenario.StudentId, scenario.Teacher.TenantId, "Student");
        var from = DateTime.UtcNow.AddDays(-80);
        var to = DateTime.UtcNow.AddDays(10);
        var response = await client.GetAsync($"/api/students/me/timeline?from={from:O}&to={to:O}&page=1&pageSize=20");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(3, body.GetProperty("totalCount").GetInt32());
        var items = body.GetProperty("items").EnumerateArray().ToArray();
        Assert.Equal(3, items.Length);
        Assert.All(items, item => Assert.Equal(scenario.GroupId, item.GetProperty("groupId").GetGuid()));
        Assert.DoesNotContain(items, item => item.GetProperty("sessionId").GetGuid() == scenario.BeforeEnrollmentSessionId);
        Assert.DoesNotContain(items, item => item.GetProperty("sessionId").GetGuid() == scenario.OtherGroupSessionId);

        var past = Assert.Single(items, item => item.GetProperty("sessionId").GetGuid() == scenario.PastSessionId);
        Assert.Equal("Present", past.GetProperty("attendanceStatus").GetString());
        var attachments = past.GetProperty("attachments").EnumerateArray().ToArray();
        Assert.Equal(2, attachments.Length);
        Assert.Contains(attachments, item => item.GetProperty("kind").GetString() == "Explanation");
        Assert.Contains(attachments, item => item.GetProperty("kind").GetString() == "Homework");

        var upcoming = Assert.Single(items, item => item.GetProperty("sessionId").GetGuid() == scenario.UpcomingSessionId);
        Assert.Equal(JsonValueKind.Null, upcoming.GetProperty("attendanceStatus").ValueKind);
        Assert.Empty(upcoming.GetProperty("attachments").EnumerateArray());

        var cancelled = Assert.Single(items, item => item.GetProperty("sessionId").GetGuid() == scenario.CancelledSessionId);
        Assert.True(cancelled.GetProperty("isCancelled").GetBoolean());
        Assert.Equal("Teacher unavailable", cancelled.GetProperty("cancelReason").GetString());
    }

    [Fact]
    public async Task Timeline_EnforcesStudentAccessRangeAndPagination()
    {
        var scenario = await SeedAsync();
        var student = _factory.CreateClientFor(scenario.StudentId, scenario.Teacher.TenantId, "Student");
        var now = DateTime.UtcNow;
        var from = Uri.EscapeDataString(now.AddDays(-10).ToString("O"));
        var to = Uri.EscapeDataString(now.AddDays(10).ToString("O"));
        var first = await student.GetAsync($"/api/students/me/timeline?from={from}&to={to}&groupId={scenario.GroupId}&page=1&pageSize=1");
        var second = await student.GetAsync($"/api/students/me/timeline?from={from}&to={to}&groupId={scenario.GroupId}&page=2&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        var firstBody = await first.Content.ReadFromJsonAsync<JsonElement>();
        var secondBody = await second.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(3, firstBody.GetProperty("totalCount").GetInt32());
        Assert.Equal(1, firstBody.GetProperty("items").GetArrayLength());
        Assert.Equal(1, secondBody.GetProperty("items").GetArrayLength());
        Assert.NotEqual(firstBody.GetProperty("items")[0].GetProperty("sessionId").GetGuid(),
            secondBody.GetProperty("items")[0].GetProperty("sessionId").GetGuid());

        var wideFrom = Uri.EscapeDataString(now.AddDays(-200).ToString("O"));
        var wideTo = Uri.EscapeDataString(now.AddDays(200).ToString("O"));
        var tooWide = await student.GetAsync(
            $"/api/students/me/timeline?from={wideFrom}&to={wideTo}");
        Assert.Equal(HttpStatusCode.BadRequest, tooWide.StatusCode);

        var teacher = _factory.CreateClientFor(scenario.Teacher.OwnerId, scenario.Teacher.TenantId);
        Assert.Equal(HttpStatusCode.Forbidden,
            (await teacher.GetAsync("/api/students/me/timeline")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await _factory.CreateClient().GetAsync("/api/students/me/timeline")).StatusCode);
    }

    [Fact]
    public async Task Timeline_HidesSessionsAfterTheStudentLeavesTheGroup()
    {
        var scenario = await SeedAsync();
        await _factory.SeedAsync(async db =>
        {
            var enrollment = await db.GroupEnrollments.IgnoreQueryFilters()
                .SingleAsync(item => item.TenantId == scenario.Teacher.TenantId
                    && item.StudentId == scenario.StudentId && item.GroupId == scenario.GroupId);
            enrollment.End(DateTime.UtcNow);
            await db.SaveChangesAsync();
        });

        var client = _factory.CreateClientFor(scenario.StudentId, scenario.Teacher.TenantId, "Student");
        var response = await client.GetAsync("/api/students/me/timeline");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0, body.GetProperty("totalCount").GetInt32());
        Assert.Empty(body.GetProperty("items").EnumerateArray());
    }
}
