using NSubstitute;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Attendance;
using TeacherERP.Application.Features.Enrollment.ApproveRequest;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Unit;

// F5 (1): موقع مسح الـ Walk-in يُحفظ في الطلب المعلّق (أو في سجل الحضور مباشرة لو التسجيل فوري)
public class WalkInLocationRecorderTests
{
    private static readonly DateTime Now = new(2026, 11, 14, 10, 0, 0, DateTimeKind.Utc);
    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid StudentId = Guid.NewGuid();

    private const double Lat = 30.0444;
    private const double Lon = 31.2357;

    private readonly IAttendanceRepository _attendance = Substitute.For<IAttendanceRepository>();
    private readonly IEnrollmentRepository _enrollment = Substitute.For<IEnrollmentRepository>();
    private readonly IEnrollmentService _enrollmentService = Substitute.For<IEnrollmentService>();
    private readonly INotificationService _notifications = Substitute.For<INotificationService>();
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IFinancialLedgerService _financialLedger = Substitute.For<IFinancialLedgerService>();

    private readonly RunningSessionInfo _running = new(
        Guid.NewGuid(), Guid.NewGuid(), "Math A", Now.AddMinutes(-5), Now.AddMinutes(55), null, null, 100);

    private AttendanceRecorder CreateRecorder()
        => new(_attendance, _enrollment, _enrollmentService, _notifications, _users, _unitOfWork,
            new FixedTimeProvider(Now), _financialLedger, Substitute.For<ILogger<AttendanceRecorder>>());

    public WalkInLocationRecorderTests()
    {
        _attendance
            .GetRunningSessionsAsync(TenantId, Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(new List<RunningSessionInfo> { _running });
        _financialLedger.RecordPresentAttendanceAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(),
            Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    }

    private static AttendanceRequest Request(ScanMethod method, bool registerImmediately, Guid? recordedBy = null)
        => new(TenantId, StudentId, method, Lat, Lon, recordedBy, null, false, registerImmediately, null);

    [Fact]
    public async Task PendingWalkIn_StoresScanLocationAndSourceOnTheRequest()
    {
        EnrollmentRequest? captured = null;
        _enrollment
            .When(r => r.AddRequest(Arg.Any<EnrollmentRequest>()))
            .Do(call => captured = call.Arg<EnrollmentRequest>());

        var result = await CreateRecorder().RecordAsync(Request(ScanMethod.SelfScan, registerImmediately: false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(AttendanceOutcome.PendingApproval, result.Value!.Outcome);

        Assert.NotNull(captured);
        Assert.Equal(Lat, captured!.SourceLatitude);
        Assert.Equal(Lon, captured.SourceLongitude);
        Assert.Equal(_running.SessionId, captured.SourceSessionId);
        Assert.Equal(_running.GroupId, captured.GroupId);
        Assert.Equal(StudentId, captured.StudentId);
        Assert.Equal(ScanMethod.SelfScan, captured.SourceScanMethod);
        Assert.Equal(EnrollmentRequestStatus.Pending, captured.Status);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PendingWalkIn_ByGroupAdmin_KeepsTheScannerAndMethod()
    {
        var groupAdminId = Guid.NewGuid();
        EnrollmentRequest? captured = null;
        _enrollment
            .When(r => r.AddRequest(Arg.Any<EnrollmentRequest>()))
            .Do(call => captured = call.Arg<EnrollmentRequest>());

        await CreateRecorder().RecordAsync(Request(ScanMethod.AdminScan, registerImmediately: false, groupAdminId), CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(ScanMethod.AdminScan, captured!.SourceScanMethod);
        Assert.Equal(groupAdminId, captured.SourceRecordedByUserId);
        Assert.Equal(Lat, captured.SourceLatitude);
        Assert.Equal(Lon, captured.SourceLongitude);
    }

    [Fact]
    public async Task PendingWalkIn_WhenStudentAlreadyHasAPendingRequest_DoesNotAddAnother()
    {
        _enrollment.HasPendingRequestAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await CreateRecorder().RecordAsync(Request(ScanMethod.SelfScan, registerImmediately: false), CancellationToken.None);

        Assert.Equal(AttendanceOutcome.PendingApproval, result.Value!.Outcome);
        _enrollment.DidNotReceive().AddRequest(Arg.Any<EnrollmentRequest>());
    }

    [Fact]
    public async Task ImmediateWalkIn_WritesTheLocationOnTheAttendanceRecord()
    {
        _enrollmentService
            .ApproveAsync(Arg.Any<EnrollmentRequest>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(true));

        AttendanceRecord? record = null;
        _attendance
            .When(a => a.AddRecord(Arg.Any<AttendanceRecord>()))
            .Do(call => record = call.Arg<AttendanceRecord>());

        var result = await CreateRecorder().RecordAsync(Request(ScanMethod.SelfScan, registerImmediately: true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(AttendanceOutcome.Recorded, result.Value!.Outcome);
        Assert.NotNull(record);
        Assert.Equal(Lat, record!.ScannedLatitude);
        Assert.Equal(Lon, record.ScannedLongitude);
        Assert.Equal(AttendanceStatus.Present, record.Status);
    }
}

// F5 (2): الموافقة على طلب الـ Walk-in تنقل موقع المسح الأصلي لسجل الحضور
public class WalkInApprovalTests
{
    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid StudentId = Guid.NewGuid();
    private static readonly Guid ApproverId = Guid.NewGuid();

    private const double Lat = 30.0444;
    private const double Lon = 31.2357;

    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ICurrentTenantAccessor _tenant = Substitute.For<ICurrentTenantAccessor>();
    private readonly IEnrollmentRepository _repository = Substitute.For<IEnrollmentRepository>();
    private readonly IEnrollmentService _enrollmentService = Substitute.For<IEnrollmentService>();
    private readonly IGroupSessionRepository _sessions = Substitute.For<IGroupSessionRepository>();
    private readonly IAttendanceRepository _attendance = Substitute.For<IAttendanceRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IFinancialLedgerService _financialLedger = Substitute.For<IFinancialLedgerService>();

    private AttendanceRecord? _recorded;

    public WalkInApprovalTests()
    {
        _currentUser.UserId.Returns(ApproverId);
        _tenant.Current.Returns(new TenantContext(TenantId, TenantRole.Owner, MembershipStatus.Active));
        _enrollmentService
            .ApproveAsync(Arg.Any<EnrollmentRequest>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(true));
        _attendance
            .When(a => a.AddRecord(Arg.Any<AttendanceRecord>()))
            .Do(call => _recorded = call.Arg<AttendanceRecord>());
        _financialLedger.RecordPresentAttendanceAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(),
            Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    }

    private ApproveRequestCommandHandler CreateHandler()
        => new(_currentUser, _tenant, _repository, _enrollmentService, _sessions, _attendance, _financialLedger, _unitOfWork);

    private Session ExistingSession()
    {
        var session = Session.Create(
            TenantId, Guid.NewGuid(),
            new DateTime(2026, 11, 14, 9, 55, 0, DateTimeKind.Utc),
            new DateTime(2026, 11, 14, 10, 55, 0, DateTimeKind.Utc));
        _sessions.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        return session;
    }

    private EnrollmentRequest PendingRequest(
        Session session, ScanMethod? method, Guid? recordedBy, double? lat, double? lon)
    {
        var request = EnrollmentRequest.Create(
            TenantId, StudentId, session.GroupId, EnrollmentRequestType.Join,
            session.Id, method, recordedBy, lat, lon);
        _repository.GetTrackedRequestAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        return request;
    }

    [Fact]
    public async Task Approve_CopiesTheScanLocationToTheAttendanceRecord()
    {
        var session = ExistingSession();
        var request = PendingRequest(session, ScanMethod.SelfScan, null, Lat, Lon);

        var result = await CreateHandler().Handle(new ApproveRequestCommand(request.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(_recorded);
        Assert.Equal(Lat, _recorded!.ScannedLatitude);
        Assert.Equal(Lon, _recorded.ScannedLongitude);
        Assert.Equal(session.Id, _recorded.SessionId);
        Assert.Equal(StudentId, _recorded.StudentId);
        Assert.Equal(AttendanceStatus.Present, _recorded.Status);
        Assert.Equal(ScanMethod.SelfScan, _recorded.ScannedVia);
        // مسح ذاتي: الموافِق هو المسؤول عن التسجيل
        Assert.Equal(ApproverId, _recorded.RecordedByUserId);
    }

    [Fact]
    public async Task Approve_KeepsTheOriginalScannerWhenAGroupAdminScanned()
    {
        var groupAdminId = Guid.NewGuid();
        var session = ExistingSession();
        var request = PendingRequest(session, ScanMethod.AdminScan, groupAdminId, Lat, Lon);

        await CreateHandler().Handle(new ApproveRequestCommand(request.Id), CancellationToken.None);

        Assert.NotNull(_recorded);
        Assert.Equal(ScanMethod.AdminScan, _recorded!.ScannedVia);
        Assert.Equal(groupAdminId, _recorded.RecordedByUserId);
        Assert.Equal(Lat, _recorded.ScannedLatitude);
        Assert.Equal(Lon, _recorded.ScannedLongitude);
    }

    [Fact]
    public async Task Approve_LegacyRequestWithoutLocation_LeavesTheLocationEmpty()
    {
        var session = ExistingSession();
        var request = PendingRequest(session, null, null, null, null);

        var result = await CreateHandler().Handle(new ApproveRequestCommand(request.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(_recorded);
        Assert.Null(_recorded!.ScannedLatitude);
        Assert.Null(_recorded.ScannedLongitude);
        Assert.Equal(ScanMethod.SelfScan, _recorded.ScannedVia);
    }

    [Fact]
    public async Task Approve_RequestWithoutSourceSession_RecordsNoAttendance()
    {
        var request = EnrollmentRequest.Create(TenantId, StudentId, Guid.NewGuid(), EnrollmentRequestType.Join);
        _repository.GetTrackedRequestAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var result = await CreateHandler().Handle(new ApproveRequestCommand(request.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(_recorded);
    }

    [Fact]
    public async Task Approve_CancelledSourceSession_RecordsNoAttendance()
    {
        var session = ExistingSession();
        session.Cancel("holiday");
        var request = PendingRequest(session, ScanMethod.SelfScan, null, Lat, Lon);

        var result = await CreateHandler().Handle(new ApproveRequestCommand(request.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(_recorded);
    }

    [Fact]
    public async Task Approve_WhenARecordAlreadyExists_DoesNotDuplicateIt()
    {
        var session = ExistingSession();
        var request = PendingRequest(session, ScanMethod.SelfScan, null, Lat, Lon);
        _attendance
            .GetRecordAsync(session.Id, StudentId, Arg.Any<CancellationToken>())
            .Returns(AttendanceRecord.RecordPresence(TenantId, session.Id, StudentId, ScanMethod.SelfScan, null, null));

        var result = await CreateHandler().Handle(new ApproveRequestCommand(request.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(_recorded);
    }
}
