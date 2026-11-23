using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Integration;

public sealed class SessionMoveIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SessionMoveIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task MoveGeneratedOccurrence_PreservesIdentityAttachmentsAndRoster_AndSourceRecursNextWeek()
    {
        SeededTeacher teacher = null!;
        Guid sessionId = default, targetGroupId = default, sourceStudentId = default, targetStudentId = default;
        var originalDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(4);
        var originalStart = originalDate.ToDateTime(new TimeOnly(17, 0), DateTimeKind.Utc);
        var newStart = originalStart.AddDays(1).AddHours(2);

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            sourceStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            targetGroupId = await TestData.AddGroupAsync(db, teacher);
            targetStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, targetGroupId);
            var sourceGroup = await db.Groups.IgnoreQueryFilters().Include(g => g.Slots)
                .SingleAsync(g => g.Id == teacher.GroupId);
            var slot = sourceGroup.AddSlot(originalDate.DayOfWeek, originalStart.TimeOfDay, 60);
            db.GroupScheduleSlots.Add(slot);
            var session = Session.Create(teacher.TenantId, teacher.GroupId, originalStart, originalStart.AddHours(1), slot.Id);
            db.Sessions.Add(session);
            await db.SaveChangesAsync();
            sessionId = session.Id;
            db.SessionAttachments.Add(SessionAttachment.Create(teacher.TenantId, session.Id,
                SessionAttachmentKind.Explanation, "https://files.test/session.pdf", "test/session", "session.pdf",
                "application/pdf", 100, null, teacher.OwnerId));
            await db.SaveChangesAsync();
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"/api/Schedule/sessions/{sessionId}/move", new
            {
                targetGroupId,
                newStartTime = newStart,
                durationMinutes = 90
            });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var moved = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId)
            .Select(s => new { s.Id, s.GroupId, s.StartTime, s.EndTime, s.OriginalStartTime, s.SlotId })
            .SingleAsync());
        Assert.Equal(sessionId, moved.Id);
        Assert.Equal(targetGroupId, moved.GroupId);
        Assert.Equal(newStart, moved.StartTime);
        Assert.Equal(newStart.AddMinutes(90), moved.EndTime);
        Assert.Equal(originalStart, moved.OriginalStartTime);
        Assert.NotNull(moved.SlotId);
        Assert.True(await _factory.QueryAsync(db => db.SessionAttachments.IgnoreQueryFilters()
            .AnyAsync(a => a.SessionId == sessionId && a.FileName == "session.pdf")));
        Assert.True(await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters()
            .AnyAsync(e => e.TenantId == teacher.TenantId && e.GroupId == teacher.GroupId && e.StudentId == sourceStudentId)));
        Assert.True(await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters()
            .AnyAsync(e => e.TenantId == teacher.TenantId && e.GroupId == targetGroupId && e.StudentId == targetStudentId)));
        Assert.Equal(1, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(notification =>
            notification.TenantId == teacher.TenantId && notification.RecipientUserId == sourceStudentId
            && notification.Type == NotificationType.SessionMoved && notification.RelatedId == sessionId)));
        Assert.Equal(1, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(notification =>
            notification.TenantId == teacher.TenantId && notification.RecipientUserId == targetStudentId
            && notification.Type == NotificationType.SessionMoved && notification.RelatedId == sessionId)));

        using var scope = _factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>()
            .Set(new TenantContext(teacher.TenantId, TenantRole.Owner, MembershipStatus.Active));
        var generated = await scope.ServiceProvider.GetRequiredService<ISessionGenerator>()
            .GenerateForGroupAsync(teacher.GroupId, CancellationToken.None);
        Assert.True(generated > 0);

        var sourceStarts = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.TenantId == teacher.TenantId && s.GroupId == teacher.GroupId)
            .Select(s => s.StartTime).ToListAsync());
        Assert.DoesNotContain(originalStart, sourceStarts);
        Assert.Contains(originalStart.AddDays(7), sourceStarts);
    }

    [Fact]
    public async Task MoveExtraSession_PreservesTheSessionAndKeepsItOutsideTheWeeklyTemplate()
    {
        SeededTeacher teacher = null!;
        Guid sessionId = default, targetGroupId = default;
        var start = DateTime.UtcNow.Date.AddDays(7).AddHours(15);
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            targetGroupId = await TestData.AddGroupAsync(db, teacher);
            var session = Session.CreateExtra(teacher.TenantId, teacher.GroupId, start, start.AddMinutes(75));
            db.Sessions.Add(session);
            await db.SaveChangesAsync();
            sessionId = session.Id;
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"/api/Schedule/sessions/{sessionId}/move", new
            {
                targetGroupId,
                newStartTime = start.AddDays(2),
                durationMinutes = (int?)null
            });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var moved = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId)
            .Select(s => new { s.GroupId, s.StartTime, s.EndTime, s.SlotId, s.OriginalStartTime })
            .SingleAsync());
        Assert.Equal(targetGroupId, moved.GroupId);
        Assert.Equal(start.AddDays(2), moved.StartTime);
        Assert.Equal(start.AddDays(2).AddMinutes(75), moved.EndTime);
        Assert.Null(moved.SlotId);
        Assert.Null(moved.OriginalStartTime);
    }

    [Fact]
    public async Task Move_RejectsHardConflictWithAnotherGroupOfSameTeacher()
    {
        SeededTeacher teacher = null!;
        Guid sessionId = default, targetGroupId = default;
        var start = DateTime.UtcNow.Date.AddDays(10).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            targetGroupId = await TestData.AddGroupAsync(db, teacher);
            var peerGroupId = await TestData.AddGroupAsync(db, teacher);
            sessionId = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, start);
            await TestData.AddSessionAsync(db, teacher.TenantId, peerGroupId, start.AddHours(2));
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"/api/Schedule/sessions/{sessionId}/move", new
            {
                targetGroupId,
                newStartTime = start.AddHours(2),
                durationMinutes = 60
            });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.Equal("Session.TimeConflict", body.GetProperty("errorCode").GetString());
        Assert.Equal(teacher.GroupId, await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId).Select(s => s.GroupId).SingleAsync()));
    }

    [Fact]
    public async Task Move_SharedRoomConflictReturnsDetailsAndAllowsExplicitConfirmation()
    {
        SeededTeacher owner = null!, peer = null!;
        Guid sessionId = default, targetGroupId = default;
        var start = DateTime.UtcNow.Date.AddDays(11).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            owner = await TestData.CreateTeacherAsync(db);
            peer = await TestData.CreateTeacherAsync(db);
            var target = Group.Create(owner.TenantId, "Move target", new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, "Room A", null, null);
            var peerGroup = Group.Create(peer.TenantId, "Other teacher room", new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, " room a ", null, null);
            db.Groups.AddRange(target, peerGroup);
            await db.SaveChangesAsync();
            targetGroupId = target.Id;
            sessionId = await TestData.AddSessionAsync(db, owner.TenantId, owner.GroupId, start);
            await TestData.AddSessionAsync(db, peer.TenantId, peerGroup.Id, start.AddHours(2));
        });

        var client = _factory.CreateClientFor(owner.OwnerId, owner.TenantId);
        var warning = await client.PostAsJsonAsync($"/api/Schedule/sessions/{sessionId}/move", new
        {
            targetGroupId,
            newStartTime = start.AddHours(2),
            durationMinutes = 60
        });
        Assert.Equal(HttpStatusCode.Conflict, warning.StatusCode);
        var warningBody = await warning.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.Equal("Schedule.RoomConflictConfirmationRequired", warningBody.GetProperty("errorCode").GetString());
        Assert.Contains("Other teacher room", warningBody.GetRawText());

        var confirmed = await client.PostAsJsonAsync($"/api/Schedule/sessions/{sessionId}/move", new
        {
            targetGroupId,
            newStartTime = start.AddHours(2),
            durationMinutes = 60,
            confirmRoomConflicts = true
        });
        Assert.Equal(HttpStatusCode.OK, confirmed.StatusCode);
        Assert.Equal(targetGroupId, await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId).Select(s => s.GroupId).SingleAsync()));
    }

    [Theory]
    [InlineData("attendance", "Session.MoveBlockedByAttendance")]
    [InlineData("compensation", "Session.MoveBlockedByCompensation")]
    [InlineData("request", "Session.MoveBlockedByRequest")]
    public async Task Move_RejectsSessionWithHistoricalOrPendingReferences(string blocker, string expectedCode)
    {
        SeededTeacher teacher = null!;
        Guid sessionId = default, targetGroupId = default;
        var start = DateTime.UtcNow.Date.AddDays(6).AddHours(14);
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            targetGroupId = await TestData.AddGroupAsync(db, teacher);
            sessionId = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, start);
            var studentId = await TestData.AddStudentAsync(db, teacher.TenantId, withMembership: false);
            if (blocker == "attendance")
            {
                db.AttendanceRecords.Add(AttendanceRecord.RecordPresence(teacher.TenantId, sessionId, studentId,
                    ScanMethod.ManualEntry, null, null, teacher.OwnerId));
            }
            else if (blocker == "compensation")
            {
                var anotherSession = Session.CreateExtra(teacher.TenantId, targetGroupId, start.AddDays(1), start.AddDays(1).AddHours(1));
                db.Sessions.Add(anotherSession);
                db.AttendanceRecords.Add(AttendanceRecord.RecordPresence(teacher.TenantId, anotherSession.Id, studentId,
                    ScanMethod.SelfScan, null, null, null, AttendanceStatus.PendingCompensation, sessionId));
            }
            else
            {
                db.EnrollmentRequests.Add(EnrollmentRequest.Create(teacher.TenantId, studentId, targetGroupId,
                    EnrollmentRequestType.Join, sessionId, ScanMethod.SelfScan));
            }
            await db.SaveChangesAsync();
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"/api/Schedule/sessions/{sessionId}/move", new
            {
                targetGroupId,
                newStartTime = start.AddDays(2),
                durationMinutes = 60
            });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.Equal(expectedCode, body.GetProperty("errorCode").GetString());
        Assert.Equal(teacher.GroupId, await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId).Select(s => s.GroupId).SingleAsync()));
    }
}
