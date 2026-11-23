using NSubstitute;
using TeacherERP.Application.Features.Schedule;
using TeacherERP.Application.Features.Schedule.BulkCancelSessions;
using TeacherERP.Application.Features.Schedule.CancelSession;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Unit;

// F4: حصة عليها تعويض معلّق (فردي) لا تُلغى حتى يُبتّ في التعويض
public class CancelSessionCommandHandlerTests
{
    private static readonly DateTime Now = new(2026, 11, 14, 10, 0, 0, DateTimeKind.Utc);
    private static readonly Guid TenantId = Guid.NewGuid();

    private readonly ICurrentTenantAccessor _tenant = Substitute.For<ICurrentTenantAccessor>();
    private readonly IGroupSessionRepository _sessions = Substitute.For<IGroupSessionRepository>();
    private readonly IAttendanceRepository _attendance = Substitute.For<IAttendanceRepository>();
    private readonly INotificationService _notifications = Substitute.For<INotificationService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public CancelSessionCommandHandlerTests()
    {
        _tenant.Current.Returns(new TenantContext(TenantId, TenantRole.Owner, MembershipStatus.Active));
    }

    private CancelSessionCommandHandler CreateHandler()
        => new(_tenant, _sessions, _attendance, _notifications, _unitOfWork, new FixedTimeProvider(Now));

    private Session UpcomingSession()
    {
        var session = Session.Create(TenantId, Guid.NewGuid(), Now.AddDays(1), Now.AddDays(1).AddHours(1));
        _sessions.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        return session;
    }

    private void BlockWithPendingCompensation(Session session)
    {
        _attendance
            .GetSessionIdsWithPendingCompensationAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new HashSet<Guid> { session.Id });
    }

    [Fact]
    public async Task Handle_WithPendingCompensation_FailsAndLeavesSessionUntouched()
    {
        var session = UpcomingSession();
        BlockWithPendingCompensation(session);

        var result = await CreateHandler().Handle(new CancelSessionCommand(session.Id, "holiday"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ScheduleErrors.HasPendingCompensations.Code, result.Error.Code);
        Assert.False(session.IsCancelled);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutPendingCompensation_CancelsAndSaves()
    {
        var session = UpcomingSession();
        _attendance
            .GetSessionIdsWithPendingCompensationAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new HashSet<Guid>());

        var result = await CreateHandler().Handle(new CancelSessionCommand(session.Id, "  holiday  "), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsCancelled);
        Assert.Equal("holiday", result.Value.CancelReason);
        Assert.True(session.IsCancelled);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyCancelledSession_IsIdempotentAndSkipsTheCompensationCheck()
    {
        var session = UpcomingSession();
        session.Cancel("first");

        var result = await CreateHandler().Handle(new CancelSessionCommand(session.Id, "second"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("first", session.CancelReason);
        await _attendance.DidNotReceive().GetSessionIdsWithPendingCompensationAsync(
            Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PastSession_FailsBeforeCheckingCompensations()
    {
        var past = Session.Create(TenantId, Guid.NewGuid(), Now.AddHours(-3), Now.AddHours(-2));
        _sessions.GetByIdAsync(past.Id, Arg.Any<CancellationToken>()).Returns(past);

        var result = await CreateHandler().Handle(new CancelSessionCommand(past.Id, null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ScheduleErrors.InPast.Code, result.Error.Code);
        await _attendance.DidNotReceive().GetSessionIdsWithPendingCompensationAsync(
            Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownSession_ReturnsNotFound()
    {
        var result = await CreateHandler().Handle(new CancelSessionCommand(Guid.NewGuid(), null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ScheduleErrors.NotFound.Code, result.Error.Code);
    }
}

// F4: الإلغاء الجماعي إما يُلغى الكل أو لا شيء لو أي حصة عليها تعويض معلّق
public class BulkCancelSessionsCommandHandlerTests
{
    private static readonly DateTime Now = new(2026, 11, 14, 10, 0, 0, DateTimeKind.Utc);
    private static readonly Guid TenantId = Guid.NewGuid();

    private readonly ICurrentTenantAccessor _tenant = Substitute.For<ICurrentTenantAccessor>();
    private readonly IGroupSessionRepository _sessions = Substitute.For<IGroupSessionRepository>();
    private readonly IAttendanceRepository _attendance = Substitute.For<IAttendanceRepository>();
    private readonly INotificationService _notifications = Substitute.For<INotificationService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public BulkCancelSessionsCommandHandlerTests()
    {
        _tenant.Current.Returns(new TenantContext(TenantId, TenantRole.Owner, MembershipStatus.Active));
    }

    private BulkCancelSessionsCommandHandler CreateHandler()
        => new(_tenant, _sessions, _attendance, _notifications, _unitOfWork, new FixedTimeProvider(Now));

    private static Session Upcoming(int daysAhead)
        => Session.Create(TenantId, Guid.NewGuid(), Now.AddDays(daysAhead), Now.AddDays(daysAhead).AddHours(1));

    private void ReturnSessions(params Session[] sessions)
    {
        _sessions
            .GetTrackedByRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<IReadOnlyCollection<Guid>?>(), Arg.Any<CancellationToken>())
            .Returns(sessions.ToList());
    }

    private static BulkCancelSessionsCommand Range() => new(Now.AddDays(1), Now.AddDays(10), null, "holiday");

    [Fact]
    public async Task Handle_WhenOneSessionHasPendingCompensation_CancelsNothing()
    {
        var blocked = Upcoming(2);
        var free = Upcoming(3);
        ReturnSessions(blocked, free);
        _attendance
            .GetSessionIdsWithPendingCompensationAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new HashSet<Guid> { blocked.Id });

        var result = await CreateHandler().Handle(Range(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ScheduleErrors.HasPendingCompensations.Code, result.Error.Code);
        Assert.False(blocked.IsCancelled);
        Assert.False(free.IsCancelled);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutPendingCompensations_CancelsEverythingAndSavesOnce()
    {
        var first = Upcoming(2);
        var second = Upcoming(3);
        ReturnSessions(first, second);
        _attendance
            .GetSessionIdsWithPendingCompensationAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new HashSet<Guid>());

        var result = await CreateHandler().Handle(Range(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Affected);
        Assert.Equal(0, result.Value.Skipped);
        Assert.True(first.IsCancelled);
        Assert.True(second.IsCancelled);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyCancelledSessions_AreSkippedAndNotCheckedForCompensations()
    {
        var alreadyCancelled = Upcoming(2);
        alreadyCancelled.Cancel("earlier");
        var active = Upcoming(3);
        ReturnSessions(alreadyCancelled, active);
        _attendance
            .GetSessionIdsWithPendingCompensationAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new HashSet<Guid>());

        var result = await CreateHandler().Handle(Range(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Affected);
        Assert.Equal(1, result.Value.Skipped);
        // الفحص يتم على الحصص غير الملغاة فقط
        await _attendance.Received(1).GetSessionIdsWithPendingCompensationAsync(
            Arg.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 1 && ids.Contains(active.Id)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyRange_DoesNothing()
    {
        ReturnSessions();

        var result = await CreateHandler().Handle(Range(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value!.Affected);
        await _attendance.DidNotReceive().GetSessionIdsWithPendingCompensationAsync(
            Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
