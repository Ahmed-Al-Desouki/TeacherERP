using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class GroupAdminAttendanceIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GroupAdminAttendanceIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    private sealed record Scenario(SeededTeacher Teacher, Guid AdminStudentId, Guid TargetStudentId, Guid SessionId);

    private async Task<Scenario> SeedAsync()
    {
        Scenario? scenario = null;
        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var admin = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var target = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var session = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddMinutes(-5), TimeSpan.FromMinutes(45));
            var settings = await db.TenantSettings.IgnoreQueryFilters().SingleAsync(s => s.TenantId == teacher.TenantId);
            settings.UpdateAttendance(AttendanceMode.StaticGlobalQR, rejectMockLocations: false,
                autoRegisterOnScan: true, autoApproveGroupAdminScans: false);
            await db.SaveChangesAsync();
            scenario = new Scenario(teacher, admin, target, session);
        });
        return scenario!;
    }

    private async Task<string> GetQrAsync(Guid studentId)
        => await _factory.QueryAsync(db => db.Users.IgnoreQueryFilters().Where(u => u.Id == studentId)
            .Select(u => u.StaticQrCode).SingleAsync());

    private static async Task<string?> ErrorCodeAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.TryGetProperty("errorCode", out var code) ? code.GetString() : null;
    }

    [Fact]
    public async Task TeacherAppointsGroupAdmin_AndGroupAdminRecordsAttendanceForAnEnrolledStudent()
    {
        var s = await SeedAsync();
        var teacher = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);
        var appointed = await teacher.PutAsync(
            $"/api/Groups/{s.Teacher.GroupId}/admins/{s.AdminStudentId}", null);

        Assert.Equal(HttpStatusCode.OK, appointed.StatusCode);
        var listed = await teacher.GetAsync($"/api/Groups/{s.Teacher.GroupId}/admins");
        Assert.Equal(HttpStatusCode.OK, listed.StatusCode);
        Assert.Contains(s.AdminStudentId.ToString(), await listed.Content.ReadAsStringAsync());

        var admin = _factory.CreateClientFor(s.AdminStudentId, s.Teacher.TenantId, "Student");
        var groupAttendance = await admin.GetAsync("/api/Attendance/group-session");
        Assert.Equal(HttpStatusCode.OK, groupAttendance.StatusCode);
        var scan = await admin.PostAsJsonAsync("/api/Attendance/group-scan", new
        {
            studentQrCode = await GetQrAsync(s.TargetStudentId),
            latitude = 30.0561,
            longitude = 31.3301,
            isMockLocation = false
        });

        Assert.Equal(HttpStatusCode.OK, scan.StatusCode);
        var body = await scan.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Recorded", body.GetProperty("outcome").GetString());
        Assert.True(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters().AnyAsync(a =>
            a.TenantId == s.Teacher.TenantId && a.SessionId == s.SessionId && a.StudentId == s.TargetStudentId
            && a.ScannedVia == ScanMethod.AdminScan && a.RecordedByUserId == s.AdminStudentId)));
    }

    [Fact]
    public async Task GroupScan_RequiresAppointment_AndAdminCannotRecordSelf()
    {
        var s = await SeedAsync();
        var member = _factory.CreateClientFor(s.AdminStudentId, s.Teacher.TenantId, "Student");
        var payload = new
        {
            studentQrCode = await GetQrAsync(s.TargetStudentId),
            latitude = 30.0561,
            longitude = 31.3301,
            isMockLocation = false
        };

        var notAdmin = await member.PostAsJsonAsync("/api/Attendance/group-scan", payload);
        Assert.Equal(HttpStatusCode.Forbidden, notAdmin.StatusCode);
        Assert.Equal("GroupAdmin.NotAllowed", await ErrorCodeAsync(notAdmin));
        var hiddenAttendance = await member.GetAsync("/api/Attendance/group-session");
        Assert.Equal(HttpStatusCode.Forbidden, hiddenAttendance.StatusCode);

        var teacher = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);
        Assert.Equal(HttpStatusCode.OK, (await teacher.PutAsync(
            $"/api/Groups/{s.Teacher.GroupId}/admins/{s.AdminStudentId}", null)).StatusCode);

        await _factory.SeedAsync(async db =>
        {
            var settings = await db.TenantSettings.IgnoreQueryFilters().SingleAsync(x => x.TenantId == s.Teacher.TenantId);
            settings.UpdateAttendance(AttendanceMode.StaticGlobalQR, rejectMockLocations: true,
                autoRegisterOnScan: true, autoApproveGroupAdminScans: false);
            await db.SaveChangesAsync();
        });
        var mocked = await member.PostAsJsonAsync("/api/Attendance/group-scan", new
        {
            studentQrCode = await GetQrAsync(s.TargetStudentId),
            latitude = 30.0561,
            longitude = 31.3301,
            isMockLocation = true
        });
        Assert.Equal(HttpStatusCode.Forbidden, mocked.StatusCode);
        Assert.Equal("Attendance.MockLocation", await ErrorCodeAsync(mocked));

        var self = await member.PostAsJsonAsync("/api/Attendance/group-scan", new
        {
            studentQrCode = await GetQrAsync(s.AdminStudentId),
            latitude = 30.0561,
            longitude = 31.3301,
            isMockLocation = false
        });
        Assert.Equal(HttpStatusCode.Forbidden, self.StatusCode);
        Assert.Equal("GroupAdmin.CannotRecordSelf", await ErrorCodeAsync(self));
    }

    [Fact]
    public async Task AssistantGroupAdminChange_WaitsForTeacherApproval()
    {
        var s = await SeedAsync();
        Guid assistantId = default;
        await _factory.SeedAsync(async db =>
        {
            var assistant = ApplicationUser.CreateAssistant("Assistant", $"assistant-{Guid.NewGuid():N}@test.local");
            db.Users.Add(assistant);
            db.TenantMembers.Add(TenantMember.Create(s.Teacher.TenantId, assistant.Id, TenantRole.Assistant));
            await db.SaveChangesAsync();
            assistantId = assistant.Id;
        });

        var assistantClient = _factory.CreateClientFor(assistantId, s.Teacher.TenantId, "Assistant");
        var requested = await assistantClient.PutAsync(
            $"/api/Groups/{s.Teacher.GroupId}/admins/{s.AdminStudentId}", null);
        Assert.Equal(HttpStatusCode.Accepted, requested.StatusCode);
        var requestBody = await requested.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(requestBody.GetProperty("applied").GetBoolean());
        var requestId = requestBody.GetProperty("requestId").GetGuid();

        var teacher = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);
        var approved = await teacher.PostAsync($"/api/GroupAdmins/requests/{requestId}/approve", null);

        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters().AnyAsync(e =>
            e.TenantId == s.Teacher.TenantId && e.StudentId == s.AdminStudentId && e.GroupId == s.Teacher.GroupId
            && e.IsGroupAdmin)));
        Assert.Equal(GroupAdminRequestStatus.Approved, await _factory.QueryAsync(db => db.GroupAdminRequests
            .IgnoreQueryFilters().Where(r => r.Id == requestId).Select(r => r.Status).SingleAsync()));
        Assert.Equal(1, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(n =>
            n.TenantId == s.Teacher.TenantId && n.RecipientUserId == assistantId
            && n.Type == NotificationType.GroupAdminRequestApproved && n.RelatedId == requestId)));
        Assert.Equal(1, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(n =>
            n.TenantId == s.Teacher.TenantId && n.RecipientUserId == s.AdminStudentId
            && n.Type == NotificationType.GroupAdminRequestApproved && n.RelatedId == requestId)));

        var removeRequested = await assistantClient.DeleteAsync(
            $"/api/Groups/{s.Teacher.GroupId}/admins/{s.AdminStudentId}");
        Assert.Equal(HttpStatusCode.Accepted, removeRequested.StatusCode);
        var removal = await removeRequested.Content.ReadFromJsonAsync<JsonElement>();
        var removalId = removal.GetProperty("requestId").GetGuid();
        var rejected = await teacher.PostAsJsonAsync($"/api/GroupAdmins/requests/{removalId}/reject", new
        {
            note = "Keep the current group admin"
        });
        Assert.Equal(HttpStatusCode.OK, rejected.StatusCode);
        Assert.Equal(GroupAdminRequestStatus.Rejected, await _factory.QueryAsync(db => db.GroupAdminRequests
            .IgnoreQueryFilters().Where(r => r.Id == removalId).Select(r => r.Status).SingleAsync()));
        Assert.Equal(1, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(n =>
            n.TenantId == s.Teacher.TenantId && n.RecipientUserId == assistantId
            && n.Type == NotificationType.GroupAdminRequestRejected && n.RelatedId == removalId)));
        Assert.Equal(1, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(n =>
            n.TenantId == s.Teacher.TenantId && n.RecipientUserId == s.AdminStudentId
            && n.Type == NotificationType.GroupAdminRequestRejected && n.RelatedId == removalId)));
        Assert.True(await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters().AnyAsync(e =>
            e.TenantId == s.Teacher.TenantId && e.StudentId == s.AdminStudentId && e.GroupId == s.Teacher.GroupId
            && e.IsGroupAdmin)));
    }

    [Fact]
    public async Task GroupAdminLimit_AllowsTwoAndRejectsTheThirdAppointment()
    {
        var s = await SeedAsync();
        Guid secondStudent = default, thirdStudent = default;
        await _factory.SeedAsync(async db =>
        {
            secondStudent = await TestData.AddStudentAsync(db, s.Teacher.TenantId, s.Teacher.GroupId);
            thirdStudent = await TestData.AddStudentAsync(db, s.Teacher.TenantId, s.Teacher.GroupId);
        });

        var teacher = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);
        Assert.Equal(HttpStatusCode.OK, (await teacher.PutAsync(
            $"/api/Groups/{s.Teacher.GroupId}/admins/{s.AdminStudentId}", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await teacher.PutAsync(
            $"/api/Groups/{s.Teacher.GroupId}/admins/{secondStudent}", null)).StatusCode);

        var third = await teacher.PutAsync($"/api/Groups/{s.Teacher.GroupId}/admins/{thirdStudent}", null);

        Assert.Equal(HttpStatusCode.Conflict, third.StatusCode);
        Assert.Equal("GroupAdmin.LimitReached", await ErrorCodeAsync(third));
        Assert.Equal(2, await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters()
            .CountAsync(e => e.TenantId == s.Teacher.TenantId && e.GroupId == s.Teacher.GroupId && e.IsGroupAdmin)));
    }

    [Theory]
    [InlineData(false, "PendingCompensation", AttendanceStatus.PendingCompensation)]
    [InlineData(true, "Recorded", AttendanceStatus.Present)]
    public async Task GroupScan_CrossGroupCompensationFollowsAutoApprovalSetting(
        bool autoApprove, string expectedOutcome, AttendanceStatus expectedStatus)
    {
        SeededTeacher teacher = null!;
        Guid adminStudentId = default, targetStudentId = default, compensableSessionId = default;
        var now = DateTime.UtcNow;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var otherGroupId = await TestData.AddGroupAsync(db, teacher);
            adminStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            targetStudentId = await TestData.AddStudentAsync(db, teacher.TenantId);
            db.GroupEnrollments.Add(GroupEnrollment.Create(
                teacher.TenantId, otherGroupId, targetStudentId, now.AddDays(-30), null));
            await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId,
                now.AddMinutes(-5), TimeSpan.FromMinutes(45));
            compensableSessionId = await TestData.AddSessionAsync(db, teacher.TenantId, otherGroupId,
                now.AddDays(-1), TimeSpan.FromHours(1));

            var settings = await db.TenantSettings.IgnoreQueryFilters().SingleAsync(s => s.TenantId == teacher.TenantId);
            settings.UpdateAttendance(AttendanceMode.StaticGlobalQR, rejectMockLocations: false,
                autoRegisterOnScan: true, autoApproveGroupAdminScans: autoApprove);
            await db.SaveChangesAsync();
        });

        var owner = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        Assert.Equal(HttpStatusCode.OK, (await owner.PutAsync(
            $"/api/Groups/{teacher.GroupId}/admins/{adminStudentId}", null)).StatusCode);
        var admin = _factory.CreateClientFor(adminStudentId, teacher.TenantId, "Student");
        var response = await admin.PostAsJsonAsync("/api/Attendance/group-scan", new
        {
            studentQrCode = await GetQrAsync(targetStudentId),
            latitude = 30.0561,
            longitude = 31.3301,
            isMockLocation = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(expectedOutcome, body.GetProperty("outcome").GetString());
        var record = await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .SingleAsync(a => a.TenantId == teacher.TenantId && a.StudentId == targetStudentId
                              && a.SessionId != compensableSessionId));
        Assert.Equal(expectedStatus, record.Status);
        Assert.Equal(compensableSessionId, record.CompensatesSessionId);
        Assert.Equal(adminStudentId, record.RecordedByUserId);
    }

    [Theory]
    [InlineData(false, "PendingApproval")]
    [InlineData(true, "Recorded")]
    public async Task GroupScan_WalkInUsesGroupAdminAutoApprovalIndependentlyOfSelfScanSetting(
        bool autoApprove, string expectedOutcome)
    {
        SeededTeacher teacher = null!;
        Guid adminStudentId = default, walkInStudentId = default, sessionId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            adminStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            walkInStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, withMembership: false);
            sessionId = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId,
                DateTime.UtcNow.AddMinutes(-5), TimeSpan.FromMinutes(45));
            var settings = await db.TenantSettings.IgnoreQueryFilters().SingleAsync(s => s.TenantId == teacher.TenantId);
            settings.UpdateAttendance(AttendanceMode.StaticGlobalQR, rejectMockLocations: false,
                autoRegisterOnScan: false, autoApproveGroupAdminScans: autoApprove);
            await db.SaveChangesAsync();
        });

        var owner = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        Assert.Equal(HttpStatusCode.OK, (await owner.PutAsync(
            $"/api/Groups/{teacher.GroupId}/admins/{adminStudentId}", null)).StatusCode);
        var admin = _factory.CreateClientFor(adminStudentId, teacher.TenantId, "Student");
        var response = await admin.PostAsJsonAsync("/api/Attendance/group-scan", new
        {
            studentQrCode = await GetQrAsync(walkInStudentId),
            latitude = 30.0561,
            longitude = 31.3301,
            isMockLocation = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(expectedOutcome, body.GetProperty("outcome").GetString());
        Assert.Equal(autoApprove, await _factory.QueryAsync(db => db.TenantMembers.IgnoreQueryFilters()
            .AnyAsync(m => m.TenantId == teacher.TenantId && m.UserId == walkInStudentId)));
        Assert.Equal(autoApprove, await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.TenantId == teacher.TenantId && a.SessionId == sessionId
                           && a.StudentId == walkInStudentId && a.Status == AttendanceStatus.Present)));
        Assert.Equal(!autoApprove, await _factory.QueryAsync(db => db.EnrollmentRequests.IgnoreQueryFilters()
            .AnyAsync(r => r.TenantId == teacher.TenantId && r.StudentId == walkInStudentId
                           && r.Status == EnrollmentRequestStatus.Pending)));
    }
}
