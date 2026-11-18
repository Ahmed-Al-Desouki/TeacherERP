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
}
