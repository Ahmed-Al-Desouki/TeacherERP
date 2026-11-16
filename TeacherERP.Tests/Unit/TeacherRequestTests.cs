using System.Data;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using TeacherERP.Application.Features.Administration;
using TeacherERP.Application.Features.TeacherRequests;
using TeacherERP.Application.Features.TeacherRequests.Admin;
using TeacherERP.Application.Features.TeacherRequests.Review;
using TeacherERP.Application.Features.TeacherRequests.Submit;
using TeacherERP.Application.Features.TeacherRequests.Verify;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Teachers;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Unit;

// A4: خطوة 1 — إرسال بيانات المدرس وكود الإيميل
public class SubmitTeacherRequestCommandHandlerTests
{
    private readonly IUserAccountService _users = Substitute.For<IUserAccountService>();
    private readonly IOtpService _otp = Substitute.For<IOtpService>();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly IBackgroundJobClient _jobs = Substitute.For<IBackgroundJobClient>();

    public SubmitTeacherRequestCommandHandlerTests()
    {
        _users.CheckEmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Result<bool>.Success(false));
        _otp.GenerateAsync(OtpPurpose.TeacherRequest, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<OtpIssueResult>.Success(new OtpIssueResult("123456", 600, 60)));
    }

    private SubmitTeacherRequestCommandHandler CreateHandler() => new(_users, _otp, _cache, _jobs);

    private static SubmitTeacherRequestCommand Command(string email = " Ali@X.com ")
        => new(" Ali ", email, " Math ", "note");

    [Fact]
    public async Task Handle_NewEmail_CachesNormalizedData_AndQueuesOtpEmail()
    {
        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("ali@x.com", result.Value!.Email);
        Assert.Equal(600, result.Value.ExpiresInSeconds);

        Assert.True(_cache.TryGetValue(TeacherRequestCacheKey.For("ali@x.com"), out CachedTeacherRequestData? data));
        Assert.Equal("Ali", data!.FullName);
        Assert.Equal("Math", data.Subject);
        Assert.Equal("Africa/Cairo", data.TimeZoneId);

        _jobs.Received(1).Create(Arg.Any<Job>(), Arg.Any<IState>());
    }

    [Fact]
    public async Task Handle_ExistingEmail_ReturnsSameSuccess_ButCachesNothingAndSendsNothing()
    {
        _users.CheckEmailExistsAsync("ali@x.com", Arg.Any<CancellationToken>()).Returns(Result<bool>.Success(true));

        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(600, result.Value!.ExpiresInSeconds);
        Assert.False(_cache.TryGetValue(TeacherRequestCacheKey.For("ali@x.com"), out _));
        _jobs.DidNotReceiveWithAnyArgs().Create(default!, default!);
        // الكود يُولَّد في الحالتين حتى لا يختلف سلوك الحدود بين إيميل مسجّل وغير مسجّل
        await _otp.Received(1).GenerateAsync(OtpPurpose.TeacherRequest, "ali@x.com", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOtpIsRejected_ReturnsThatError_AndCachesNothing()
    {
        var error = new Error("Otp.ResendTooSoon", "wait");
        _otp.GenerateAsync(OtpPurpose.TeacherRequest, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<OtpIssueResult>.Failure(error));

        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        Assert.Equal(error, result.Error);
        Assert.False(_cache.TryGetValue(TeacherRequestCacheKey.For("ali@x.com"), out _));
    }
}

// A4: خطوة 2 — تأكيد الكود وإصدار سر الـ QR
public class VerifyTeacherRequestCommandHandlerTests
{
    private static readonly DateTime Now = new(2026, 11, 16, 10, 0, 0, DateTimeKind.Utc);

    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly IOtpService _otp = Substitute.For<IOtpService>();
    private readonly ITeacherAccessRequestRepository _requests = Substitute.For<ITeacherAccessRequestRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public VerifyTeacherRequestCommandHandlerTests()
    {
        _otp.VerifyAsync(OtpPurpose.TeacherRequest, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(true));
        _requests.GetPendingByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new List<TeacherAccessRequest>());

        _unitOfWork.ExecuteInTransactionAsync(
                Arg.Any<Func<Task<bool>>>(), Arg.Any<IsolationLevel>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Func<Task<bool>>>().Invoke());
    }

    private VerifyTeacherRequestCommandHandler CreateHandler()
        => new(_cache, _otp, _requests, _unitOfWork, new FixedTimeProvider(Now));

    private void CacheData() => _cache.Set(
        TeacherRequestCacheKey.For("ali@x.com"),
        new CachedTeacherRequestData { FullName = "Ali", Email = "ali@x.com", Subject = "Math", TimeZoneId = "Africa/Cairo" });

    [Fact]
    public async Task Handle_WithoutPendingData_Fails_AndDoesNotConsumeTheCode()
    {
        var result = await CreateHandler().Handle(new VerifyTeacherRequestCommand("ali@x.com", "123456"), CancellationToken.None);

        Assert.Equal(TeacherRequestErrors.SubmissionExpired, result.Error);
        await _otp.DidNotReceiveWithAnyArgs().VerifyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Handle_WhenCodeIsWrong_ReturnsOtpError_AndSavesNothing()
    {
        CacheData();
        var error = new Error("Otp.Invalid", "invalid");
        _otp.VerifyAsync(OtpPurpose.TeacherRequest, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Failure(error));

        var result = await CreateHandler().Handle(new VerifyTeacherRequestCommand("ali@x.com", "000000"), CancellationToken.None);

        Assert.Equal(error, result.Error);
        _requests.DidNotReceiveWithAnyArgs().Add(default!);
    }

    [Fact]
    public async Task Handle_Success_StoresOnlyTheHash_ReturnsToken_AndClearsCache()
    {
        CacheData();
        TeacherAccessRequest? saved = null;
        _requests.When(r => r.Add(Arg.Any<TeacherAccessRequest>())).Do(call => saved = call.Arg<TeacherAccessRequest>());

        var result = await CreateHandler().Handle(new VerifyTeacherRequestCommand(" Ali@X.com ", "123456"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(saved);
        Assert.NotEqual(result.Value!.Token, saved!.TokenHash);
        Assert.Equal(TeacherRequestToken.Hash(result.Value.Token), saved.TokenHash);
        Assert.Equal(TeacherAccessRequestStatus.Pending, saved.Status);
        Assert.Equal(Now.Add(TeacherRequestPolicy.RequestLifetime), result.Value.ExpiresAtUtc);
        Assert.Equal(result.Value.ExpiresAtUtc, saved.ExpiresAtUtc);
        Assert.False(_cache.TryGetValue(TeacherRequestCacheKey.For("ali@x.com"), out _));
    }

    [Fact]
    public async Task Handle_Success_ExpiresPreviousPendingRequestOfTheSameEmail()
    {
        CacheData();
        var old = TeacherAccessRequest.Create("Ali", "ali@x.com", "Math", null, "UTC", "hash-old", Now.AddHours(10));
        _requests.GetPendingByEmailAsync("ali@x.com", Arg.Any<CancellationToken>()).Returns(new List<TeacherAccessRequest> { old });

        var result = await CreateHandler().Handle(new VerifyTeacherRequestCommand("ali@x.com", "123456"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherAccessRequestStatus.Expired, old.Status);
        // حفظ الإنهاء ثم حفظ الجديد (منفصلان حتى لا يتعارض الفهرس الفريد المرشَّح)
        await _unitOfWork.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

// A4: القبول والرفض (مسار المدرس ومسار مدير النظام)
public class TeacherRequestReviewServiceTests
{
    private static readonly DateTime Now = new(2026, 11, 16, 10, 0, 0, DateTimeKind.Utc);

    private readonly ITeacherAccessRequestRepository _requests = Substitute.For<ITeacherAccessRequestRepository>();
    private readonly ITeacherProvisioningService _provisioning = Substitute.For<ITeacherProvisioningService>();
    private readonly ITenantRepository _tenants = Substitute.For<ITenantRepository>();
    private readonly IAuditRepository _audit = Substitute.For<IAuditRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Guid _reviewerId = Guid.NewGuid();
    private readonly Tenant _reviewerTenant;
    private readonly Guid _newUserId = Guid.NewGuid();
    private readonly Tenant _newTenant;

    public TeacherRequestReviewServiceTests()
    {
        _reviewerTenant = Tenant.Create("Reviewer", _reviewerId, "UTC");
        _newTenant = Tenant.Create("Ali", _newUserId, "UTC");

        _tenants.GetByOwnerIdAsync(_reviewerId, Arg.Any<CancellationToken>()).Returns(_reviewerTenant);
        _tenants.GetByOwnerIdAsync(_newUserId, Arg.Any<CancellationToken>()).Returns(_newTenant);

        _provisioning.ProvisionAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreatedTeacherDto>.Success(new CreatedTeacherDto(_newUserId, _newTenant.Id, "Ali", "ali@x.com", false)));

        // المحاكاة تنفّذ العملية مباشرة (الاستثناء داخلها يخرج من الـ Transaction كما في الحقيقي)
        _unitOfWork.ExecuteInTransactionAsync(
                Arg.Any<Func<Task<Result<CreatedTeacherDto>>>>(), Arg.Any<IsolationLevel>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Func<Task<Result<CreatedTeacherDto>>>>().Invoke());
    }

    private TeacherRequestReviewService CreateService()
        => new(_requests, _provisioning, _tenants, _audit, _unitOfWork, new FixedTimeProvider(Now));

    private static TeacherAccessRequest PendingRequest(DateTime? expiresAt = null)
        => TeacherAccessRequest.Create("Ali", "ali@x.com", "Math", null, "Africa/Cairo", "hash", expiresAt ?? Now.AddHours(5));

    [Fact]
    public async Task Approve_ByTeacher_CreatesAccountThroughProvisioning_SetsSubject_AndAudits()
    {
        var request = PendingRequest();

        var result = await CreateService().ApproveAsync(request, _reviewerId, ReviewerKind.Teacher, CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _provisioning.Received(1).ProvisionAsync("Ali", "ali@x.com", "Africa/Cairo", Arg.Any<CancellationToken>());
        Assert.Equal("Math", _newTenant.Subject);
        Assert.Equal(TeacherAccessRequestStatus.Approved, request.Status);
        Assert.Equal(_reviewerId, request.ReviewedByUserId);
        Assert.Equal(_newUserId, request.CreatedUserId);
        _audit.Received(1).Add(Arg.Is<AuditLog>(a =>
            a.TenantId == _reviewerTenant.Id
            && a.ActorUserId == _reviewerId
            && a.TargetUserId == _newUserId
            && a.Action == AuditAction.TeacherAdmitted));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Approve_RunsProvisioningAndRequestUpdateInOneTransaction()
    {
        var result = await CreateService().ApproveAsync(PendingRequest(), _reviewerId, ReviewerKind.Teacher, CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _unitOfWork.Received(1).ExecuteInTransactionAsync(
            Arg.Any<Func<Task<Result<CreatedTeacherDto>>>>(), IsolationLevel.ReadCommitted, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Approve_ByTeacher_WhenQuotaReached_Fails_AndCreatesNothing()
    {
        _requests.CountApprovedByReviewerSinceAsync(_reviewerId, Now.AddDays(-TeacherRequestPolicy.ApprovalWindowDays), Arg.Any<CancellationToken>())
            .Returns(TeacherRequestPolicy.MaxApprovalsPerWindow);
        var request = PendingRequest();

        var result = await CreateService().ApproveAsync(request, _reviewerId, ReviewerKind.Teacher, CancellationToken.None);

        Assert.Equal(TeacherRequestErrors.QuotaReached, result.Error);
        await _provisioning.DidNotReceiveWithAnyArgs().ProvisionAsync(default!, default!, default!, default);
        Assert.Equal(TeacherAccessRequestStatus.Pending, request.Status);
    }

    [Fact]
    public async Task Approve_ByTeacher_WhenReviewerHasNoTeacherSpace_Fails()
    {
        var stranger = Guid.NewGuid();
        _tenants.GetByOwnerIdAsync(stranger, Arg.Any<CancellationToken>()).Returns((Tenant?)null);

        var result = await CreateService().ApproveAsync(PendingRequest(), stranger, ReviewerKind.Teacher, CancellationToken.None);

        Assert.Equal(TeacherRequestErrors.ReviewerNotAllowed, result.Error);
        await _provisioning.DidNotReceiveWithAnyArgs().ProvisionAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Approve_ByAdministrator_SkipsQuotaAndAudit()
    {
        var admin = Guid.NewGuid();
        var request = PendingRequest();

        var result = await CreateService().ApproveAsync(request, admin, ReviewerKind.Administrator, CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _requests.DidNotReceiveWithAnyArgs().CountApprovedByReviewerSinceAsync(default, default, default);
        _audit.DidNotReceiveWithAnyArgs().Add(default!);
        Assert.Equal(admin, request.ReviewedByUserId);
    }

    [Fact]
    public async Task Approve_WhenExpired_Fails_AndCreatesNothing()
    {
        var result = await CreateService()
            .ApproveAsync(PendingRequest(Now.AddMinutes(-1)), _reviewerId, ReviewerKind.Teacher, CancellationToken.None);

        Assert.Equal(TeacherRequestErrors.Expired, result.Error);
        await _provisioning.DidNotReceiveWithAnyArgs().ProvisionAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Approve_WhenAlreadyReviewed_Fails()
    {
        var request = PendingRequest();
        request.Reject(Guid.NewGuid(), Now, "no");

        var result = await CreateService().ApproveAsync(request, _reviewerId, ReviewerKind.Teacher, CancellationToken.None);

        Assert.Equal(TeacherRequestErrors.NotPending, result.Error);
    }

    [Fact]
    public async Task Approve_WhenProvisioningFails_KeepsRequestPending_AndWritesNoAudit()
    {
        _provisioning.ProvisionAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreatedTeacherDto>.Failure(AdministrationErrors.EmailInUse));
        var request = PendingRequest();

        var result = await CreateService().ApproveAsync(request, _reviewerId, ReviewerKind.Teacher, CancellationToken.None);

        Assert.Equal(AdministrationErrors.EmailInUse, result.Error);
        Assert.Equal(TeacherAccessRequestStatus.Pending, request.Status);
        _audit.DidNotReceiveWithAnyArgs().Add(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Reject_ByTeacher_StoresReason_AndAudits()
    {
        var request = PendingRequest();

        var result = await CreateService().RejectAsync(request, _reviewerId, ReviewerKind.Teacher, "  not a teacher  ", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherAccessRequestStatus.Rejected, request.Status);
        Assert.Equal("not a teacher", request.DecisionNote);
        _audit.Received(1).Add(Arg.Is<AuditLog>(a =>
            a.Action == AuditAction.TeacherRequestRejected && a.TenantId == _reviewerTenant.Id && a.TargetUserId == null));
        await _provisioning.DidNotReceiveWithAnyArgs().ProvisionAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Reject_ByAdministrator_DoesNotAudit()
    {
        var request = PendingRequest();

        var result = await CreateService().RejectAsync(request, Guid.NewGuid(), ReviewerKind.Administrator, null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _audit.DidNotReceiveWithAnyArgs().Add(default!);
    }
}

// A4: الكيان والسر والمدخلات
public class TeacherRequestBasicsTests
{
    private static readonly DateTime Now = new(2026, 11, 16, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Token_IsRandom_AndHashIsStable()
    {
        var first = TeacherRequestToken.Generate();
        var second = TeacherRequestToken.Generate();

        Assert.NotEqual(first, second);
        Assert.True(first.Length >= 43);
        Assert.Equal(TeacherRequestToken.Hash(first), TeacherRequestToken.Hash($" {first} "));
        Assert.Equal(64, TeacherRequestToken.Hash(first).Length);
    }

    [Fact]
    public void EffectiveStatus_TreatsPendingPastExpiryAsExpired()
    {
        var request = TeacherAccessRequest.Create("Ali", "Ali@X.com", "Math", " ", "UTC", "hash", Now.AddHours(1));

        Assert.Equal("ali@x.com", request.Email);
        Assert.Null(request.Note);
        Assert.Equal(TeacherAccessRequestStatus.Pending, request.GetEffectiveStatus(Now));
        Assert.Equal(TeacherAccessRequestStatus.Expired, request.GetEffectiveStatus(Now.AddHours(1)));
    }

    [Fact]
    public void SubmitValidator_RequiresSubject_AndValidEmailAndTimeZone()
    {
        var validator = new SubmitTeacherRequestValidator();

        Assert.True(validator.Validate(new SubmitTeacherRequestCommand("Ali", "ali@x.com", "Math")).IsValid);
        Assert.False(validator.Validate(new SubmitTeacherRequestCommand("Ali", "ali@x.com", "")).IsValid);
        Assert.False(validator.Validate(new SubmitTeacherRequestCommand("Ali", "bad", "Math")).IsValid);
        Assert.False(validator.Validate(new SubmitTeacherRequestCommand("Ali", "ali@x.com", "Math", null, "Mars/Base")).IsValid);
        Assert.False(validator.Validate(new SubmitTeacherRequestCommand("Ali", "ali@x.com", "Math", new string('n', 301))).IsValid);
    }

    [Fact]
    public void VerifyValidator_RequiresSixDigitCode()
    {
        var validator = new VerifyTeacherRequestValidator();

        Assert.True(validator.Validate(new VerifyTeacherRequestCommand("ali@x.com", "012345")).IsValid);
        Assert.False(validator.Validate(new VerifyTeacherRequestCommand("ali@x.com", "12345")).IsValid);
        Assert.False(validator.Validate(new VerifyTeacherRequestCommand("ali@x.com", "abcdef")).IsValid);
    }

    [Fact]
    public void ReviewValidators_RequireToken_AndLimitReason()
    {
        Assert.False(new ApproveTeacherRequestValidator().Validate(new ApproveTeacherRequestCommand("")).IsValid);
        Assert.False(new PreviewTeacherRequestValidator().Validate(new PreviewTeacherRequestQuery("")).IsValid);
        Assert.False(new RejectTeacherRequestValidator().Validate(new RejectTeacherRequestCommand("t", new string('r', 301))).IsValid);
        Assert.True(new RejectTeacherRequestValidator().Validate(new RejectTeacherRequestCommand("t", null)).IsValid);
    }
}
