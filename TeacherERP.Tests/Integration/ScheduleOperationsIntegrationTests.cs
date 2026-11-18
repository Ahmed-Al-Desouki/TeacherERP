using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Integration;

public sealed class ScheduleOperationsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ScheduleOperationsIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    private static async Task<string?> ErrorCodeAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.TryGetProperty("errorCode", out var code) ? code.GetString() : null;
    }

    [Fact]
    public async Task Reschedule_RejectsConflictThenCancelAndRestoreKeepTheSessionConsistent()
    {
        SeededTeacher teacher = null!;
        Guid sessionId = default, blockerId = default;
        var baseStart = DateTime.UtcNow.Date.AddDays(3).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            sessionId = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, baseStart);
            blockerId = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, baseStart.AddHours(2));
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var conflict = await client.PutAsJsonAsync($"/api/Schedule/sessions/{sessionId}/reschedule", new
        {
            newStartTime = baseStart.AddHours(2),
            durationMinutes = 60
        });
        Assert.Equal(HttpStatusCode.BadRequest, conflict.StatusCode);
        Assert.Equal("Session.TimeConflict", await ErrorCodeAsync(conflict));
        Assert.Equal(baseStart, await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId).Select(s => s.StartTime).SingleAsync()));

        var newStart = baseStart.AddHours(4);
        var rescheduled = await client.PutAsJsonAsync($"/api/Schedule/sessions/{sessionId}/reschedule", new
        {
            newStartTime = newStart,
            durationMinutes = 75
        });
        Assert.Equal(HttpStatusCode.OK, rescheduled.StatusCode);
        var times = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters().Where(s => s.Id == sessionId)
            .Select(s => new { s.StartTime, s.EndTime, s.IsModified }).SingleAsync());
        Assert.Equal(newStart, times.StartTime);
        Assert.Equal(newStart.AddMinutes(75), times.EndTime);
        Assert.True(times.IsModified);

        var cancelled = await client.PostAsJsonAsync($"/api/Schedule/sessions/{sessionId}/cancel", new { reason = "change" });
        Assert.Equal(HttpStatusCode.OK, cancelled.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId).Select(s => s.IsCancelled).SingleAsync()));

        var restored = await client.PostAsync($"/api/Schedule/sessions/{sessionId}/restore", null);
        Assert.Equal(HttpStatusCode.OK, restored.StatusCode);
        Assert.False(await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId).Select(s => s.IsCancelled).SingleAsync()));
        Assert.False(await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == blockerId).Select(s => s.IsCancelled).SingleAsync()));
    }

    [Fact]
    public async Task Reschedule_ChangesDurationWithoutChangingStartTime_AndRejectsDurationOverlap()
    {
        SeededTeacher teacher = null!;
        Guid sessionId = default;
        var start = DateTime.UtcNow.Date.AddDays(4).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            sessionId = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, start);
            await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, start.AddHours(2));
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var overlap = await client.PutAsJsonAsync($"/api/Schedule/sessions/{sessionId}/reschedule", new
        {
            newStartTime = start,
            durationMinutes = 180
        });
        Assert.Equal(HttpStatusCode.BadRequest, overlap.StatusCode);
        Assert.Equal("Session.TimeConflict", await ErrorCodeAsync(overlap));
        var unchanged = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId).Select(s => new { s.StartTime, s.EndTime }).SingleAsync());
        Assert.Equal(start, unchanged.StartTime);
        Assert.Equal(start.AddHours(1), unchanged.EndTime);

        var durationOnly = await client.PutAsJsonAsync($"/api/Schedule/sessions/{sessionId}/reschedule", new
        {
            newStartTime = start,
            durationMinutes = 90
        });
        Assert.Equal(HttpStatusCode.OK, durationOnly.StatusCode);
        var updated = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == sessionId).Select(s => new { s.StartTime, s.EndTime }).SingleAsync());
        Assert.Equal(start, updated.StartTime);
        Assert.Equal(start.AddMinutes(90), updated.EndTime);
    }

    [Fact]
    public async Task BulkShift_MovesAllSelectedSessionsAndLeavesAllUntouchedWhenAnyConflicts()
    {
        SeededTeacher teacher = null!;
        Guid first = default, second = default, selectedConflict = default, blocker = default;
        var baseStart = DateTime.UtcNow.Date.AddDays(5).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            first = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, baseStart);
            second = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, baseStart.AddDays(1));
            selectedConflict = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, baseStart.AddDays(3));
            blocker = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, baseStart.AddDays(3).AddHours(1));
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var shifted = await client.PostAsJsonAsync("/api/Schedule/sessions/bulk-shift", new
        {
            from = baseStart.AddMinutes(-1),
            to = baseStart.AddDays(2),
            groupIds = new[] { teacher.GroupId },
            shiftMinutes = 30
        });
        Assert.Equal(HttpStatusCode.OK, shifted.StatusCode);
        var shiftedBody = await shifted.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2, shiftedBody.GetProperty("affected").GetInt32());
        Assert.Equal(baseStart.AddMinutes(30), await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == first).Select(s => s.StartTime).SingleAsync()));
        Assert.Equal(baseStart.AddDays(1).AddMinutes(30), await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == second).Select(s => s.StartTime).SingleAsync()));

        var conflict = await client.PostAsJsonAsync("/api/Schedule/sessions/bulk-shift", new
        {
            from = baseStart.AddDays(3).AddMinutes(-1),
            to = baseStart.AddDays(3).AddMinutes(20),
            groupIds = new[] { teacher.GroupId },
            shiftMinutes = 60
        });
        Assert.Equal(HttpStatusCode.BadRequest, conflict.StatusCode);
        Assert.Equal("Session.TimeConflict", await ErrorCodeAsync(conflict));
        Assert.Equal(baseStart.AddDays(3), await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == selectedConflict).Select(s => s.StartTime).SingleAsync()));
        Assert.Equal(baseStart.AddDays(3).AddHours(1), await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == blocker).Select(s => s.StartTime).SingleAsync()));
    }

    [Fact]
    public async Task Reschedule_RejectsAnotherGroupOfSameTeacher_EvenAtAnotherCenter()
    {
        SeededTeacher teacher = null!;
        Guid movingSession = default;
        var start = DateTime.UtcNow.Date.AddDays(8).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var otherCenter = Center.Create("Other center", teacher.OwnerId, 30.1, 31.2);
            db.Centers.Add(otherCenter);
            db.CenterTeachers.Add(CenterTeacher.CreateFounder(otherCenter.Id, teacher.TenantId));
            var otherGroup = Group.Create(teacher.TenantId, "Other group", new SchoolGrade(EducationStage.Primary, 1), otherCenter.Id, null, null, null);
            db.Groups.Add(otherGroup);
            movingSession = await TestData.AddSessionAsync(db, teacher.TenantId, teacher.GroupId, start);
            await TestData.AddSessionAsync(db, teacher.TenantId, otherGroup.Id, start.AddHours(2));
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var response = await client.PutAsJsonAsync($"/api/Schedule/sessions/{movingSession}/reschedule", new
        {
            newStartTime = start.AddHours(2), durationMinutes = 60
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Session.TimeConflict", await ErrorCodeAsync(response));
        Assert.Equal(start, await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == movingSession).Select(s => s.StartTime).SingleAsync()));
    }

    [Fact]
    public async Task Reschedule_SharedRoomReturnsDetailsThenAllowsExplicitConfirmation()
    {
        SeededTeacher firstTeacher = null!, secondTeacher = null!;
        Guid firstSession = default;
        var start = DateTime.UtcNow.Date.AddDays(9).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            firstTeacher = await TestData.CreateTeacherAsync(db);
            secondTeacher = await TestData.CreateTeacherAsync(db);
            var ownGroup = Group.Create(firstTeacher.TenantId, "Room group", new SchoolGrade(EducationStage.Primary, 1), firstTeacher.CenterId, "Room A", null, null);
            var otherGroup = Group.Create(secondTeacher.TenantId, "Shared room group", new SchoolGrade(EducationStage.Primary, 1), firstTeacher.CenterId, " room a ", null, null);
            db.Groups.AddRange(ownGroup, otherGroup);
            await db.SaveChangesAsync();
            firstSession = await TestData.AddSessionAsync(db, firstTeacher.TenantId, ownGroup.Id, start);
            await TestData.AddSessionAsync(db, secondTeacher.TenantId, otherGroup.Id, start.AddHours(2));
        });

        var client = _factory.CreateClientFor(firstTeacher.OwnerId, firstTeacher.TenantId);
        var warning = await client.PutAsJsonAsync($"/api/Schedule/sessions/{firstSession}/reschedule", new
        {
            newStartTime = start.AddHours(2), durationMinutes = 60
        });
        Assert.Equal(HttpStatusCode.Conflict, warning.StatusCode);
        var warningBody = await warning.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Schedule.RoomConflictConfirmationRequired", warningBody.GetProperty("errorCode").GetString());
        Assert.Contains("Shared room group", warningBody.GetRawText());

        var confirmed = await client.PutAsJsonAsync($"/api/Schedule/sessions/{firstSession}/reschedule", new
        {
            newStartTime = start.AddHours(2), durationMinutes = 60, confirmRoomConflicts = true
        });
        Assert.Equal(HttpStatusCode.OK, confirmed.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_RejectsWeeklyOverlapWithAnotherGroupOfSameTeacher()
    {
        SeededTeacher teacher = null!;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().SingleAsync(g => g.Id == teacher.GroupId);
            var slot = group.AddSlot(DayOfWeek.Monday, TimeSpan.FromHours(10), 60);
            db.GroupScheduleSlots.Add(slot);
            await db.SaveChangesAsync();
        });

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var response = await client.PostAsJsonAsync("/api/Groups", new
        {
            name = "Overlapping weekly group",
            grade = new { stage = "Primary", number = 1 },
            centerId = teacher.CenterId,
            room = "Room B",
            capacity = (int?)null,
            whatsAppGroupLink = (string?)null,
            slots = new[] { new { day = "Monday", startTime = "10:30:00", durationMinutes = 60 } }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Session.TimeConflict", await ErrorCodeAsync(response));
        Assert.Equal(1, await _factory.QueryAsync(db => db.Groups.IgnoreQueryFilters()
            .CountAsync(g => g.TenantId == teacher.TenantId)));
    }

    [Fact]
    public async Task CreateGroup_SharedRoomTemplateConflictRequiresConfirmation()
    {
        SeededTeacher owner = null!, visitingTeacher = null!;
        await _factory.SeedAsync(async db =>
        {
            owner = await TestData.CreateTeacherAsync(db);
            visitingTeacher = await TestData.CreateTeacherAsync(db);
            var peerGroup = Group.Create(visitingTeacher.TenantId, "Existing shared room group",
                new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, "Room A", null, null);
            peerGroup.AddSlot(DayOfWeek.Tuesday, TimeSpan.FromHours(11), 60);
            db.Groups.Add(peerGroup);
            await db.SaveChangesAsync();
        });

        var client = _factory.CreateClientFor(owner.OwnerId, owner.TenantId);
        object Request(bool confirm) => new
        {
            name = "New shared room group",
            grade = new { stage = "Primary", number = 1 },
            centerId = owner.CenterId,
            room = " room a ",
            capacity = (int?)null,
            whatsAppGroupLink = (string?)null,
            confirmRoomConflicts = confirm,
            slots = new[] { new { day = "Tuesday", startTime = "11:00:00", durationMinutes = 60 } }
        };

        var warning = await client.PostAsJsonAsync("/api/Groups", Request(false));
        Assert.Equal(HttpStatusCode.Conflict, warning.StatusCode);
        var body = await warning.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Schedule.RoomConflictConfirmationRequired", body.GetProperty("errorCode").GetString());
        Assert.Contains("Existing shared room group", body.GetRawText());

        var confirmed = await client.PostAsJsonAsync("/api/Groups", Request(true));
        Assert.Equal(HttpStatusCode.OK, confirmed.StatusCode);
    }

    [Fact]
    public async Task UpdateGroupSchedule_RejectsTeacherOverlapAndRequiresRoomConfirmation()
    {
        SeededTeacher owner = null!, visitor = null!;
        Guid editableGroupId = default;
        await _factory.SeedAsync(async db =>
        {
            owner = await TestData.CreateTeacherAsync(db);
            visitor = await TestData.CreateTeacherAsync(db);
            var editable = Group.Create(owner.TenantId, "Editable", new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, "Room A", null, null);
            var visitorGroup = Group.Create(visitor.TenantId, "Visitor", new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, "Room A", null, null);
            visitorGroup.AddSlot(DayOfWeek.Wednesday, TimeSpan.FromHours(14), 60);
            db.Groups.AddRange(editable, visitorGroup);
            await db.SaveChangesAsync();
            editableGroupId = editable.Id;
        });

        var client = _factory.CreateClientFor(owner.OwnerId, owner.TenantId);
        var templateWarning = await client.PutAsJsonAsync($"/api/Groups/{editableGroupId}/schedule", new
        {
            effectiveFrom = (DateOnly?)null,
            confirmRoomConflicts = false,
            slots = new[] { new { day = "Wednesday", startTime = "14:00:00", durationMinutes = 60 } }
        });
        Assert.Equal(HttpStatusCode.Conflict, templateWarning.StatusCode);

        var templateConfirmed = await client.PutAsJsonAsync($"/api/Groups/{editableGroupId}/schedule", new
        {
            effectiveFrom = (DateOnly?)null,
            confirmRoomConflicts = true,
            slots = new[] { new { day = "Wednesday", startTime = "14:00:00", durationMinutes = 60 } }
        });
        Assert.Equal(HttpStatusCode.OK, templateConfirmed.StatusCode);
    }

    [Fact]
    public async Task RestoreAndExtraSession_SharedRoomConflictsRequireConfirmation()
    {
        SeededTeacher owner = null!, visitor = null!;
        Guid cancelledSession = default, ownerGroupId = default;
        var start = DateTime.UtcNow.Date.AddDays(12).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            owner = await TestData.CreateTeacherAsync(db);
            visitor = await TestData.CreateTeacherAsync(db);
            var ownerGroup = Group.Create(owner.TenantId, "Owner room", new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, "Room A", null, null);
            var visitorGroup = Group.Create(visitor.TenantId, "Visitor room", new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, "Room A", null, null);
            db.Groups.AddRange(ownerGroup, visitorGroup);
            await db.SaveChangesAsync();
            ownerGroupId = ownerGroup.Id;
            cancelledSession = await TestData.AddSessionAsync(db, owner.TenantId, ownerGroup.Id, start);
            var session = await db.Sessions.IgnoreQueryFilters().SingleAsync(s => s.Id == cancelledSession);
            session.Cancel("test");
            await db.SaveChangesAsync();
            await TestData.AddSessionAsync(db, visitor.TenantId, visitorGroup.Id, start);
            await TestData.AddSessionAsync(db, visitor.TenantId, visitorGroup.Id, start.AddHours(4));
        });

        var client = _factory.CreateClientFor(owner.OwnerId, owner.TenantId);
        var restoreWarning = await client.PostAsync($"/api/Schedule/sessions/{cancelledSession}/restore", null);
        Assert.Equal(HttpStatusCode.Conflict, restoreWarning.StatusCode);
        var restored = await client.PostAsync($"/api/Schedule/sessions/{cancelledSession}/restore?confirmRoomConflicts=true", null);
        Assert.Equal(HttpStatusCode.OK, restored.StatusCode);

        object Extra(bool confirm) => new
        {
            groupId = ownerGroupId,
            startTime = start.AddHours(4),
            durationMinutes = 60,
            confirmRoomConflicts = confirm
        };
        var extraWarning = await client.PostAsJsonAsync("/api/Schedule/sessions", Extra(false));
        Assert.Equal(HttpStatusCode.Conflict, extraWarning.StatusCode);
        var extraConfirmed = await client.PostAsJsonAsync("/api/Schedule/sessions", Extra(true));
        Assert.Equal(HttpStatusCode.OK, extraConfirmed.StatusCode);
    }

    [Fact]
    public async Task BulkShift_SharedRoomConflictsRequireConfirmationForWholeBatch()
    {
        SeededTeacher owner = null!, visitor = null!;
        Guid ownerGroupId = default, movingSession = default;
        var start = DateTime.UtcNow.Date.AddDays(13).AddHours(10);
        await _factory.SeedAsync(async db =>
        {
            owner = await TestData.CreateTeacherAsync(db);
            visitor = await TestData.CreateTeacherAsync(db);
            var ownerGroup = Group.Create(owner.TenantId, "Owner bulk room", new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, "Room A", null, null);
            var visitorGroup = Group.Create(visitor.TenantId, "Visitor bulk room", new SchoolGrade(EducationStage.Primary, 1), owner.CenterId, "Room A", null, null);
            db.Groups.AddRange(ownerGroup, visitorGroup);
            await db.SaveChangesAsync();
            ownerGroupId = ownerGroup.Id;
            movingSession = await TestData.AddSessionAsync(db, owner.TenantId, ownerGroup.Id, start);
            await TestData.AddSessionAsync(db, visitor.TenantId, visitorGroup.Id, start.AddHours(1));
        });

        var client = _factory.CreateClientFor(owner.OwnerId, owner.TenantId);
        object Shift(bool confirm) => new
        {
            from = start.AddMinutes(-1), to = start.AddMinutes(30),
            groupIds = new[] { ownerGroupId }, shiftMinutes = 60,
            confirmRoomConflicts = confirm
        };
        var warning = await client.PostAsJsonAsync("/api/Schedule/sessions/bulk-shift", Shift(false));
        Assert.Equal(HttpStatusCode.Conflict, warning.StatusCode);
        var confirmed = await client.PostAsJsonAsync("/api/Schedule/sessions/bulk-shift", Shift(true));
        Assert.Equal(HttpStatusCode.OK, confirmed.StatusCode);
        Assert.Equal(start.AddHours(1), await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == movingSession).Select(s => s.StartTime).SingleAsync()));
    }
}
