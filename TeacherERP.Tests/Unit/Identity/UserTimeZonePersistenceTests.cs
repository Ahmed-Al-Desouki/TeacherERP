using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TeacherERP.Application.Features.Users.Commands.CreateStudent;
using TeacherERP.Application.Features.Users.Commands.VerifyOtp;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Sessions;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Unit.Identity;

public sealed class UserTimeZonePersistenceTests
{
    [Fact]
    public void UserTimeZone_RejectsUnknownZonesAndNormalizesUtc()
    {
        var user = ApplicationUser.CreateStudent("Student", "student@example.test");

        user.SetTimeZoneId("Invalid/Zone");
        Assert.Equal("UTC", user.TimeZoneId);

        user.SetTimeZoneId("utc");
        Assert.Equal("UTC", user.TimeZoneId);
    }

    [Fact]
    public async Task Registration_StoresValidatedRequestTimeZoneInThePendingPayload()
    {
        var users = Substitute.For<IUserAccountService>();
        users.CheckEmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(false));
        users.HashPasswordAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<string>.Success("hashed-password"));
        var otp = Substitute.For<IOtpService>();
        otp.GenerateAsync(OtpPurpose.Registration, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<OtpIssueResult>.Success(new OtpIssueResult("123456", 600, 60)));
        var state = Substitute.For<ITransientAuthStateStore>();
        var zone = Substitute.For<IUserTimeZoneAccessor>();
        zone.TimeZoneId.Returns("Asia/Riyadh");

        var handler = new CreateStudentCommandHandler(users, otp, state, Substitute.For<IBackgroundJobClient>(), zone);
        var result = await handler.Handle(
            new CreateStudentCommand("Student", "student@example.test", "Strong#Password1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await state.Received(1).SavePayloadAsync(
            OtpPurpose.Registration.ToString(), "student@example.test",
            Arg.Is<PendingStudentRegistrationData>(payload => payload.TimeZoneId == "Asia/Riyadh"),
            Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegistrationVerification_CopiesPendingTimeZoneToTheNewUser()
    {
        var payload = new PendingStudentRegistrationData
        {
            FullName = "Student",
            Email = "student@example.test",
            PasswordHash = "hashed-password",
            TimeZoneId = "Asia/Riyadh"
        };
        var state = Substitute.For<ITransientAuthStateStore>();
        state.GetPayloadAsync<PendingStudentRegistrationData>(
                OtpPurpose.Registration.ToString(), payload.Email, Arg.Any<CancellationToken>())
            .Returns(payload);
        var otp = Substitute.For<IOtpService>();
        otp.VerifyAsync(OtpPurpose.Registration, payload.Email, "123456", Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(true));
        var users = Substitute.For<IUserAccountService>();
        users.CreateUserWithPasswordHashAsync(
                Arg.Any<ApplicationUser>(), payload.PasswordHash, RoleNames.Student, Arg.Any<CancellationToken>())
            .Returns(Result<Guid>.Success(Guid.NewGuid()));

        var handler = new VerifyOtpCommandHandler(state, users, otp);
        var result = await handler.Handle(new VerifyOtpCommand(payload.Email, "123456"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await users.Received(1).CreateUserWithPasswordHashAsync(
            Arg.Is<ApplicationUser>(user => user.TimeZoneId == "Asia/Riyadh"),
            payload.PasswordHash, RoleNames.Student, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SessionLogin_UsesPersistedTimeZoneWhenRequestHasNoZoneHeader()
    {
        var user = ApplicationUser.CreateStudent("Student", "student@example.test");
        user.SetTimeZoneId("Asia/Riyadh");
        var sessions = Substitute.For<ISessionRepository>();
        sessions.GetActiveByUserAsync(user.Id, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<LoginSession>());
        var jwt = Substitute.For<IJwtProvider>();
        jwt.GenerateRefreshToken().Returns("refresh-secret");
        jwt.GenerateAccessTokenAsync(user, Arg.Any<Guid>())
            .Returns(new AccessTokenResult("access-token", Array.Empty<string>()));
        var jobs = Substitute.For<IBackgroundJobClient>();
        Job? scheduledJob = null;
        jobs.When(client => client.Create(Arg.Any<Job>(), Arg.Any<IState>()))
            .Do(call => scheduledJob = call.Arg<Job>());
        var timeZone = Substitute.For<IUserTimeZoneAccessor>();

        var manager = new SessionManager(
            jwt, sessions, Substitute.For<ISessionValidator>(), Substitute.For<IUnitOfWork>(),
            jobs, timeZone, TimeProvider.System, NullLogger<SessionManager>.Instance);

        var response = await manager.IssueAsync(user, "device-id", "device", CancellationToken.None);

        Assert.NotNull(scheduledJob);
        Assert.Equal("Asia/Riyadh", user.TimeZoneId);
        Assert.Equal("Asia/Riyadh", response.TimeZoneId);
        Assert.Equal("Asia/Riyadh", scheduledJob!.Args[3]);
    }

    [Fact]
    public async Task SessionLogin_UpdatesAndPersistsAValidZoneFromTheRequest()
    {
        var user = ApplicationUser.CreateStudent("Student", "student@example.test");
        var sessions = Substitute.For<ISessionRepository>();
        sessions.GetActiveByUserAsync(user.Id, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<LoginSession>());
        var jwt = Substitute.For<IJwtProvider>();
        jwt.GenerateRefreshToken().Returns("refresh-secret");
        jwt.GenerateAccessTokenAsync(user, Arg.Any<Guid>())
            .Returns(new AccessTokenResult("access-token", Array.Empty<string>()));
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var timeZone = Substitute.For<IUserTimeZoneAccessor>();
        timeZone.TimeZoneId.Returns("Asia/Riyadh");

        var manager = new SessionManager(
            jwt, sessions, Substitute.For<ISessionValidator>(), unitOfWork,
            Substitute.For<IBackgroundJobClient>(), timeZone, TimeProvider.System, NullLogger<SessionManager>.Instance);

        var response = await manager.IssueAsync(user, "device-id", "device", CancellationToken.None);

        Assert.Equal("Asia/Riyadh", user.TimeZoneId);
        Assert.Equal("Asia/Riyadh", response.TimeZoneId);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
