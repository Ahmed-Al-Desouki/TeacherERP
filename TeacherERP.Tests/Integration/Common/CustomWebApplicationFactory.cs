using System.Globalization;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NSubstitute;
using TeacherERP.API.Security;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
using TeacherERP.Application.Finance;

namespace TeacherERP.Tests.Integration;

// يشغّل الـ API كاملاً على SQLite In-Memory (اتصال واحد مفتوح طول عمر الـ Factory)، بمصادقة بديلة تمرّر UserId والدور بسهولة
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;
    public TestFileStorage FileStorage { get; } = new();
    public TestPaymobGateway PaymobGateway { get; } = new();
    public PaymobSettings PaymobSettings { get; } = new()
    {
        Mode = "Test",
        BaseUrl = "https://accept.paymob.com",
        Currency = "EGP",
        SecretKey = "test-paymob-secret-key",
        PublicKey = "pk_test_teacher_erp",
        HmacSecret = "test-paymob-hmac-secret",
        NotificationUrl = "https://api.test.example/api/paymob/webhooks/transaction",
        RedirectionUrl = "https://api.test.example/api/paymob/return",
        IntegrationIds = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["CAGG_Online_NonShopify"] = 6068299,
            ["VPC_Shopify"] = 6068290,
            ["CASH_CashChannel"] = 6068288,
            ["UIG_OnlineNew_NonShopify"] = 6068287,
            ["VPC_Online_NonShopify"] = 6068264
        }
    };

    public CustomWebApplicationFactory()
    {
        // متغيرات بيئة بدل الـ Settings لأن Program.cs يقرأ الإعدادات مبكراً (قبل تطبيق إعدادات الـ Factory)
        Environment.SetEnvironmentVariable("JwtSettings__Secret", "tests-only-secret-tests-only-secret-0123456789");
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", "TeacherERP.Tests");
        Environment.SetEnvironmentVariable("JwtSettings__Audience", "TeacherERP.Tests");

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // الجداول لازم تتكوّن قبل بدء التطبيق لأن الـ Seeder يشتغل داخل Program.cs
        using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        context.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // استبدال SQL Server بـ SQLite (نشيل كل تسجيلات الـ DbContext بما فيها إعدادات المزوّد)
            var dbDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                            || d.ServiceType == typeof(DbContextOptions)
                            || d.ServiceType == typeof(AppDbContext)
                            || (d.ServiceType.IsGenericType
                                && d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration", StringComparison.Ordinal)
                                && d.ServiceType.GenericTypeArguments[0] == typeof(AppDbContext)))
                .ToList();

            foreach (var descriptor in dbDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

            services.RemoveAll<IFileStorage>();
            services.AddSingleton<IFileStorage>(FileStorage);

            // Hangfire غير مشغّل في بيئة Testing، فنوفّر بدائل صامتة لمن يحقنها
            services.AddSingleton(Substitute.For<IBackgroundJobClient>());
            services.AddSingleton(Substitute.For<IRecurringJobManager>());

            services.RemoveAll<IOptions<PaymobSettings>>();
            services.AddSingleton<IOptions<PaymobSettings>>(Options.Create(PaymobSettings));
            services.RemoveAll<IPaymobGateway>();
            services.AddSingleton<IPaymobGateway>(PaymobGateway);

            // مصادقة الاختبارات تحل محل الـ JWT
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = TestAuthHandler.SchemeName;
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultForbidScheme = TestAuthHandler.SchemeName;
            });
        });
    }

    // عميل بهوية مستخدم ودور، وبالسنتر (X-Teacher-Id) لو اتحدد
    public HttpClient CreateClientFor(Guid userId, Guid? tenantId = null, string role = "SuperAdmin")
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role);

        if (tenantId is { } tenant)
        {
            client.DefaultRequestHeaders.Add(TenantRequirement.HeaderName, tenant.ToString());
        }

        return client;
    }

    // كتابة مباشرة في الداتابيز (بدون سنتر محدد، فالفلتر العام وحماية الكتابة لا يتدخلان)
    public async Task SeedAsync(Func<AppDbContext, Task> action)
    {
        using var scope = Services.CreateScope();
        await action(scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }

    // قراءة مباشرة (استخدم IgnoreQueryFilters() لأن الـ Scope بلا سنتر محدد)
    public async Task<T> QueryAsync<T>(Func<AppDbContext, Task<T>> query)
    {
        using var scope = Services.CreateScope();
        return await query(scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}


public sealed class TestFileStorage : IFileStorage
{
    private int _sequence;
    public int UploadCount { get; private set; }
    public List<string> DeletedPublicIds { get; } = new();

    public Task<Result<StoredFile>> UploadAsync(Stream content, string fileName, string folder, CancellationToken cancellationToken)
    {
        UploadCount++;
        var id = $"test/{folder}/{Interlocked.Increment(ref _sequence)}";
        return Task.FromResult(Result<StoredFile>.Success(new StoredFile($"https://storage.test/{id}", id)));
    }

    public Task<Result<bool>> DeleteAsync(string publicId, CancellationToken cancellationToken)
    {
        DeletedPublicIds.Add(publicId);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<FileStorageResourcePage> ListApplicationFilesAsync(string? nextCursor, int pageSize, CancellationToken cancellationToken)
        => Task.FromResult(new FileStorageResourcePage(Array.Empty<FileStorageResource>(), null));
}

public sealed class TestPaymobGateway : IPaymobGateway
{
    private int _sequence;
    public int CreateCount => Volatile.Read(ref _sequence);
    public PaymobIntentCommandData? LastRequest { get; private set; }
    public string? FailNextWithCode { get; set; }

    public Task<Result<PaymobIntentData>> CreateIntentAsync(PaymobIntentCommandData request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        var sequence = Interlocked.Increment(ref _sequence);
        if (FailNextWithCode is { } errorCode)
        {
            FailNextWithCode = null;
            return Task.FromResult(Result<PaymobIntentData>.Failure(
                new Error(errorCode, "Simulated provider failure.")));
        }
        return Task.FromResult(Result<PaymobIntentData>.Success(new PaymobIntentData(
            $"pi_test_{sequence}", (700000 + sequence).ToString(CultureInfo.InvariantCulture),
            $"egy_csk_test_{Guid.NewGuid():N}")));
    }
}
