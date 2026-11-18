using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class ManualAttendanceIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ManualAttendanceIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    private static async Task<string?> ErrorCodeAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.TryGetProperty("errorCode", out var code) ? code.GetString() : null;
    }

    [Fact]
    public async Task ManualAttendance_RecordsCurrentSessionForAnEnrolledStudent()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default, sessionId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            sessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddMinutes(-5), TimeSpan.FromMinutes(45));
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync("/api/Attendance/manual", new { studentId, sessionId, reason = (string?)null });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Recorded", body.GetProperty("outcome").GetString());
        Assert.True(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters().AnyAsync(a =>
            a.TenantId == teacher.TenantId && a.SessionId == sessionId && a.StudentId == studentId
            && a.Status == AttendanceStatus.Present && a.ScannedVia == ScanMethod.ManualEntry
            && a.RecordedByUserId == teacher.OwnerId)));
    }

    [Fact]
    public async Task ManualCorrection_RequiresReasonAndConvertsAnAbsenceToPresent()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default, sessionId = default, absenceId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, withMembership: true);
            var now = DateTime.UtcNow;
            db.GroupEnrollments.Add(GroupEnrollment.Create(
                teacher.TenantId, teacher.GroupId, studentId, now.AddDays(-10), null));
            sessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, now.AddHours(-2), TimeSpan.FromMinutes(60));
            var absence = AttendanceRecord.RecordAbsence(teacher.TenantId, sessionId, studentId);
            db.AttendanceRecords.Add(absence);
            await db.SaveChangesAsync();
            absenceId = absence.Id;
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var missingReason = await client.PostAsJsonAsync("/api/Attendance/manual", new { studentId, sessionId, reason = " " });
        Assert.Equal(HttpStatusCode.BadRequest, missingReason.StatusCode);
        Assert.Equal("Attendance.ReasonRequired", await ErrorCodeAsync(missingReason));
        Assert.Equal(AttendanceStatus.Absent, await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .Where(a => a.Id == absenceId).Select(a => a.Status).SingleAsync()));

        var corrected = await client.PostAsJsonAsync("/api/Attendance/manual", new
        {
            studentId,
            sessionId,
            reason = "Teacher verified attendance"
        });

        Assert.Equal(HttpStatusCode.OK, corrected.StatusCode);
        var body = await corrected.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Corrected", body.GetProperty("outcome").GetString());
        var record = await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .SingleAsync(a => a.Id == absenceId));
        Assert.Equal(AttendanceStatus.Present, record.Status);
        Assert.Equal(ScanMethod.ManualEntry, record.ScannedVia);
        Assert.True(await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters().AnyAsync(log =>
            log.TenantId == teacher.TenantId && log.Action == AuditAction.AttendanceCorrected
            && log.TargetUserId == studentId && log.ActorUserId == teacher.OwnerId
            && log.Reason != null && log.Reason.Contains("Teacher verified attendance"))));
    }
}
