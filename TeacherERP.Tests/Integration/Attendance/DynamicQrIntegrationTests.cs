using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

// F3: الكود المتغير مشتق من السر بـ HMAC، والكارت الثابت المطبوع يفضل شغال (عبر الـ API الحقيقي)
public class DynamicQrIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DynamicQrIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(SeededTeacher Teacher, string Secret)> SeedTeacherWithModeAsync(AttendanceMode mode)
    {
        SeededTeacher teacher = null!;
        var secret = string.Empty;

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);

            var settings = await db.TenantSettings.IgnoreQueryFilters().SingleAsync(s => s.TenantId == teacher.TenantId);
            settings.UpdateAttendance(mode, rejectMockLocations: false, autoRegisterOnScan: true, autoApproveGroupAdminScans: false);
            await db.SaveChangesAsync();

            secret = settings.QrSecret;
        });

        return (teacher, secret);
    }

    [Fact]
    public async Task DisplayCode_IsAValidDynamicCodeForTheTeacher_AndNotTheStaticSecret()
    {
        var (teacher, secret) = await SeedTeacherWithModeAsync(AttendanceMode.DynamicScreenQR);
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var response = await client.GetAsync("/api/Attendance/display-code");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var code = body.GetProperty("code").GetString()!;
        Assert.Equal(teacher.TenantId, body.GetProperty("teacherId").GetGuid());
        Assert.InRange(body.GetProperty("expiresInSeconds").GetInt32(), 1, 30);

        using var scope = _factory.Services.CreateScope();
        var codes = scope.ServiceProvider.GetRequiredService<IAttendanceCodeService>();
        var now = DateTime.UtcNow;

        Assert.True(codes.IsValid(AttendanceMode.DynamicScreenQR, teacher.TenantId, secret, code, now));
        Assert.NotEqual(secret[..8].ToUpperInvariant(), code.ToUpperInvariant());

        // الكود الثابت (السر) لا يُقبل في الوضع المتغير
        Assert.False(codes.IsValid(AttendanceMode.DynamicScreenQR, teacher.TenantId, secret, secret, now));
    }

    [Fact]
    public async Task DisplayCode_IsRejectedWhenTheModeIsNotDynamic()
    {
        var (teacher, _) = await SeedTeacherWithModeAsync(AttendanceMode.StaticGlobalQR);
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var response = await client.GetAsync("/api/Attendance/display-code");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Attendance.NotDynamicMode", body.GetProperty("errorCode").GetString());
    }

    [Fact]
    public async Task StaticCard_StillWorks_AndIsExposedOnlyInStaticMode()
    {
        var (teacher, secret) = await SeedTeacherWithModeAsync(AttendanceMode.StaticGlobalQR);
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var response = await client.GetAsync("/api/Attendance/settings");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var printed = body.GetProperty("staticCode").GetString()!;
        Assert.Equal(secret, printed);

        using var scope = _factory.Services.CreateScope();
        var codes = scope.ServiceProvider.GetRequiredService<IAttendanceCodeService>();

        Assert.True(codes.IsValid(AttendanceMode.StaticGlobalQR, teacher.TenantId, secret, printed, DateTime.UtcNow));
    }

    [Fact]
    public async Task Settings_HideTheSecretInDynamicMode()
    {
        var (teacher, _) = await SeedTeacherWithModeAsync(AttendanceMode.DynamicScreenQR);
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var response = await client.GetAsync("/api/Attendance/settings");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Null, body.GetProperty("staticCode").ValueKind);
    }
}
