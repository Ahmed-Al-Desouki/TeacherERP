using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Services.Users;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Unit.Identity;

public sealed class PasswordChangedNotifierTests
{
    [Fact]
    public void Enqueue_SchedulesBilingualPasswordChangeAlertWithUtcTimeAndUserTimeZone()
    {
        var jobs = Substitute.For<IBackgroundJobClient>();
        var timeZone = Substitute.For<IUserTimeZoneAccessor>();
        timeZone.TimeZoneId.Returns("Africa/Cairo");
        var now = new DateTime(2030, 4, 5, 6, 7, 8, DateTimeKind.Utc);
        var notifier = new PasswordChangedNotifier(jobs, timeZone, new FixedTimeProvider(now), NullLogger<PasswordChangedNotifier>.Instance);
        var user = ApplicationUser.CreateStudent("Password test", "password-alert@example.test");
        Job? scheduledJob = null;
        jobs.When(client => client.Create(Arg.Any<Job>(), Arg.Any<IState>()))
            .Do(call => scheduledJob = call.Arg<Job>());

        notifier.Enqueue(user);

        Assert.NotNull(scheduledJob);
        Assert.Equal(nameof(IEmailService.SendPasswordChangedAlertAsync), scheduledJob!.Method.Name);
        Assert.Equal("password-alert@example.test", scheduledJob.Args[0]);
        Assert.Equal(now, scheduledJob.Args[1]);
        Assert.Equal("Africa/Cairo", scheduledJob.Args[2]);
    }

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        private readonly DateTimeOffset _now = new(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc));
        public override DateTimeOffset GetUtcNow() => _now;
    }
}
