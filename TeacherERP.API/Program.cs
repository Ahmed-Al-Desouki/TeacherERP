// موجود (تعديل) — TeacherERP.API/Program.cs
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;
using TeacherERP.API.Extensions;
using TeacherERP.API.HealthChecks;
using TeacherERP.API.Middlewares;
using TeacherERP.API.Security;
using TeacherERP.Application;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
using TeacherERP.Infrastructure;
using TeacherERP.Infrastructure.Data;
using TenantRole = TeacherERP.Domain.Entities.TenantRole;


var builder = WebApplication.CreateBuilder(args);

// بيئة الاختبارات الآلية (TeacherERP.Tests): نتخطى Hangfire لأنه يتصل بـ SQL Server عند البدء
var isTesting = builder.Environment.IsEnvironment("Testing");

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // الـ Enums تُرسل وتُستقبل كنص (Father, Male) بدل أرقام
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Smart Teacher ERP API", Version = "v1" });

    // إعداد زر Authorize في Swagger للـ JWT Bearer (الصق التوكن فقط بدون كلمة Bearer)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste your JWT token only (no need to type 'Bearer').",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddMemoryCache();

// حماية الـ API: الـ IP الحقيقي خلف الـ Proxy (اختياري)، CORS، وتحديد معدل الطلبات
builder.Services.AddApiForwardedHeaders(builder.Configuration);
builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddApiRateLimiting();

// Clean Architecture Extensions
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var healthChecks = builder.Services.AddHealthChecks()
    .AddCheck("process", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddCheck<DatabaseHealthCheck>("database", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready", "components" }, timeout: TimeSpan.FromSeconds(5))
    .AddCheck<CoreConfigurationHealthCheck>("authentication-configuration", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready", "components" }, timeout: TimeSpan.FromSeconds(2))
    .AddCheck<SmtpEndpointHealthCheck>("smtp", failureStatus: HealthStatus.Degraded, tags: new[] { "components" }, timeout: TimeSpan.FromSeconds(3))
    .AddCheck<CloudinaryConfigurationHealthCheck>("cloudinary-configuration", failureStatus: HealthStatus.Degraded, tags: new[] { "components" })
    .AddCheck<PaymobConfigurationHealthCheck>("paymob-configuration", failureStatus: HealthStatus.Degraded, tags: new[] { "components" })
    .AddCheck<GoogleAuthConfigurationHealthCheck>("google-auth-configuration", failureStatus: HealthStatus.Degraded, tags: new[] { "components" });

// Hangfire Configuration (استخدام قاعدة البيانات لتخزين الـ Jobs)
if (!isTesting)
{
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddHangfireServer(); // لتشغيل الـ Background Server
    healthChecks.AddCheck<HangfireWorkerHealthCheck>("hangfire-worker", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready", "components" }, timeout: TimeSpan.FromSeconds(5));
}

// Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
        ClockSkew = TimeSpan.Zero // بدون سماح إضافي: الـ Access Token يعيش المدة المحددة بالضبط
    };

    // التأكد من أن الجلسة المرتبطة بالتوكن ما زالت نشطة عند كل طلب
    options.Events = new SessionJwtBearerEvents();
});

// Authorization: Policy قفل الـ features حتى اكتمال بروفايل الطالب
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthPolicies.CompletedProfile, policy =>
        policy.RequireAuthenticatedUser().AddRequirements(new CompletedProfileRequirement()));

    // Policy تحديد السنتر الحالي والتحقق من العضوية
    options.AddPolicy(AuthPolicies.TenantMember, policy =>
        policy.RequireAuthenticatedUser().AddRequirements(new TenantRequirement()));

    options.AddPolicy(AuthPolicies.TenantStudent, policy =>
        policy.RequireAuthenticatedUser().AddRequirements(new TenantRequirement(TenantRole.Student)));

    // نفس التحقق مع قصر الدور داخل السنتر (الدور من العضوية لا من دور الـ Identity)
    options.AddPolicy(AuthPolicies.TenantStaff, policy =>
        policy.RequireAuthenticatedUser().AddRequirements(new TenantRequirement(TenantRole.Owner, TenantRole.Assistant)));

    options.AddPolicy(AuthPolicies.TenantOwner, policy =>
        policy.RequireAuthenticatedUser().AddRequirements(new TenantRequirement(TenantRole.Owner)));

    options.AddPolicy(AuthPolicies.TenantFinance, policy =>
        policy.RequireAuthenticatedUser().AddRequirements(new TenantRequirement(true, TenantRole.Owner, TenantRole.Assistant)));

    // مدير النظام: من دور الـ Identity نفسه (بلا سنتر حالي)، لأنه لا ينتمي لأي مساحة مدرس
    options.AddPolicy(AuthPolicies.AdministratorOnly, policy =>
        policy.RequireAuthenticatedUser().RequireRole(RoleNames.Administrator));

    // المدرس من دور الـ Identity (بلا سنتر حالي)، لمسار قبول طلبات المدرسين بمسح الـ QR
    options.AddPolicy(AuthPolicies.TeacherOnly, policy =>
        policy.RequireAuthenticatedUser().RequireRole(RoleNames.SuperAdmin));
});
builder.Services.AddScoped<IAuthorizationHandler, CompletedProfileHandler>();
builder.Services.AddScoped<IAuthorizationHandler, TenantHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, ApiAuthorizationResultHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    await ProductionDbSeeder.SeedAsync(services, configuration);

    // بيانات اختبار (مدرس ومساعد وطلاب ومجموعات) في بيئة التطوير فقط وبتفعيل صريح من الإعدادات
    if (app.Environment.IsDevelopment() && configuration.GetValue<bool>("TestData:Enabled"))
    {
        await TestDataSeeder.SeedAsync(services, configuration);
    }
}

if (!isTesting)
{
    // مهمة دورية يومياً (3 صباحاً UTC) لحذف الجلسات المنتهية منذ أكثر من 30 يوم
    app.Services.GetRequiredService<IRecurringJobManager>()
        .AddOrUpdate<ISessionCleanupJob>("cleanup-ended-sessions", job => job.RunAsync(), Cron.Daily(3));

    // مهمة دورية يومياً (2 صباحاً UTC) لتوليد حصص الأسابيع القادمة لكل المدرسين من قوالب جداولهم
    app.Services.GetRequiredService<IRecurringJobManager>()
        .AddOrUpdate<ISessionGenerationJob>("generate-sessions", job => job.RunAsync(), Cron.Daily(2));

    // مهمة دورية كل 10 دقايق لحساب غياب الحصص المنتهية والإيقاف التلقائي بعد 4 غيابات متتالية
    app.Services.GetRequiredService<IRecurringJobManager>()
        .AddOrUpdate<IAbsenceProcessingJob>("process-absences", job => job.RunAsync(), "*/10 * * * *");

    app.Services.GetRequiredService<IRecurringJobManager>()
        .AddOrUpdate<ICenterMembershipCleanupJob>("cleanup-center-membership", job => job.RunAsync(), "0 4 * * 0");

    app.Services.GetRequiredService<IRecurringJobManager>()
        .AddOrUpdate<ITransientAuthStateCleanupJob>("cleanup-transient-auth-state", job => job.RunAsync(), Cron.Daily(4));
}

// الـ IP الحقيقي أولاً (لا يعمل إلا لو فُعّل من الإعدادات)، لأن كل ما بعده (Rate Limiting واللوجات) يعتمد عليه
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

// HTTPS قبل اللوحة، حتى لا تُرسل بيانات الدخول عبر اتصال غير مشفر
app.UseHttpsRedirection();

// CORS قبل المصادقة (الـ Preflight لازم يُرد عليه قبلها)
app.UseCors(CorsExtensions.PolicyName);

// لوحة Hangfire محمية بـ Basic Auth (البيانات من قسم HangfireDashboard في الإعدادات)
if (!isTesting)
{
    var dashboardSettings = builder.Configuration.GetSection("HangfireDashboard");
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new IDashboardAuthorizationFilter[]
        {
            new HangfireBasicAuthFilter(dashboardSettings["Username"] ?? string.Empty, dashboardSettings["Password"] ?? string.Empty)
        }
    });
}

app.UseAuthentication();

// بعد المصادقة عشان الحدود اللي بالمستخدم (مسح الحضور) تعرف هويته، وقبل الـ Authorization عشان نوفّر استعلامات الداتابيز على الطلبات المرفوضة
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

HealthCheckOptions HealthOptions(string tag) => new()
{
    Predicate = check => check.Tags.Contains(tag),
    AllowCachingResponses = false,
    ResponseWriter = WriteHealthReportAsync
};

app.MapHealthChecks("/health/live", HealthOptions("live")).AllowAnonymous();
app.MapHealthChecks("/health/ready", HealthOptions("ready")).AllowAnonymous();
app.MapHealthChecks("/health/components", HealthOptions("components")).AllowAnonymous();

static Task WriteHealthReportAsync(HttpContext context, HealthReport report)
    => context.Response.WriteAsJsonAsync(new
    {
        status = report.Status.ToString(),
        checks = report.Entries.ToDictionary(entry => entry.Key, entry => entry.Value.Status.ToString())
    }, cancellationToken: context.RequestAborted);

app.Run();

// يتيح لـ WebApplicationFactory في TeacherERP.Tests الوصول للـ Program (الـ Top-level statements تجعله internal)
public partial class Program { }
