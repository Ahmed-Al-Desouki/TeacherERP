using System.Data;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TeacherERP.Application.Features.Administration;
using TeacherERP.Application.Features.Administration.CreateCenter;
using TeacherERP.Application.Features.Administration.CreateTeacher;
using TeacherERP.Application.Features.Administration.LinkTeacherToCenter;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Teachers;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Unit;

// A3: إنشاء مدرس بباسورد مؤقت ومساحته (الخدمة المشتركة)
public class TeacherProvisioningServiceTests
{
    private readonly IUserAccountService _users = Substitute.For<IUserAccountService>();
    private readonly ITenantRepository _tenants = Substitute.For<ITenantRepository>();
    private readonly IPasswordGenerator _passwords = Substitute.For<IPasswordGenerator>();
    private readonly IEmailService _email = Substitute.For<IEmailService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public TeacherProvisioningServiceTests()
    {
        _passwords.Generate().Returns("Temp#Pass1");
        _users.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<ApplicationUser?>.Success(null));
        _users.CreateUserAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<Guid>.Success(Guid.NewGuid()));

        // الـ Transaction الحقيقية تنفذ الدالة وتعيد نتيجتها، فنحاكيها بنفس السلوك
        _unitOfWork.ExecuteInTransactionAsync(
                Arg.Any<Func<Task<Result<CreatedTeacherDto>>>>(), Arg.Any<IsolationLevel>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Func<Task<Result<CreatedTeacherDto>>>>().Invoke());
    }

    private TeacherProvisioningService CreateService()
        => new(_users, _tenants, _passwords, _email, _unitOfWork, NullLogger<TeacherProvisioningService>.Instance);

    [Fact]
    public async Task Provision_WhenEmailExists_Fails_AndCreatesNothing()
    {
        _users.FindByEmailAsync("t@x.com", Arg.Any<CancellationToken>())
            .Returns(Result<ApplicationUser?>.Success(ApplicationUser.CreateTeacher("Old", "t@x.com")));

        var result = await CreateService().ProvisionAsync("Teacher", "t@x.com", "Africa/Cairo", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(AdministrationErrors.EmailInUse, result.Error);
        await _users.DidNotReceiveWithAnyArgs().CreateUserAsync(default!, default!, default!, default);
        await _email.DidNotReceiveWithAnyArgs().SendTeacherWelcomeAsync(default!, default!, default!);
    }

    [Fact]
    public async Task Provision_Success_CreatesSuperAdminWithOwnSpace_AndEmailsTemporaryPassword()
    {
        var result = await CreateService().ProvisionAsync("Teacher", "t@x.com", "Africa/Cairo", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value!.IsActivated);
        Assert.Equal("t@x.com", result.Value.Email);

        await _users.Received(1).CreateUserAsync(
            Arg.Is<ApplicationUser>(u => u.Email == "t@x.com" && !u.EmailConfirmed && u.TimeZoneId == "Africa/Cairo"),
            "Temp#Pass1",
            RoleNames.SuperAdmin,
            Arg.Any<CancellationToken>());
        _tenants.Received(1).AddTeacherSpace(
            Arg.Is<Tenant>(t => t.OwnerId == result.Value.UserId),
            Arg.Any<TenantMember>(),
            Arg.Any<TenantSettings>());
        await _email.Received(1).SendTeacherWelcomeAsync("t@x.com", "Teacher", "Temp#Pass1");
    }

    [Fact]
    public async Task Provision_WhenUserCreationFails_ReturnsThatError_AndSendsNoEmail()
    {
        var error = new Error("User.Failed", "failed");
        _users.CreateUserAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<Guid>.Failure(error));

        var result = await CreateService().ProvisionAsync("Teacher", "t@x.com", "Africa/Cairo", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        _tenants.DidNotReceiveWithAnyArgs().AddTeacherSpace(default!, default!, default!);
        await _email.DidNotReceiveWithAnyArgs().SendTeacherWelcomeAsync(default!, default!, default!);
    }

    [Fact]
    public async Task Provision_WhenEmailFails_ReturnsEmailSendFailed()
    {
        _email.SendTeacherWelcomeAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromException(new InvalidOperationException("smtp down")));

        var result = await CreateService().ProvisionAsync("Teacher", "t@x.com", "Africa/Cairo", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(AdministrationErrors.EmailSendFailed, result.Error);
    }
}

public class CreateTeacherCommandHandlerTests
{
    private readonly ITeacherProvisioningService _provisioning = Substitute.For<ITeacherProvisioningService>();

    [Fact]
    public async Task Handle_NormalizesInput_AndDefaultsTimeZoneToCairo()
    {
        var dto = new CreatedTeacherDto(Guid.NewGuid(), Guid.NewGuid(), "Ali", "ali@x.com", false);
        _provisioning.ProvisionAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreatedTeacherDto>.Success(dto));

        var result = await new CreateTeacherCommandHandler(_provisioning)
            .Handle(new CreateTeacherCommand("  Ali  ", " Ali@X.com "), CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _provisioning.Received(1).ProvisionAsync("Ali", "ali@x.com", "Africa/Cairo", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UsesRequestedTimeZone()
    {
        _provisioning.ProvisionAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreatedTeacherDto>.Failure(AdministrationErrors.EmailInUse));

        await new CreateTeacherCommandHandler(_provisioning)
            .Handle(new CreateTeacherCommand("Ali", "ali@x.com", "Asia/Riyadh"), CancellationToken.None);

        await _provisioning.Received(1).ProvisionAsync("Ali", "ali@x.com", "Asia/Riyadh", Arg.Any<CancellationToken>());
    }
}

public class CreateTeacherValidatorTests
{
    private readonly CreateTeacherValidator _validator = new();

    [Fact]
    public void Valid_WithoutTimeZone() =>
        Assert.True(_validator.Validate(new CreateTeacherCommand("Ali", "ali@x.com")).IsValid);

    [Fact]
    public void Valid_WithUtcOrKnownTimeZone()
    {
        Assert.True(_validator.Validate(new CreateTeacherCommand("Ali", "ali@x.com", "UTC")).IsValid);
        Assert.True(_validator.Validate(new CreateTeacherCommand("Ali", "ali@x.com", "Africa/Cairo")).IsValid);
    }

    [Fact]
    public void Invalid_WhenTimeZoneUnknown() =>
        Assert.False(_validator.Validate(new CreateTeacherCommand("Ali", "ali@x.com", "Mars/Base")).IsValid);

    [Fact]
    public void Invalid_WhenEmailOrNameBad()
    {
        Assert.False(_validator.Validate(new CreateTeacherCommand("Ali", "not-an-email")).IsValid);
        Assert.False(_validator.Validate(new CreateTeacherCommand("A", "ali@x.com")).IsValid);
    }
}

// A3: سنتر جديد لمدرس موجود
public class CreateCenterCommandHandlerTests
{
    private readonly ITenantRepository _tenants = Substitute.For<ITenantRepository>();
    private readonly ICenterProvisioningService _provisioning = Substitute.For<ICenterProvisioningService>();

    private CreateCenterCommandHandler CreateHandler() => new(_tenants, _provisioning);

    [Fact]
    public async Task Handle_WhenOwnerIsNotATeacher_Fails()
    {
        _tenants.GetByOwnerIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Tenant?)null);

        var result = await CreateHandler().Handle(new CreateCenterCommand("Center", Guid.NewGuid(), null, 30.05, 31.33), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(AdministrationErrors.TeacherNotFound, result.Error);
        await _provisioning.DidNotReceiveWithAnyArgs().CreateAsync(default, default!, default, default, default, default, default);
    }

    [Fact]
    public async Task Handle_CreatesCenter_AndMakesTeacherItsActiveFounder()
    {
        var ownerId = Guid.NewGuid();
        var tenant = Tenant.Create("Teacher", ownerId, "Africa/Cairo");
        _tenants.GetByOwnerIdAsync(ownerId, Arg.Any<CancellationToken>()).Returns(tenant);

        var center = Center.Create("Al-Nour", ownerId, 30.05, 31.33);
        _provisioning.CreateAsync(ownerId, "Al-Nour", "Nasr City", 30.05, 31.33, 50, Arg.Any<CancellationToken>())
            .Returns(Result<Center>.Success(center));
        var result = await CreateHandler().Handle(new CreateCenterCommand("  Al-Nour  ", ownerId, "Nasr City", 30.05, 31.33), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Al-Nour", result.Value!.Name);
        Assert.Equal(ownerId, result.Value.OwnerUserId);
        await _provisioning.Received(1).CreateAsync(ownerId, "Al-Nour", "Nasr City", 30.05, 31.33, 50, Arg.Any<CancellationToken>());
    }
}

// A3: ربط مدرس موجود بسنتر
public class LinkTeacherToCenterCommandHandlerTests
{
    private readonly ITenantRepository _tenants = Substitute.For<ITenantRepository>();
    private readonly ICenterRepository _centers = Substitute.For<ICenterRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Guid _teacherId = Guid.NewGuid();
    private readonly Center _center;
    private readonly Tenant _tenant;

    public LinkTeacherToCenterCommandHandlerTests()
    {
        _center = Center.Create("Center", Guid.NewGuid(), 30.05, 31.33);
        _tenant = Tenant.Create("Teacher", _teacherId, "Africa/Cairo");
        _centers.GetByIdAsync(_center.Id, Arg.Any<CancellationToken>()).Returns(_center);
        _tenants.GetByOwnerIdAsync(_teacherId, Arg.Any<CancellationToken>()).Returns(_tenant);
    }

    private LinkTeacherToCenterCommandHandler CreateHandler() => new(_tenants, _centers, _unitOfWork);

    [Fact]
    public async Task Handle_WhenCenterMissing_Fails()
    {
        var result = await CreateHandler().Handle(new LinkTeacherToCenterCommand(Guid.NewGuid(), _teacherId), CancellationToken.None);

        Assert.Equal(AdministrationErrors.CenterNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenTeacherMissing_Fails()
    {
        var result = await CreateHandler().Handle(new LinkTeacherToCenterCommand(_center.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(AdministrationErrors.TeacherNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenNotLinked_AddsActiveLink_WithAdministratorOrigin()
    {
        var result = await CreateHandler().Handle(new LinkTeacherToCenterCommand(_center.Id, _teacherId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(CenterTeacherStatus.Active, result.Value!.Status);
        _centers.Received(1).AddCenterTeacher(Arg.Is<CenterTeacher>(link =>
            link.CenterId == _center.Id
            && link.TenantId == _tenant.Id
            && link.Origin == CenterTeacherOrigin.Administrator));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAlreadyActive_Fails()
    {
        var link = CenterTeacher.CreateActive(_center.Id, _tenant.Id, CenterTeacherOrigin.Invitation);
        _centers.GetCenterTeacherAsync(_center.Id, _tenant.Id, Arg.Any<CancellationToken>()).Returns(link);

        var result = await CreateHandler().Handle(new LinkTeacherToCenterCommand(_center.Id, _teacherId), CancellationToken.None);

        Assert.Equal(AdministrationErrors.AlreadyLinked, result.Error);
        _centers.DidNotReceiveWithAnyArgs().AddCenterTeacher(default!);
    }

    [Fact]
    public async Task Handle_WhenPreviouslyRemoved_ReactivatesSameLink()
    {
        var link = CenterTeacher.CreateActive(_center.Id, _tenant.Id, CenterTeacherOrigin.Invitation);
        link.Remove();
        _centers.GetCenterTeacherAsync(_center.Id, _tenant.Id, Arg.Any<CancellationToken>()).Returns(link);

        var result = await CreateHandler().Handle(new LinkTeacherToCenterCommand(_center.Id, _teacherId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(CenterTeacherStatus.Active, link.Status);
        _centers.DidNotReceiveWithAnyArgs().AddCenterTeacher(default!);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
