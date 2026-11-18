using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Integration;

public sealed class AttendanceFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private const double CenterLatitude = 30.0561;
    private const double CenterLongitude = 31.3301;
    private readonly CustomWebApplicationFactory _factory;

    public AttendanceFlowIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    private sealed record ScanScenario(SeededTeacher Teacher, Guid StudentId, Guid? SessionId, string Code);

    private async Task<ScanScenario> SeedScanAsync(
        bool withSession = true,
        bool withMembership = true,
        bool autoRegister = true,
        MembershipStatus? memberStatus = null)
    {
        ScanScenario? scenario = null;
        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var studentId = await TestData.AddStudentAsync(
                db, teacher.TenantId, withMembership ? teacher.GroupId : null, withMembership);
            var student = await db.Users.IgnoreQueryFilters().SingleAsync(u => u.Id == studentId);
            student.CompleteProfile(student.PhoneNumber!,
                GuardianInfo.Create("Guardian", "+201155500001", GuardianRelation.Father), null, null);

            if (memberStatus is { } status)
            {
                var member = await db.TenantMembers.IgnoreQueryFilters()
                    .SingleAsync(m => m.TenantId == teacher.TenantId && m.UserId == studentId);
                if (status == MembershipStatus.Blacklisted) member.Blacklist();
                if (status == MembershipStatus.Suspended) member.Suspend();
            }

            Guid? sessionId = null;
            if (withSession)
            {
                sessionId = await TestData.AddSessionAsync(
                    db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddMinutes(-5), TimeSpan.FromMinutes(45));
            }

            var settings = await db.TenantSettings.IgnoreQueryFilters().SingleAsync(s => s.TenantId == teacher.TenantId);
            settings.UpdateAttendance(AttendanceMode.StaticGlobalQR, rejectMockLocations: true,
                autoRegisterOnScan: autoRegister, autoApproveGroupAdminScans: false);
            await db.SaveChangesAsync();
            scenario = new ScanScenario(teacher, studentId, sessionId, settings.QrSecret);
        });
        return scenario!;
    }

    private HttpClient StudentClient(ScanScenario s)
        => _factory.CreateClientFor(s.StudentId, s.Teacher.TenantId, "Student");

    private static object ScanBody(ScanScenario s, string? code = null, double latitude = CenterLatitude,
        double longitude = CenterLongitude, bool mock = false, double? accuracyMeters = 5)
        => new
        {
            teacherId = s.Teacher.TenantId,
            code = code ?? s.Code,
            latitude,
            longitude,
            accuracyMeters,
            isMockLocation = mock,
            groupId = s.Teacher.GroupId
        };

    private static async Task<string?> ErrorCodeAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.TryGetProperty("errorCode", out var code) ? code.GetString() : null;
    }

    [Fact]
    public async Task SelfScan_RejectsWrongCodeMockLocationAndOutOfRangeLocation()
    {
        var s = await SeedScanAsync();
        var client = StudentClient(s);

        var invalidAccuracy = await client.PostAsJsonAsync("/api/Attendance/scan", new
        {
            teacherId = s.Teacher.TenantId,
            code = s.Code,
            latitude = CenterLatitude,
            longitude = CenterLongitude,
            accuracyMeters = -1,
            isMockLocation = false,
            groupId = s.Teacher.GroupId
        });
        var wrongCode = await client.PostAsJsonAsync("/api/Attendance/scan", ScanBody(s, code: "WRONG"));
        var mock = await client.PostAsJsonAsync("/api/Attendance/scan", ScanBody(s, mock: true));
        var outside = await client.PostAsJsonAsync("/api/Attendance/scan", ScanBody(s, latitude: 30.0444, longitude: 31.2357));

        Assert.Equal(HttpStatusCode.BadRequest, invalidAccuracy.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, wrongCode.StatusCode);
        Assert.Equal("Attendance.InvalidCode", await ErrorCodeAsync(wrongCode));
        Assert.Equal(HttpStatusCode.BadRequest, mock.StatusCode);
        Assert.Equal("Attendance.MockLocation", await ErrorCodeAsync(mock));
        Assert.Equal(HttpStatusCode.BadRequest, outside.StatusCode);
        Assert.Equal("Attendance.OutOfRange", await ErrorCodeAsync(outside));
        Assert.False(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.StudentId == s.StudentId)));
    }

    [Fact]
    public async Task SelfScan_RequiresAccurateLocationAndAcceptsTheMaximumAllowedAccuracy()
    {
        var rejected = await SeedScanAsync();
        var client = StudentClient(rejected);

        var missingAccuracy = await client.PostAsJsonAsync(
            "/api/Attendance/scan", ScanBody(rejected, accuracyMeters: null));
        var poorAccuracy = await client.PostAsJsonAsync(
            "/api/Attendance/scan", ScanBody(rejected, accuracyMeters: 100.01));

        Assert.Equal(HttpStatusCode.BadRequest, missingAccuracy.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, poorAccuracy.StatusCode);
        Assert.False(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.StudentId == rejected.StudentId)));

        var boundary = await SeedScanAsync();
        var accepted = await StudentClient(boundary).PostAsJsonAsync(
            "/api/Attendance/scan", ScanBody(boundary, accuracyMeters: 100));

        Assert.Equal(HttpStatusCode.OK, accepted.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.StudentId == boundary.StudentId && a.Status == AttendanceStatus.Present)));
    }

    [Fact]
    public async Task SelfScan_RejectsBlacklistedAndSuspendedMemberships()
    {
        var blacklisted = await SeedScanAsync(memberStatus: MembershipStatus.Blacklisted);
        var suspended = await SeedScanAsync(memberStatus: MembershipStatus.Suspended);

        var blocked = await StudentClient(blacklisted).PostAsJsonAsync("/api/Attendance/scan", ScanBody(blacklisted));
        var stopped = await StudentClient(suspended).PostAsJsonAsync("/api/Attendance/scan", ScanBody(suspended));

        Assert.Equal("Enrollment.NotAllowed", await ErrorCodeAsync(blocked));
        Assert.Equal("Attendance.Suspended", await ErrorCodeAsync(stopped));
        Assert.False(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.StudentId == blacklisted.StudentId || a.StudentId == suspended.StudentId)));
    }

    [Fact]
    public async Task SelfScan_RequiresAValidCodeAndRunningSession()
    {
        var s = await SeedScanAsync(withSession: false);
        var response = await StudentClient(s).PostAsJsonAsync("/api/Attendance/scan", ScanBody(s));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Attendance.NoSessionNow", await ErrorCodeAsync(response));
    }

    [Fact]
    public async Task SelfScan_IsDisabledWhenTeacherRequiresStaffAttendance()
    {
        var s = await SeedScanAsync();
        await _factory.SeedAsync(async db =>
        {
            var settings = await db.TenantSettings.IgnoreQueryFilters().SingleAsync(x => x.TenantId == s.Teacher.TenantId);
            settings.UpdateAttendance(AttendanceMode.AdminOnly, rejectMockLocations: true,
                autoRegisterOnScan: true, autoApproveGroupAdminScans: false);
            await db.SaveChangesAsync();
        });

        var response = await StudentClient(s).PostAsJsonAsync("/api/Attendance/scan", ScanBody(s));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Attendance.SelfScanDisabled", await ErrorCodeAsync(response));
    }

    [Fact]
    public async Task SelfScan_WhenAutoRegistrationIsDisabled_CreatesPendingRequestWithoutEnrollmentOrAttendance()
    {
        var s = await SeedScanAsync(withMembership: false, autoRegister: false);
        var response = await StudentClient(s).PostAsJsonAsync("/api/Attendance/scan", ScanBody(s));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("PendingApproval", body.GetProperty("outcome").GetString());
        Assert.True(await _factory.QueryAsync(db => db.EnrollmentRequests.IgnoreQueryFilters()
            .AnyAsync(r => r.TenantId == s.Teacher.TenantId && r.StudentId == s.StudentId
                           && r.Status == EnrollmentRequestStatus.Pending)));
        Assert.False(await _factory.QueryAsync(db => db.TenantMembers.IgnoreQueryFilters()
            .AnyAsync(m => m.TenantId == s.Teacher.TenantId && m.UserId == s.StudentId)));
        Assert.False(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.SessionId == s.SessionId && a.StudentId == s.StudentId)));
    }

    [Fact]
    public async Task SelfScan_WhenAutoRegistrationIsEnabled_EnrollsAndRecordsAttendance()
    {
        var s = await SeedScanAsync(withMembership: false);
        var response = await StudentClient(s).PostAsJsonAsync("/api/Attendance/scan", ScanBody(s));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Recorded", body.GetProperty("outcome").GetString());
        Assert.True(await _factory.QueryAsync(db => db.TenantMembers.IgnoreQueryFilters()
            .AnyAsync(m => m.TenantId == s.Teacher.TenantId && m.UserId == s.StudentId)));
        Assert.True(await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters()
            .AnyAsync(e => e.TenantId == s.Teacher.TenantId && e.StudentId == s.StudentId
                           && e.GroupId == s.Teacher.GroupId && e.Status == EnrollmentStatus.Active)));
        Assert.True(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.SessionId == s.SessionId && a.StudentId == s.StudentId
                           && a.Status == AttendanceStatus.Present && a.ScannedVia == ScanMethod.SelfScan)));
    }

    [Fact]
    public async Task ApprovingCompensation_ResetsAbsencesAndReactivatesAbsenceSuspendedMember()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default, compensationRecordId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var member = await db.TenantMembers.IgnoreQueryFilters()
                .SingleAsync(m => m.TenantId == teacher.TenantId && m.UserId == studentId);
            member.SetConsecutiveAbsences(4);
            member.Suspend();

            var otherGroupId = await TestData.AddGroupAsync(db, teacher);
            var original = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddDays(-2));
            var attended = await TestData.AddSessionAsync(db, teacher.TenantId, otherGroupId, DateTime.UtcNow.AddDays(-1));
            compensationRecordId = await TestData.AddPendingCompensationAsync(
                db, teacher.TenantId, attended, studentId, original);
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsync($"/api/Attendance/compensations/{compensationRecordId}/approve", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(AttendanceStatus.Present, await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .Where(a => a.Id == compensationRecordId).Select(a => a.Status).SingleAsync()));
        var memberAfter = await _factory.QueryAsync(db => db.TenantMembers.IgnoreQueryFilters()
            .SingleAsync(m => m.TenantId == teacher.TenantId && m.UserId == studentId));
        Assert.Equal(MembershipStatus.Active, memberAfter.Status);
        Assert.Equal(0, memberAfter.ConsecutiveAbsences);
        Assert.True(await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters().AnyAsync(log =>
            log.TenantId == teacher.TenantId && log.Action == AuditAction.StudentReinstatedByCompensation
            && log.TargetUserId == studentId && log.ActorUserId == teacher.OwnerId)));
    }

    [Fact]
    public async Task RejectingPendingCompensationAfterAbsenceJob_RestoresOriginalAbsence()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default, originalSessionId = default, compensationRecordId = default;
        var now = DateTime.UtcNow;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId);
            db.GroupEnrollments.Add(GroupEnrollment.Create(
                teacher.TenantId, teacher.GroupId, studentId, now.AddDays(-30), null));
            var otherGroupId = await TestData.AddGroupAsync(db, teacher);
            originalSessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, now.AddHours(-4), TimeSpan.FromHours(1));
            var compensationSessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, otherGroupId, now.AddHours(-2), TimeSpan.FromHours(1));
            var compensation = AttendanceRecord.RecordPresence(
                teacher.TenantId, compensationSessionId, studentId, ScanMethod.AdminScan,
                null, null, teacher.OwnerId, AttendanceStatus.PendingCompensation, originalSessionId);
            db.AttendanceRecords.Add(compensation);
            await db.SaveChangesAsync();
            compensationRecordId = compensation.Id;
        });

        using (var scope = _factory.Services.CreateScope())
        {
            var currentTenant = scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>();
            currentTenant.Set(new TenantContext(teacher.TenantId, TenantRole.Owner, MembershipStatus.Active));
            var processor = scope.ServiceProvider.GetRequiredService<IAbsenceProcessor>();
            Assert.Equal(0, await processor.ProcessAsync(CancellationToken.None));
        }

        Assert.False(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.SessionId == originalSessionId && a.StudentId == studentId
                           && a.Status == AttendanceStatus.Absent)));
        Assert.NotNull(await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == originalSessionId).Select(s => s.AbsencesProcessedAt).SingleAsync()));

        var rejected = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsync($"/api/Attendance/compensations/{compensationRecordId}/reject", null);

        Assert.Equal(HttpStatusCode.OK, rejected.StatusCode);
        Assert.Equal(AttendanceStatus.Rejected, await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .Where(a => a.Id == compensationRecordId).Select(a => a.Status).SingleAsync()));
        Assert.True(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(a => a.SessionId == originalSessionId && a.StudentId == studentId
                           && a.Status == AttendanceStatus.Absent)));
        Assert.Equal(1, await _factory.QueryAsync(db => db.TenantMembers.IgnoreQueryFilters()
            .Where(m => m.TenantId == teacher.TenantId && m.UserId == studentId)
            .Select(m => m.ConsecutiveAbsences).SingleAsync()));
    }

    [Fact]
    public async Task AttendanceReadsAndManualEntry_AreIsolatedToTheSelectedTeacher()
    {
        SeededTeacher first = null!, second = null!;
        Guid firstStudent = default, foreignSession = default;
        await _factory.SeedAsync(async db =>
        {
            first = await TestData.CreateTeacherAsync(db);
            firstStudent = await TestData.AddStudentAsync(db, first.TenantId, first.GroupId);
            second = await TestData.CreateTeacherAsync(db);
            foreignSession = await TestData.AddSessionAsync(
                db, second.TenantId, second.GroupId, DateTime.UtcNow.AddMinutes(-2), TimeSpan.FromMinutes(40));
        });

        var firstOwner = _factory.CreateClientFor(first.OwnerId, first.TenantId);
        var attendance = await firstOwner.GetAsync($"/api/Attendance/sessions/{foreignSession}");
        var from = Uri.EscapeDataString(DateTime.UtcNow.AddDays(-1).ToString("O"));
        var to = Uri.EscapeDataString(DateTime.UtcNow.AddDays(1).ToString("O"));
        var schedule = await firstOwner.GetAsync($"/api/Schedule/sessions?from={from}&to={to}");
        var manual = await firstOwner.PostAsJsonAsync("/api/Attendance/manual", new
        {
            studentId = firstStudent,
            sessionId = foreignSession,
            reason = (string?)null
        });

        Assert.Equal(HttpStatusCode.OK, attendance.StatusCode);
        var rows = await attendance.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0, rows.GetProperty("totalCount").GetInt32());
        Assert.Equal(HttpStatusCode.OK, schedule.StatusCode);
        Assert.DoesNotContain(foreignSession.ToString(), await schedule.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.BadRequest, manual.StatusCode);
        Assert.Equal("Session.NotFound", await ErrorCodeAsync(manual));
    }
}
