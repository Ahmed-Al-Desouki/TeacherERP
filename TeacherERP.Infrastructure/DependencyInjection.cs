using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Interfaces;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Repositories;
using TeacherERP.Infrastructure.Services;
using TeacherERP.Infrastructure.Settings;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. تسجيل DbContext مع SQL Server
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("TeacherERP.Infrastructure")
                ));

            // 2. تسجيل ASP.NET Core Identity Core
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

            // 3. قراءة وتسجيل إعدادات MailSettings و JwtSettings بشكل مباشر وآمن بدون مشاكل الـ Configure
            var mailSettings = new MailSettings();
            configuration.GetSection("MailSettings").Bind(mailSettings);
            services.AddSingleton(Options.Create(mailSettings));

            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddSingleton(Options.Create(jwtSettings));

            // 4. تفعيل الـ In-Memory Cache للـ OTP
            services.AddMemoryCache();

            // 5. تسجيل الخدمات والمستودعات
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IOtpService, OtpService>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            return services;
        }
    }
}