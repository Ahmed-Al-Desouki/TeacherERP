using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using TeacherERP.API.Middlewares;
using TeacherERP.API.Security;
using TeacherERP.Application;
using TeacherERP.Application.Interfaces;
using TeacherERP.Infrastructure;
using TeacherERP.Infrastructure.Data.TeacherERP.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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

// Clean Architecture Extensions
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Hangfire Configuration (استخدام قاعدة البيانات لتخزين الـ Jobs)
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer(); // لتشغيل الـ Background Server

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    await ProductionDbSeeder.SeedAsync(services, configuration);
}

// مهمة دورية يومياً (3 صباحاً UTC) لحذف الجلسات المنتهية منذ أكثر من 30 يوم
app.Services.GetRequiredService<IRecurringJobManager>()
    .AddOrUpdate<ISessionCleanupJob>("cleanup-ended-sessions", job => job.RunAsync(), Cron.Daily(3));

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{

}

app.UseSwagger();
app.UseSwaggerUI();

// HTTPS قبل اللوحة، حتى لا تُرسل بيانات الدخول عبر اتصال غير مشفر
app.UseHttpsRedirection();

// لوحة Hangfire محمية بـ Basic Auth (البيانات من قسم HangfireDashboard في الإعدادات)
var dashboardSettings = builder.Configuration.GetSection("HangfireDashboard");
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new IDashboardAuthorizationFilter[]
    {
        new HangfireBasicAuthFilter(dashboardSettings["Username"] ?? string.Empty, dashboardSettings["Password"] ?? string.Empty)
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();