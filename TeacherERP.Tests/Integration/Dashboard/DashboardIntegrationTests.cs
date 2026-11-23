using System.Net;
using System.Net.Http.Json;
using TeacherERP.Application.Features.Dashboards;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class DashboardIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DashboardIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task TeacherDashboard_ReturnsTenantScopedOperationalAndFinancialSummary()
    {
        SeededTeacher firstTeacher = default!;
        SeededTeacher secondTeacher = default!;
        Guid firstStudentId = Guid.Empty;

        await _factory.SeedAsync(async db =>
        {
            firstTeacher = await TestData.CreateTeacherAsync(db);
            secondTeacher = await TestData.CreateTeacherAsync(db);
            firstStudentId = await TestData.AddStudentAsync(db, firstTeacher.TenantId);
            db.EnrollmentRequests.Add(EnrollmentRequest.Create(
                firstTeacher.TenantId, firstStudentId, firstTeacher.GroupId, EnrollmentRequestType.Join));
            await db.SaveChangesAsync();
            await TestData.AddSessionAsync(db, firstTeacher.TenantId, firstTeacher.GroupId,
                DateTime.UtcNow.Date.AddHours(12));
        });

        using var firstTeacherClient = _factory.CreateClientFor(firstTeacher.OwnerId, firstTeacher.TenantId);
        using var secondTeacherClient = _factory.CreateClientFor(secondTeacher.OwnerId, secondTeacher.TenantId);
        var firstResponse = await firstTeacherClient.GetAsync("/api/dashboard/teacher");
        var secondResponse = await secondTeacherClient.GetAsync("/api/dashboard/teacher");

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        var firstDashboard = await firstResponse.Content.ReadFromJsonAsync<TeacherDashboardDto>();
        var secondDashboard = await secondResponse.Content.ReadFromJsonAsync<TeacherDashboardDto>();

        Assert.NotNull(firstDashboard);
        Assert.Equal(1, firstDashboard.ActiveGroups);
        Assert.Equal(1, firstDashboard.ActiveStudents);
        Assert.Equal(1, firstDashboard.SessionsToday);
        Assert.Equal(1, firstDashboard.PendingEnrollmentRequests);
        Assert.InRange(firstDashboard.UpcomingSessions.Count, 0, 5);
        Assert.NotNull(secondDashboard);
        Assert.Equal(1, secondDashboard.ActiveGroups);
        Assert.Equal(0, secondDashboard.ActiveStudents);
        Assert.Equal(0, secondDashboard.PendingEnrollmentRequests);
    }

    [Fact]
    public async Task AdministratorDashboard_IsRestrictedToAdministratorAndReturnsPlatformSummary()
    {
        SeededTeacher teacher = default!;
        await _factory.SeedAsync(async db => teacher = await TestData.CreateTeacherAsync(db));

        using var teacherClient = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        using var studentClient = _factory.CreateClientFor(Guid.NewGuid(), role: "Student");
        using var adminClient = _factory.CreateClientFor(Guid.NewGuid(), role: "Administrator");
        using var anonymousClient = _factory.CreateClient();

        Assert.Equal(HttpStatusCode.Unauthorized,
            (await anonymousClient.GetAsync("/api/dashboard/teacher")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await anonymousClient.GetAsync("/api/dashboard/administrator")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,
            (await teacherClient.GetAsync("/api/dashboard/administrator")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,
            (await studentClient.GetAsync("/api/dashboard/teacher")).StatusCode);

        var response = await adminClient.GetAsync("/api/dashboard/administrator");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dashboard = await response.Content.ReadFromJsonAsync<AdministratorDashboardDto>();
        Assert.NotNull(dashboard);
        Assert.True(dashboard.ActiveTeachers >= 1);
        Assert.True(dashboard.Centers >= 1);
    }
}
