using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using TeacherERP.Application.Features.TeacherRequests;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Integration;

// A4: طلب مدرس من صفحة الدخول، والمدرس الذي يمسح الـ QR يقبله (أو مدير النظام)
public class TeacherRequestsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly IEmailService _email = Substitute.For<IEmailService>();
    private readonly WebApplicationFactory<Program> _appFactory;

    public TeacherRequestsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        // الإيميل بديل صامت: القبول ينشئ حساباً ويرسل الباسورد المؤقت مباشرة (لا SMTP حقيقي في الاختبارات)
        _appFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => services.AddSingleton(_email)));
    }

    private HttpClient ClientFor(Guid userId, string role)
    {
        var client = _appFactory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role);
        return client;
    }

    private HttpClient Anonymous() => _appFactory.CreateClient();

    private async Task<(string Token, string Email)> SeedRequestAsync(DateTime? expiresAt = null, string subject = "Math")
    {
        var token = TeacherRequestToken.Generate();
        var email = $"applicant{Guid.NewGuid():N}@test.local";

        await _factory.SeedAsync(async db =>
        {
            db.TeacherAccessRequests.Add(TeacherAccessRequest.Create(
                "Applicant", email, subject, "hello", "Africa/Cairo",
                TeacherRequestToken.Hash(token), expiresAt ?? DateTime.UtcNow.AddHours(5)));
            await db.SaveChangesAsync();
        });

        return (token, email);
    }

    private async Task<SeededTeacher> SeedTeacherAsync()
    {
        SeededTeacher? teacher = null;
        await _factory.SeedAsync(async db => teacher = await TestData.CreateTeacherAsync(db));
        return teacher!;
    }

    // ---------- صاحب الطلب (بلا حساب) ----------

    [Fact]
    public async Task Submit_WithInvalidBody_ReturnsBadRequest()
    {
        var response = await Anonymous().PostAsJsonAsync(
            "api/TeacherRequests/submit", new { fullName = "Ali", email = "not-an-email", subject = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Submit_ForNewEmail_ReturnsOk_AndVerifyWithoutTheRightCodeFails()
    {
        var email = $"new{Guid.NewGuid():N}@test.local";
        var client = Anonymous();

        var submit = await client.PostAsJsonAsync(
            "api/TeacherRequests/submit", new { fullName = "Ali", email, subject = "Math" });
        var verify = await client.PostAsJsonAsync("api/TeacherRequests/verify", new { email, otpCode = "000000" });

        Assert.Equal(HttpStatusCode.OK, submit.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, verify.StatusCode);
        Assert.Contains("Otp.", await verify.Content.ReadAsStringAsync());
        Assert.False(await _factory.QueryAsync(db => db.TeacherAccessRequests.AnyAsync(r => r.Email == email)));
    }

    [Fact]
    public async Task Submit_ForRegisteredEmail_LooksIdentical_ButNeverCreatesARequest()
    {
        var teacher = await SeedTeacherAsync();
        var existingEmail = await _factory.QueryAsync(db =>
            db.Users.Where(u => u.Id == teacher.OwnerId).Select(u => u.Email!).SingleAsync());

        // حساب المدرس المزروع مباشرة بلا Normalized Email (UserManager يبحث به)، فنحاكي حساباً مسجّلاً بشكل طبيعي
        await _factory.SeedAsync(async db =>
        {
            var owner = await db.Users.SingleAsync(u => u.Id == teacher.OwnerId);
            owner.NormalizedEmail = existingEmail.ToUpperInvariant();
            await db.SaveChangesAsync();
        });
        var client = Anonymous();

        var submit = await client.PostAsJsonAsync(
            "api/TeacherRequests/submit", new { fullName = "Ali", email = existingEmail, subject = "Math" });
        var verify = await client.PostAsJsonAsync("api/TeacherRequests/verify", new { email = existingEmail, otpCode = "123456" });

        Assert.Equal(HttpStatusCode.OK, submit.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, verify.StatusCode);
        Assert.Contains("Otp.Expired", await verify.Content.ReadAsStringAsync());
        Assert.False(await _factory.QueryAsync(db => db.TeacherAccessRequests.AnyAsync(r => r.Email == existingEmail)));
    }

    [Fact]
    public async Task Status_WithUnknownToken_ReturnsNotFound_AndKnownTokenReturnsPending()
    {
        var (token, _) = await SeedRequestAsync();
        var client = Anonymous();

        var unknown = await client.PostAsJsonAsync("api/TeacherRequests/status", new { token = "nope" });
        var known = await client.PostAsJsonAsync("api/TeacherRequests/status", new { token });

        Assert.Equal(HttpStatusCode.BadRequest, unknown.StatusCode);
        Assert.Contains("TeacherRequest.NotFound", await unknown.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, known.StatusCode);
        Assert.Contains("Pending", await known.Content.ReadAsStringAsync());
    }

    // ---------- الصلاحيات ----------

    [Theory]
    [InlineData("Student")]
    [InlineData("Assistant")]
    [InlineData("Administrator")]
    public async Task ReviewEndpoints_RejectEveryRoleExceptTeacher(string role)
    {
        var client = ClientFor(Guid.NewGuid(), role);

        var preview = await client.PostAsJsonAsync("api/TeacherRequests/preview", new { token = "t" });
        var approve = await client.PostAsJsonAsync("api/TeacherRequests/approve", new { token = "t" });
        var reject = await client.PostAsJsonAsync("api/TeacherRequests/reject", new { token = "t" });

        Assert.Equal(HttpStatusCode.Forbidden, preview.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, approve.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, reject.StatusCode);
    }

    [Fact]
    public async Task ReviewEndpoints_RejectAnonymousCallers()
    {
        var response = await Anonymous().PostAsJsonAsync("api/TeacherRequests/approve", new { token = "t" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Assistant")]
    [InlineData("Student")]
    public async Task AdministrationRequestEndpoints_RejectEveryRoleExceptAdministrator(string role)
    {
        var client = ClientFor(Guid.NewGuid(), role);

        var list = await client.GetAsync("api/Administration/teacher-requests");
        var approve = await client.PostAsync($"api/Administration/teacher-requests/{Guid.NewGuid()}/approve", null);

        Assert.Equal(HttpStatusCode.Forbidden, list.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, approve.StatusCode);
    }

    // ---------- المدرس الذي مسح الـ QR ----------

    [Fact]
    public async Task Preview_ShowsApplicantData_WithoutCreatingAnything()
    {
        var teacher = await SeedTeacherAsync();
        var (token, email) = await SeedRequestAsync();

        var response = await ClientFor(teacher.OwnerId, "SuperAdmin")
            .PostAsJsonAsync("api/TeacherRequests/preview", new { token });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Applicant", body);
        Assert.Contains(email, body);
        Assert.False(await _factory.QueryAsync(db => db.Users.AnyAsync(u => u.Email == email)));
    }

    [Fact]
    public async Task Preview_WhenExpired_ReturnsExpiredError()
    {
        var teacher = await SeedTeacherAsync();
        var (token, _) = await SeedRequestAsync(expiresAt: DateTime.UtcNow.AddMinutes(-5));

        var response = await ClientFor(teacher.OwnerId, "SuperAdmin")
            .PostAsJsonAsync("api/TeacherRequests/preview", new { token });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("TeacherRequest.Expired", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Approve_CreatesTeacherWithOwnSpace_EmailsPassword_Audits_AndIsSingleUse()
    {
        var teacher = await SeedTeacherAsync();
        var (token, email) = await SeedRequestAsync(subject: "Physics");
        var client = ClientFor(teacher.OwnerId, "SuperAdmin");

        var first = await client.PostAsJsonAsync("api/TeacherRequests/approve", new { token });
        var second = await client.PostAsJsonAsync("api/TeacherRequests/approve", new { token });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
        Assert.Contains("TeacherRequest.NotPending", await second.Content.ReadAsStringAsync());

        await _email.Received(1).SendTeacherWelcomeAsync(email, "Applicant", Arg.Any<string>());

        var user = await _factory.QueryAsync(db => db.Users.SingleAsync(u => u.Email == email));
        var tenant = await _factory.QueryAsync(db => db.Tenants.SingleAsync(t => t.OwnerId == user.Id));
        Assert.Equal("Physics", tenant.Subject);
        Assert.Equal("Africa/Cairo", tenant.TimeZoneId);

        var request = await _factory.QueryAsync(db => db.TeacherAccessRequests.SingleAsync(r => r.Email == email));
        Assert.Equal(TeacherAccessRequestStatus.Approved, request.Status);
        Assert.Equal(teacher.OwnerId, request.ReviewedByUserId);
        Assert.Equal(user.Id, request.CreatedUserId);

        var audit = await _factory.QueryAsync(db => db.AuditLogs.IgnoreQueryFilters()
            .SingleAsync(a => a.TenantId == teacher.TenantId && a.TargetUserId == user.Id));
        Assert.Equal(AuditAction.TeacherAdmitted, audit.Action);
        Assert.Equal(teacher.OwnerId, audit.ActorUserId);

        var status = await Anonymous().PostAsJsonAsync("api/TeacherRequests/status", new { token });
        Assert.Contains("Approved", await status.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Approve_SecondTimeWithinThirtyDays_IsBlocked_ButAdministratorIsExempt()
    {
        var teacher = await SeedTeacherAsync();
        var (firstToken, _) = await SeedRequestAsync();
        var (secondToken, secondEmail) = await SeedRequestAsync();
        var client = ClientFor(teacher.OwnerId, "SuperAdmin");

        var first = await client.PostAsJsonAsync("api/TeacherRequests/approve", new { token = firstToken });
        var second = await client.PostAsJsonAsync("api/TeacherRequests/approve", new { token = secondToken });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
        Assert.Contains("TeacherRequest.QuotaReached", await second.Content.ReadAsStringAsync());
        Assert.False(await _factory.QueryAsync(db => db.Users.AnyAsync(u => u.Email == secondEmail)));

        var requestId = await _factory.QueryAsync(db => db.TeacherAccessRequests
            .Where(r => r.Email == secondEmail).Select(r => r.Id).SingleAsync());
        var byAdministrator = await ClientFor(Guid.NewGuid(), RoleNames.Administrator)
            .PostAsync($"api/Administration/teacher-requests/{requestId}/approve", null);

        Assert.Equal(HttpStatusCode.OK, byAdministrator.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.Users.AnyAsync(u => u.Email == secondEmail)));
    }

    [Fact]
    public async Task Reject_ByTeacher_CreatesNoAccount_AndApplicantSeesTheReason()
    {
        var teacher = await SeedTeacherAsync();
        var (token, email) = await SeedRequestAsync();

        var response = await ClientFor(teacher.OwnerId, "SuperAdmin")
            .PostAsJsonAsync("api/TeacherRequests/reject", new { token, reason = "Not a teacher we know" });
        var status = await Anonymous().PostAsJsonAsync("api/TeacherRequests/status", new { token });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await status.Content.ReadAsStringAsync();
        Assert.Contains("Rejected", body);
        Assert.Contains("Not a teacher we know", body);
        Assert.False(await _factory.QueryAsync(db => db.Users.AnyAsync(u => u.Email == email)));
        await _email.DidNotReceive().SendTeacherWelcomeAsync(email, Arg.Any<string>(), Arg.Any<string>());
    }

    // ---------- مدير النظام ----------

    [Fact]
    public async Task Administration_ListsRequests_FilteredByStatus_AndRejects()
    {
        var (_, email) = await SeedRequestAsync();
        var (_, expiredEmail) = await SeedRequestAsync(expiresAt: DateTime.UtcNow.AddMinutes(-5));
        var admin = ClientFor(Guid.NewGuid(), RoleNames.Administrator);

        var pending = await admin.GetStringAsync("api/Administration/teacher-requests?status=Pending&pageSize=100");
        var expired = await admin.GetStringAsync("api/Administration/teacher-requests?status=Expired&pageSize=100");

        Assert.Contains(email, pending);
        Assert.DoesNotContain(expiredEmail, pending);
        Assert.Contains(expiredEmail, expired);

        var requestId = await _factory.QueryAsync(db => db.TeacherAccessRequests
            .Where(r => r.Email == email).Select(r => r.Id).SingleAsync());
        var reject = await admin.PostAsJsonAsync(
            $"api/Administration/teacher-requests/{requestId}/reject", new { reason = "duplicate" });

        Assert.Equal(HttpStatusCode.OK, reject.StatusCode);
        var request = await _factory.QueryAsync(db => db.TeacherAccessRequests.SingleAsync(r => r.Id == requestId));
        Assert.Equal(TeacherAccessRequestStatus.Rejected, request.Status);
    }

    [Fact]
    public async Task Administration_WithInvalidPage_ReturnsBadRequest()
    {
        var response = await ClientFor(Guid.NewGuid(), RoleNames.Administrator)
            .GetAsync("api/Administration/teacher-requests?page=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
