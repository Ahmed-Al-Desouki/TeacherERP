using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Tests.Integration;

// A3: مدير النظام (دور Administrator) يضيف السناتر ويربط المدرسين بها، وحساب واحد فقط
public class AdministrationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AdministrationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient AdministratorClient() => _factory.CreateClientFor(Guid.NewGuid(), role: RoleNames.Administrator);

    private static IConfiguration AdministratorConfig(string email) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AdministratorSettings:Email"] = email,
            ["AdministratorSettings:Password"] = "Str0ng#Pass1",
            ["AdministratorSettings:FullName"] = "System Administrator"
        })
        .Build();

    // ---------- الصلاحيات ----------

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Assistant")]
    [InlineData("Student")]
    public async Task AdministrationEndpoints_RejectEveryRoleExceptAdministrator(string role)
    {
        var client = _factory.CreateClientFor(Guid.NewGuid(), role: role);

        var createCenter = await client.PostAsJsonAsync("api/Administration/centers", new { name = "X", ownerUserId = Guid.NewGuid() });
        var createTeacher = await client.PostAsJsonAsync("api/Administration/teachers", new { fullName = "Ali", email = "ali@x.com" });
        var link = await client.PostAsJsonAsync($"api/Administration/centers/{Guid.NewGuid()}/teachers", new { teacherUserId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.Forbidden, createCenter.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, createTeacher.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, link.StatusCode);
    }

    [Fact]
    public async Task AdministrationEndpoints_RejectAnonymousCallers()
    {
        var response = await _factory.CreateClient()
            .PostAsJsonAsync("api/Administration/centers", new { name = "X", ownerUserId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ---------- إضافة سنتر ----------

    [Fact]
    public async Task CreateCenter_ForExistingTeacher_CreatesCenterWithActiveFounderLink()
    {
        var teacher = await SeedTeacherAsync();
        var name = $"Al-Amal {Guid.NewGuid():N}";

        var response = await AdministratorClient()
            .PostAsJsonAsync("api/Administration/centers", new { name, ownerUserId = teacher.OwnerId, address = "New Cairo", latitude = 30.1, longitude = 31.5, geofenceRadiusMeters = 50 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var link = await _factory.QueryAsync(db => db.CenterTeachers
            .IgnoreQueryFilters()
            .Where(l => l.Center.Name == name)
            .SingleAsync());

        Assert.Equal(teacher.TenantId, link.TenantId);
        Assert.Equal(CenterTeacherStatus.Active, link.Status);
        Assert.Equal(CenterTeacherOrigin.Founder, link.Origin);
    }

    [Fact]
    public async Task CreateCenter_WhenOwnerIsNotATeacher_ReturnsBadRequest()
    {
        var response = await AdministratorClient()
            .PostAsJsonAsync("api/Administration/centers", new { name = "Ghost Center", ownerUserId = Guid.NewGuid(), latitude = 30.1, longitude = 31.5 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Administration.TeacherNotFound", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CreateCenter_WithEmptyName_ReturnsBadRequest()
    {
        var response = await AdministratorClient()
            .PostAsJsonAsync("api/Administration/centers", new { name = "", ownerUserId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------- ربط مدرس بسنتر ----------

    [Fact]
    public async Task LinkTeacher_AddsActiveAdministratorLink_ThenRejectsDuplicate()
    {
        var centerOwner = await SeedTeacherAsync();
        var otherTeacher = await SeedTeacherAsync();
        var client = AdministratorClient();
        var url = $"api/Administration/centers/{centerOwner.CenterId}/teachers";

        var first = await client.PostAsJsonAsync(url, new { teacherUserId = otherTeacher.OwnerId });
        var second = await client.PostAsJsonAsync(url, new { teacherUserId = otherTeacher.OwnerId });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
        Assert.Contains("Administration.AlreadyLinked", await second.Content.ReadAsStringAsync());

        var link = await _factory.QueryAsync(db => db.CenterTeachers
            .IgnoreQueryFilters()
            .Where(l => l.CenterId == centerOwner.CenterId && l.TenantId == otherTeacher.TenantId)
            .SingleAsync());

        Assert.Equal(CenterTeacherStatus.Active, link.Status);
        Assert.Equal(CenterTeacherOrigin.Administrator, link.Origin);
    }

    [Fact]
    public async Task LinkTeacher_WhenCenterMissing_ReturnsBadRequest()
    {
        var teacher = await SeedTeacherAsync();

        var response = await AdministratorClient()
            .PostAsJsonAsync($"api/Administration/centers/{Guid.NewGuid()}/teachers", new { teacherUserId = teacher.OwnerId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Administration.CenterNotFound", await response.Content.ReadAsStringAsync());
    }

    // ---------- حساب واحد فقط ----------

    [Fact]
    public async Task Seeder_CreatesExactlyOneAdministrator_EvenWhenSettingsChange()
    {
        await ProductionDbSeeder.SeedAsync(_factory.Services, AdministratorConfig("owner@test.local"));
        await ProductionDbSeeder.SeedAsync(_factory.Services, AdministratorConfig("intruder@test.local"));
        await ProductionDbSeeder.SeedAsync(_factory.Services, AdministratorConfig("owner@test.local"));

        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var administrators = await userManager.GetUsersInRoleAsync(RoleNames.Administrator);

        var administrator = Assert.Single(administrators);
        Assert.Equal("owner@test.local", administrator.Email);
        Assert.True(administrator.EmailConfirmed);
        Assert.Null(await userManager.FindByEmailAsync("intruder@test.local"));
    }

    [Fact]
    public async Task UserAccountService_NeverAssignsTheAdministratorRole()
    {
        using var scope = _factory.Services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
        var user = ApplicationUser.CreateTeacher("Sneaky", $"sneaky{Guid.NewGuid():N}@test.local");

        var result = await accounts.CreateUserAsync(user, "Str0ng#Pass1", RoleNames.Administrator, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.RoleNotAllowed", result.Error.Code);
    }

    private async Task<SeededTeacher> SeedTeacherAsync()
    {
        SeededTeacher? teacher = null;
        await _factory.SeedAsync(async db => teacher = await TestData.CreateTeacherAsync(db));
        return teacher!;
    }
}
