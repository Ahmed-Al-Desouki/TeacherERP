using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

// F5: موقع مسح الـ Walk-in يتحفظ في الطلب (عمود الـ Migration فعلاً في الداتابيز) وينتقل لسجل الحضور عند الموافقة
public class WalkInLocationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private const double Lat = 30.0444;
    private const double Lon = 31.2357;

    private readonly CustomWebApplicationFactory _factory;

    public WalkInLocationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private sealed record Scenario(SeededTeacher Teacher, Guid StudentId, Guid RunningSessionId);

    // مدرس وحصة جارية الآن، وطالب غير مسجّل عنده (Walk-in)
    private async Task<Scenario> SeedWalkInAsync()
    {
        Scenario? scenario = null;

        await _factory.SeedAsync(async db =>
        {
            var teacher = await TestData.CreateTeacherAsync(db);
            var studentId = await TestData.AddStudentAsync(db, teacher.TenantId, enrolledGroupId: null, withMembership: false);
            var sessionId = await TestData.AddSessionAsync(
                db, teacher.TenantId, teacher.GroupId, DateTime.UtcNow.AddMinutes(-5), TimeSpan.FromHours(1));

            scenario = new Scenario(teacher, studentId, sessionId);
        });

        return scenario!;
    }

    // الطالب يمسح الكود بنفسه: بنستدعي مسجّل الحضور كما يفعل الـ Handler (التحقق من الكود مغطى في اختبارات أخرى)
    private async Task<AttendanceResultDto> ScanAsWalkInAsync(Scenario s, ScanMethod method = ScanMethod.SelfScan, Guid? recordedBy = null)
    {
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        services.GetRequiredService<ICurrentTenantAccessor>()
            .Set(new TenantContext(s.Teacher.TenantId, TenantRole.Student, MembershipStatus.Active));

        var recorder = services.GetRequiredService<IAttendanceRecorder>();
        var request = new AttendanceRequest(
            s.Teacher.TenantId, s.StudentId, method, Lat, Lon, recordedBy,
            PreferredGroupId: null, EnforceGeofence: false, RegisterImmediately: false, RestrictToGroupId: null);

        var result = await recorder.RecordAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsFailure ? result.Error.Code : null);
        return result.Value!;
    }

    private Task<EnrollmentRequest> GetPendingRequestAsync(Scenario s)
        => _factory.QueryAsync(db => db.EnrollmentRequests.IgnoreQueryFilters()
            .SingleAsync(r => r.TenantId == s.Teacher.TenantId && r.StudentId == s.StudentId));

    private Task<AttendanceRecord?> GetAttendanceAsync(Scenario s)
        => _factory.QueryAsync(db => db.AttendanceRecords.IgnoreQueryFilters()
            .SingleOrDefaultAsync(a => a.SessionId == s.RunningSessionId && a.StudentId == s.StudentId));

    [Fact]
    public async Task WalkInScan_PersistsTheLocationOnThePendingRequest()
    {
        var s = await SeedWalkInAsync();

        var outcome = await ScanAsWalkInAsync(s);

        Assert.Equal(AttendanceOutcome.PendingApproval, outcome.Outcome);

        var request = await GetPendingRequestAsync(s);
        Assert.Equal(EnrollmentRequestStatus.Pending, request.Status);
        Assert.Equal(s.RunningSessionId, request.SourceSessionId);
        Assert.Equal(ScanMethod.SelfScan, request.SourceScanMethod);
        Assert.Equal(Lat, request.SourceLatitude);
        Assert.Equal(Lon, request.SourceLongitude);

        // لسه ما اتسجّل له حضور قبل الموافقة
        Assert.Null(await GetAttendanceAsync(s));
    }

    [Fact]
    public async Task ApprovingTheRequest_MovesTheLocationToTheAttendanceRecord()
    {
        var s = await SeedWalkInAsync();
        await ScanAsWalkInAsync(s);
        var request = await GetPendingRequestAsync(s);

        var client = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);
        var response = await client.PostAsync($"/api/Enrollment/requests/{request.Id}/approve", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var attendance = await GetAttendanceAsync(s);
        Assert.NotNull(attendance);
        Assert.Equal(AttendanceStatus.Present, attendance!.Status);
        Assert.Equal(ScanMethod.SelfScan, attendance.ScannedVia);
        Assert.Equal(Lat, attendance.ScannedLatitude);
        Assert.Equal(Lon, attendance.ScannedLongitude);

        // الطالب اتسجّل فعلاً في المجموعة (ومن هنا يبدأ تتبع غيابه)
        var enrolled = await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters()
            .AnyAsync(e => e.TenantId == s.Teacher.TenantId && e.StudentId == s.StudentId && e.GroupId == s.Teacher.GroupId));
        Assert.True(enrolled);
    }

    [Fact]
    public async Task ApprovingALegacyRequestWithoutLocation_LeavesTheLocationEmpty()
    {
        var s = await SeedWalkInAsync();
        Guid requestId = Guid.Empty;

        await _factory.SeedAsync(async db =>
        {
            // طلب قديم (قبل حفظ الموقع): فيه الحصة والطريقة فقط
            var legacy = EnrollmentRequest.Create(
                s.Teacher.TenantId, s.StudentId, s.Teacher.GroupId, EnrollmentRequestType.Join,
                s.RunningSessionId, ScanMethod.SelfScan, null);
            db.EnrollmentRequests.Add(legacy);
            await db.SaveChangesAsync();
            requestId = legacy.Id;
        });

        var client = _factory.CreateClientFor(s.Teacher.OwnerId, s.Teacher.TenantId);
        var response = await client.PostAsync($"/api/Enrollment/requests/{requestId}/approve", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var attendance = await GetAttendanceAsync(s);
        Assert.NotNull(attendance);
        Assert.Null(attendance!.ScannedLatitude);
        Assert.Null(attendance.ScannedLongitude);
    }
}
