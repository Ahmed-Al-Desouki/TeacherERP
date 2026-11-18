using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class SessionGenerationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SessionGenerationIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task GenerateForGroup_IsIdempotent_DoesNotCreatePastSessions_AndIncludesDay28()
    {
        SeededTeacher teacher = null!;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var horizonDay = today.AddDays(28);
        var slotTime = TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59));

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().Include(g => g.Slots)
                .SingleAsync(g => g.Id == teacher.GroupId);
            var slot = group.AddSlot(horizonDay.DayOfWeek, slotTime, 60);
            db.GroupScheduleSlots.Add(slot);
            await db.SaveChangesAsync();
        });

        int generated;
        int generatedAgain;
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>()
                .Set(new TenantContext(teacher.TenantId, TenantRole.Owner, MembershipStatus.Active));
            var generator = scope.ServiceProvider.GetRequiredService<ISessionGenerator>();
            generated = await generator.GenerateForGroupAsync(teacher.GroupId, CancellationToken.None);
            generatedAgain = await generator.GenerateForGroupAsync(teacher.GroupId, CancellationToken.None);
        }

        var sessions = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.TenantId == teacher.TenantId && s.GroupId == teacher.GroupId)
            .OrderBy(s => s.StartTime).ToListAsync());
        var now = DateTime.UtcNow;

        Assert.True(generated > 0);
        Assert.Equal(0, generatedAgain);
        Assert.All(sessions, session => Assert.True(session.StartTime > now));
        Assert.All(sessions, session => Assert.True(DateOnly.FromDateTime(session.StartTime) <= horizonDay));
        Assert.Contains(sessions, session => DateOnly.FromDateTime(session.StartTime) == horizonDay
                                               && session.StartTime.TimeOfDay == slotTime);
    }

    [Fact]
    public async Task UpdateGroupSchedule_FromEffectiveDate_ReplacesOnlyUpcomingUntouchedGeneratedSessions()
    {
        SeededTeacher teacher = null!;
        var targetDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3);
        var oldTime = TimeSpan.FromHours(18);
        var newTime = TimeSpan.FromHours(19);
        Guid oldSessionId = default, pastSessionId = default, modifiedFutureSessionId = default;

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().Include(g => g.Slots)
                .SingleAsync(g => g.Id == teacher.GroupId);
            var slot = group.AddSlot(targetDate.DayOfWeek, oldTime, 60);
            db.GroupScheduleSlots.Add(slot);
            var start = targetDate.ToDateTime(TimeOnly.FromTimeSpan(oldTime), DateTimeKind.Utc);
            var oldSession = Session.Create(teacher.TenantId, group.Id, start, start.AddHours(1), slot.Id);
            var pastStart = targetDate.AddDays(-7).ToDateTime(TimeOnly.FromTimeSpan(oldTime), DateTimeKind.Utc);
            var pastSession = Session.Create(teacher.TenantId, group.Id, pastStart, pastStart.AddHours(1), slot.Id);
            var modifiedStart = targetDate.AddDays(7).ToDateTime(TimeOnly.FromTimeSpan(oldTime), DateTimeKind.Utc);
            var modifiedFutureSession = Session.Create(
                teacher.TenantId, group.Id, modifiedStart, modifiedStart.AddHours(1), slot.Id);
            modifiedFutureSession.Reschedule(modifiedStart, modifiedStart.AddMinutes(90));
            db.Sessions.Add(oldSession);
            db.Sessions.Add(pastSession);
            db.Sessions.Add(modifiedFutureSession);
            await db.SaveChangesAsync();
            oldSessionId = oldSession.Id;
            pastSessionId = pastSession.Id;
            modifiedFutureSessionId = modifiedFutureSession.Id;
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PutAsJsonAsync($"/api/Groups/{teacher.GroupId}/schedule", new
            {
                effectiveFrom = targetDate.ToString("yyyy-MM-dd"),
                slots = new[]
                {
                    new { day = targetDate.DayOfWeek.ToString(), startTime = "19:00:00", durationMinutes = 60 }
                }
            });

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var rows = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.TenantId == teacher.TenantId && s.GroupId == teacher.GroupId
                        && s.StartTime >= targetDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
                        && s.StartTime < targetDate.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc))
            .ToListAsync());
        Assert.DoesNotContain(rows, session => session.Id == oldSessionId);
        Assert.Contains(rows, session => session.StartTime.TimeOfDay == newTime && session.EndTime - session.StartTime == TimeSpan.FromHours(1));
        Assert.True(await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters().AnyAsync(s =>
            s.Id == pastSessionId && s.StartTime == targetDate.AddDays(-7).ToDateTime(TimeOnly.FromTimeSpan(oldTime), DateTimeKind.Utc)
            && s.SlotId == null)));
        var modifiedFuture = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.Id == modifiedFutureSessionId)
            .Select(s => new { s.StartTime, s.EndTime, s.SlotId })
            .SingleAsync());
        Assert.Equal(targetDate.AddDays(7).ToDateTime(TimeOnly.FromTimeSpan(oldTime), DateTimeKind.Utc), modifiedFuture.StartTime);
        Assert.Equal(TimeSpan.FromMinutes(90), modifiedFuture.EndTime - modifiedFuture.StartTime);
        Assert.Null(modifiedFuture.SlotId);
    }

    [Fact]
    public async Task UpdateGroupSchedule_SwapsWeeklyDaysFromEffectiveDate_AndRegeneratesOnlyUntouchedFutureOccurrences()
    {
        SeededTeacher teacher = null!;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var effectiveDate = NextOrSame(today.AddDays(10), DayOfWeek.Monday);
        var previousMonday = effectiveDate.AddDays(-7);
        var previousWednesday = effectiveDate.AddDays(-5);
        var nextWednesday = effectiveDate.AddDays(2);
        var followingMonday = effectiveDate.AddDays(7);
        var followingWednesday = effectiveDate.AddDays(9);
        var mondayTime = TimeSpan.FromHours(10);
        var wednesdayTime = TimeSpan.FromHours(16);
        Guid previousMondayId = default, previousWednesdayId = default;
        Guid removedMondayId = default, removedWednesdayId = default;
        Guid modifiedFutureId = default, attendedFutureId = default;
        Guid originalMondaySlotId = default, originalWednesdaySlotId = default, studentId = default;

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().Include(g => g.Slots)
                .SingleAsync(g => g.Id == teacher.GroupId);
            var mondaySlot = group.AddSlot(DayOfWeek.Monday, mondayTime, 60);
            var wednesdaySlot = group.AddSlot(DayOfWeek.Wednesday, wednesdayTime, 90);
            db.GroupScheduleSlots.AddRange(mondaySlot, wednesdaySlot);
            originalMondaySlotId = mondaySlot.Id;
            originalWednesdaySlotId = wednesdaySlot.Id;

            Session AddAt(DateOnly date, TimeSpan time, int minutes, Guid slotId)
            {
                var start = date.ToDateTime(TimeOnly.FromTimeSpan(time), DateTimeKind.Utc);
                var session = Session.Create(teacher.TenantId, group.Id, start, start.AddMinutes(minutes), slotId);
                db.Sessions.Add(session);
                return session;
            }

            var previousMondaySession = AddAt(previousMonday, mondayTime, 60, mondaySlot.Id);
            var previousWednesdaySession = AddAt(previousWednesday, wednesdayTime, 90, wednesdaySlot.Id);
            var removedMondaySession = AddAt(effectiveDate, mondayTime, 60, mondaySlot.Id);
            var removedWednesdaySession = AddAt(nextWednesday, wednesdayTime, 90, wednesdaySlot.Id);
            var modifiedSession = AddAt(followingMonday, mondayTime, 60, mondaySlot.Id);
            modifiedSession.Reschedule(modifiedSession.StartTime, modifiedSession.EndTime.AddMinutes(30));
            var attendedSession = AddAt(followingWednesday, wednesdayTime, 90, wednesdaySlot.Id);

            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, withMembership: false);
            db.AttendanceRecords.Add(AttendanceRecord.RecordPresence(
                teacher.TenantId, attendedSession.Id, studentId, ScanMethod.ManualEntry, null, null, teacher.OwnerId));
            await db.SaveChangesAsync();

            previousMondayId = previousMondaySession.Id;
            previousWednesdayId = previousWednesdaySession.Id;
            removedMondayId = removedMondaySession.Id;
            removedWednesdayId = removedWednesdaySession.Id;
            modifiedFutureId = modifiedSession.Id;
            attendedFutureId = attendedSession.Id;
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PutAsJsonAsync($"/api/Groups/{teacher.GroupId}/schedule", new
            {
                effectiveFrom = effectiveDate.ToString("yyyy-MM-dd"),
                slots = new[]
                {
                    new { day = "Monday", startTime = "16:00:00", durationMinutes = 90 },
                    new { day = "Wednesday", startTime = "10:00:00", durationMinutes = 60 }
                }
            });

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var sessions = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(s => s.TenantId == teacher.TenantId && s.GroupId == teacher.GroupId)
            .ToListAsync());
        var byId = sessions.ToDictionary(session => session.Id);

        Assert.True(byId.TryGetValue(previousMondayId, out var previousMondaySession));
        Assert.Null(previousMondaySession!.SlotId);
        Assert.True(byId.TryGetValue(previousWednesdayId, out var previousWednesdaySession));
        Assert.Null(previousWednesdaySession!.SlotId);
        Assert.DoesNotContain(removedMondayId, byId.Keys);
        Assert.DoesNotContain(removedWednesdayId, byId.Keys);

        Assert.True(byId.TryGetValue(modifiedFutureId, out var modifiedFuture));
        Assert.Equal(followingMonday.ToDateTime(TimeOnly.FromTimeSpan(mondayTime), DateTimeKind.Utc), modifiedFuture!.StartTime);
        Assert.Equal(TimeSpan.FromMinutes(90), modifiedFuture.EndTime - modifiedFuture.StartTime);
        Assert.Null(modifiedFuture.SlotId);

        Assert.True(byId.TryGetValue(attendedFutureId, out var attendedFuture));
        Assert.Null(attendedFuture!.SlotId);
        Assert.True(await _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .AnyAsync(record => record.SessionId == attendedFutureId && record.StudentId == studentId)));

        DateTime At(DateOnly date, TimeSpan time) => date.ToDateTime(TimeOnly.FromTimeSpan(time), DateTimeKind.Utc);
        Assert.Contains(sessions, session => session.StartTime == At(effectiveDate, wednesdayTime)
                                             && session.EndTime - session.StartTime == TimeSpan.FromMinutes(90));
        Assert.Contains(sessions, session => session.StartTime == At(nextWednesday, mondayTime)
                                             && session.EndTime - session.StartTime == TimeSpan.FromMinutes(60));
        Assert.Contains(sessions, session => session.StartTime == At(followingMonday, wednesdayTime)
                                             && session.EndTime - session.StartTime == TimeSpan.FromMinutes(90));
        Assert.Contains(sessions, session => session.StartTime == At(followingWednesday, mondayTime)
                                             && session.EndTime - session.StartTime == TimeSpan.FromMinutes(60));
        Assert.DoesNotContain(sessions, session => session.SlotId == originalMondaySlotId
                                                    || session.SlotId == originalWednesdaySlotId);
    }

    private static DateOnly NextOrSame(DateOnly date, DayOfWeek day)
        => date.AddDays(((int)day - (int)date.DayOfWeek + 7) % 7);

    [Fact]
    public async Task ArchiveGroup_PreservesExistingSessions_StopsGeneration_AndReactivationGeneratesMissingSlotsOnly()
    {
        SeededTeacher teacher = null!;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var firstSessionDate = today.AddDays(5);
        var addedSlotDate = firstSessionDate.AddDays(1);
        var firstSlotTime = TimeSpan.FromHours(10);
        var addedSlotTime = TimeSpan.FromHours(14);

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            var group = await db.Groups.IgnoreQueryFilters().Include(g => g.Slots)
                .SingleAsync(g => g.Id == teacher.GroupId);
            db.GroupScheduleSlots.Add(group.AddSlot(firstSessionDate.DayOfWeek, firstSlotTime, 60));
            await db.SaveChangesAsync();
        });

        int initiallyGenerated;
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>()
                .Set(new TenantContext(teacher.TenantId, TenantRole.Owner, MembershipStatus.Active));
            initiallyGenerated = await scope.ServiceProvider.GetRequiredService<ISessionGenerator>()
                .GenerateForGroupAsync(teacher.GroupId, CancellationToken.None);
        }
        Assert.True(initiallyGenerated > 0);

        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var groupName = $"Archive lifecycle {teacher.GroupId:N}";
        var archived = await client.PutAsJsonAsync($"/api/Groups/{teacher.GroupId}", new
        {
            name = groupName,
            grade = new { stage = "Primary", number = 1 },
            centerId = teacher.CenterId,
            room = (string?)null,
            capacity = (int?)null,
            whatsAppGroupLink = (string?)null,
            isActive = false
        });
        Assert.Equal(System.Net.HttpStatusCode.OK, archived.StatusCode);
        Assert.False(await _factory.QueryAsync(db => db.Groups.IgnoreQueryFilters()
            .Where(g => g.Id == teacher.GroupId).Select(g => g.IsActive).SingleAsync()));

        // أضف موعدًا جديدًا أثناء الأرشفة؛ يجب ألا يولّد له النظام حصصًا حتى إعادة التفعيل.
        var scheduleWhileArchived = await client.PutAsJsonAsync($"/api/Groups/{teacher.GroupId}/schedule", new
        {
            effectiveFrom = today.ToString("yyyy-MM-dd"),
            slots = new[]
            {
                new { day = firstSessionDate.DayOfWeek.ToString(), startTime = "10:00:00", durationMinutes = 60 },
                new { day = addedSlotDate.DayOfWeek.ToString(), startTime = "14:00:00", durationMinutes = 60 }
            }
        });
        Assert.Equal(System.Net.HttpStatusCode.OK, scheduleWhileArchived.StatusCode);

        int generatedWhileArchived;
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>()
                .Set(new TenantContext(teacher.TenantId, TenantRole.Owner, MembershipStatus.Active));
            var activeGroups = await scope.ServiceProvider.GetRequiredService<IGroupRepository>()
                .GetActiveWithSlotsAsync(CancellationToken.None);
            Assert.DoesNotContain(activeGroups, group => group.Id == teacher.GroupId);
            generatedWhileArchived = await scope.ServiceProvider.GetRequiredService<ISessionGenerator>()
                .GenerateForGroupAsync(teacher.GroupId, CancellationToken.None);
        }
        Assert.Equal(0, generatedWhileArchived);

        var beforeReactivation = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(session => session.TenantId == teacher.TenantId && session.GroupId == teacher.GroupId)
            .ToListAsync());
        Assert.Equal(initiallyGenerated, beforeReactivation.Count);
        Assert.DoesNotContain(beforeReactivation, session =>
            session.StartTime == addedSlotDate.ToDateTime(TimeOnly.FromTimeSpan(addedSlotTime), DateTimeKind.Utc));
        var existingIds = beforeReactivation.Select(session => session.Id).ToHashSet();

        var reactivated = await client.PutAsJsonAsync($"/api/Groups/{teacher.GroupId}", new
        {
            name = groupName,
            grade = new { stage = "Primary", number = 1 },
            centerId = teacher.CenterId,
            room = (string?)null,
            capacity = (int?)null,
            whatsAppGroupLink = (string?)null,
            isActive = true
        });
        Assert.Equal(System.Net.HttpStatusCode.OK, reactivated.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.Groups.IgnoreQueryFilters()
            .Where(g => g.Id == teacher.GroupId).Select(g => g.IsActive).SingleAsync()));
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>()
                .Set(new TenantContext(teacher.TenantId, TenantRole.Owner, MembershipStatus.Active));
            var activeGroups = await scope.ServiceProvider.GetRequiredService<IGroupRepository>()
                .GetActiveWithSlotsAsync(CancellationToken.None);
            Assert.Contains(activeGroups, group => group.Id == teacher.GroupId);
        }

        var afterReactivation = await _factory.QueryAsync(db => db.Sessions.IgnoreQueryFilters()
            .Where(session => session.TenantId == teacher.TenantId && session.GroupId == teacher.GroupId)
            .ToListAsync());
        Assert.All(existingIds, id => Assert.Contains(afterReactivation, session => session.Id == id));
        Assert.True(afterReactivation.Count > beforeReactivation.Count);
        Assert.Contains(afterReactivation, session =>
            session.StartTime == addedSlotDate.ToDateTime(TimeOnly.FromTimeSpan(addedSlotTime), DateTimeKind.Utc));
    }
}
