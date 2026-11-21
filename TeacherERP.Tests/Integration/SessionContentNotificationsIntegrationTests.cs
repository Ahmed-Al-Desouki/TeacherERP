using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class SessionContentNotificationsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private sealed record Scenario(
        SeededTeacher Teacher,
        Guid UploaderStudentId,
        Guid RecipientStudentId,
        Guid OtherGroupStudentId,
        Guid SessionId);

    private readonly CustomWebApplicationFactory _factory;

    public SessionContentNotificationsIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task HomeworkUpload_NotifiesOnlyOtherActiveStudentsInTheGroup_AndBuildsWhatsAppShareData()
    {
        var scenario = await SeedAsync("https://chat.whatsapp.com/group-invite");
        var adminClient = _factory.CreateClientFor(scenario.UploaderStudentId, scenario.Teacher.TenantId, "Student");

        var upload = await UploadAsync(adminClient, scenario.SessionId, "Homework", "Fractions practice.pdf");

        Assert.Equal(HttpStatusCode.OK, upload.StatusCode);
        var uploaderNotifications = await GetNotificationsAsync(scenario.UploaderStudentId, scenario.Teacher.TenantId);
        var uploaderNotification = Assert.Single(uploaderNotifications);
        Assert.Equal("HomeworkUploaded", uploaderNotification.GetProperty("type").GetString());
        var uploaderShare = await adminClient.GetAsync(
            $"/api/Notifications/{uploaderNotification.GetProperty("id").GetGuid()}/whatsapp-share");
        Assert.Equal(HttpStatusCode.OK, uploaderShare.StatusCode);

        var recipientNotifications = await GetNotificationsAsync(scenario.RecipientStudentId, scenario.Teacher.TenantId);
        var notification = Assert.Single(recipientNotifications);
        Assert.Equal("HomeworkUploaded", notification.GetProperty("type").GetString());
        Assert.Equal("New homework was uploaded to your group.", notification.GetProperty("message").GetString());
        var notificationId = notification.GetProperty("id").GetGuid();

        var recipient = _factory.CreateClientFor(scenario.RecipientStudentId, scenario.Teacher.TenantId, "Student");
        var shareResponse = await recipient.GetAsync($"/api/Notifications/{notificationId}/whatsapp-share");
        Assert.Equal(HttpStatusCode.OK, shareResponse.StatusCode);
        var share = await shareResponse.Content.ReadFromJsonAsync<JsonElement>();
        var shareText = share.GetProperty("shareText").GetString();
        Assert.Contains("واجب جديد", shareText);
        Assert.Contains("Fractions practice.pdf", shareText);
        Assert.Contains("https://storage.test/", shareText);
        Assert.Equal("https://chat.whatsapp.com/group-invite", share.GetProperty("whatsAppGroupLink").GetString());
        Assert.Equal($"https://wa.me/?text={Uri.EscapeDataString(shareText!)}",
            share.GetProperty("whatsAppComposeUrl").GetString());

        await _factory.SeedAsync(async db =>
        {
            var enrollment = await db.GroupEnrollments.IgnoreQueryFilters()
                .SingleAsync(item => item.TenantId == scenario.Teacher.TenantId
                    && item.GroupId == scenario.Teacher.GroupId
                    && item.StudentId == scenario.RecipientStudentId);
            enrollment.End(DateTime.UtcNow);
            await db.SaveChangesAsync();
        });
        var formerStudentShare = await recipient.GetAsync($"/api/Notifications/{notificationId}/whatsapp-share");
        Assert.NotEqual(HttpStatusCode.OK, formerStudentShare.StatusCode);

        var unrelated = _factory.CreateClientFor(scenario.OtherGroupStudentId, scenario.Teacher.TenantId, "Student");
        Assert.Empty(await GetNotificationsAsync(scenario.OtherGroupStudentId, scenario.Teacher.TenantId));
        var unrelatedShare = await unrelated.GetAsync($"/api/Notifications/{notificationId}/whatsapp-share");
        Assert.NotEqual(HttpStatusCode.OK, unrelatedShare.StatusCode);
    }

    [Fact]
    public async Task SessionMaterialUpload_NotifiesGroup_AndShareWorksWithoutAGroupInviteLink()
    {
        var scenario = await SeedAsync(null);
        var adminClient = _factory.CreateClientFor(scenario.UploaderStudentId, scenario.Teacher.TenantId, "Student");

        var upload = await UploadAsync(adminClient, scenario.SessionId, "Explanation", "Lesson explanation.pdf");

        Assert.Equal(HttpStatusCode.OK, upload.StatusCode);
        var recipientNotifications = await GetNotificationsAsync(scenario.RecipientStudentId, scenario.Teacher.TenantId);
        var notification = Assert.Single(recipientNotifications);
        Assert.Equal("SessionAttachmentUploaded", notification.GetProperty("type").GetString());

        var recipient = _factory.CreateClientFor(scenario.RecipientStudentId, scenario.Teacher.TenantId, "Student");
        var shareResponse = await recipient.GetAsync(
            $"/api/Notifications/{notification.GetProperty("id").GetGuid()}/whatsapp-share");
        Assert.Equal(HttpStatusCode.OK, shareResponse.StatusCode);
        var share = await shareResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Null, share.GetProperty("whatsAppGroupLink").ValueKind);
        Assert.Contains("شرح جديد", share.GetProperty("shareText").GetString());
        Assert.StartsWith("https://wa.me/?text=", share.GetProperty("whatsAppComposeUrl").GetString());
    }

    private async Task<Scenario> SeedAsync(string? whatsAppGroupLink)
    {
        Scenario? scenario = null;
        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters()
                .SingleAsync(item => item.Id == teacher.GroupId);
            group.UpdateDetails(group.Name, group.GradeLevel, group.CenterId,
                group.Room, group.Capacity, whatsAppGroupLink);

            var uploaderId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var recipientId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var groupAdminEnrollment = await db.GroupEnrollments.IgnoreQueryFilters()
                .SingleAsync(item => item.TenantId == teacher.TenantId
                    && item.GroupId == teacher.GroupId && item.StudentId == uploaderId);
            groupAdminEnrollment.AppointAsGroupAdmin(DateTime.UtcNow);

            var otherGroupId = await TestData.AddGroupAsync(db, teacher);
            var otherGroupStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, otherGroupId);
            var sessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromHours(1));
            await db.SaveChangesAsync();

            scenario = new Scenario(teacher, uploaderId, recipientId, otherGroupStudentId, sessionId);
        });

        return scenario!;
    }

    private async Task<HttpResponseMessage> UploadAsync(
        HttpClient client, Guid sessionId, string kind, string fileName)
    {
        using var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(Encoding.UTF8.GetBytes("%PDF-1.7\nTest attachment content"));
        file.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        form.Add(file, "file", fileName);
        form.Add(new StringContent(kind), "kind");
        form.Add(new StringContent("New material"), "caption");

        return await client.PostAsync($"/api/Schedule/sessions/{sessionId}/attachments", form);
    }

    private async Task<JsonElement[]> GetNotificationsAsync(Guid userId, Guid tenantId)
    {
        var client = _factory.CreateClientFor(userId, tenantId, "Student");
        var response = await client.GetAsync("/api/Notifications?page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("items").EnumerateArray().ToArray();
    }
}
