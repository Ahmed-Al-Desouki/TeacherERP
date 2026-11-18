using System.Net;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class CompensationStaffIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CompensationStaffIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Assistant_CanReviewCompensations_AndDecisionNotifiesTeacherAndWritesAudit()
    {
        SeededTeacher teacher = null!;
        Guid assistantId = default, studentId = default, approvedId = default, rejectedId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var assistant = ApplicationUser.CreateAdmin("Assistant", "assistant-review@test.local", "+201000000099");
            db.Users.Add(assistant);
            assistantId = assistant.Id;
            db.TenantMembers.Add(TenantMember.Create(teacher.TenantId, assistantId, TenantRole.Assistant));
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var groupId = await TestData.AddGroupAsync(db, teacher);
            var firstSession = await TestData.AddSessionAsync(db, teacher.TenantId, groupId, DateTime.UtcNow.AddDays(-2));
            var secondSession = await TestData.AddSessionAsync(db, teacher.TenantId, groupId, DateTime.UtcNow.AddDays(-1));
            approvedId = await TestData.AddPendingCompensationAsync(db, teacher.TenantId, firstSession, studentId, null);
            rejectedId = await TestData.AddPendingCompensationAsync(db, teacher.TenantId, secondSession, studentId, null);
        });

        var assistantClient = _factory.CreateClientFor(assistantId, teacher.TenantId, "SuperAdmin");
        var teacherClient = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var studentClient = _factory.CreateClientFor(studentId, teacher.TenantId, "Student");
        var pending = await assistantClient.GetAsync("/api/Attendance/compensations/pending");
        var teacherPending = await teacherClient.GetAsync("/api/Attendance/compensations/pending");
        var studentPending = await studentClient.GetAsync("/api/Attendance/compensations/pending");
        var sessionAttendance = await assistantClient.GetAsync($"/api/Attendance/sessions/{Guid.NewGuid()}");
        var approve = await assistantClient.PostAsync($"/api/Attendance/compensations/{approvedId}/approve", null);
        var reject = await assistantClient.PostAsync($"/api/Attendance/compensations/{rejectedId}/reject", null);

        Assert.Equal(HttpStatusCode.OK, pending.StatusCode);
        Assert.Equal(HttpStatusCode.OK, teacherPending.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, studentPending.StatusCode);
        Assert.Equal(HttpStatusCode.OK, sessionAttendance.StatusCode);
        Assert.Equal(HttpStatusCode.OK, approve.StatusCode);
        Assert.Equal(HttpStatusCode.OK, reject.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters().AnyAsync(log =>
            log.TenantId == teacher.TenantId && log.Action == AuditAction.CompensationApproved
            && log.ActorUserId == assistantId && log.TargetUserId == studentId && log.Reason!.Contains(approvedId.ToString()))));
        Assert.True(await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters().AnyAsync(log =>
            log.TenantId == teacher.TenantId && log.Action == AuditAction.CompensationRejected
            && log.ActorUserId == assistantId && log.TargetUserId == studentId && log.Reason!.Contains(rejectedId.ToString()))));
        Assert.Equal(2, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(n =>
            n.TenantId == teacher.TenantId && n.RecipientUserId == teacher.OwnerId
            && (n.Type == NotificationType.CompensationApproved || n.Type == NotificationType.CompensationRejected))));
        Assert.False(await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().AnyAsync(n =>
            n.TenantId == teacher.TenantId && n.RecipientUserId == assistantId
            && (n.Type == NotificationType.CompensationApproved || n.Type == NotificationType.CompensationRejected))));
    }
}
