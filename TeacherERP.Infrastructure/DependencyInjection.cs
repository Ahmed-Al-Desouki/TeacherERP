// موجود (تعديل) — TeacherERP.Infrastructure/DependencyInjection.cs
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
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders(); // مطلوب لتوليد توكن استرجاع الباسورد

            // 3. قراءة وتسجيل الإعدادات بشكل مباشر وآمن بدون مشاكل الـ Configure
            var mailSettings = new MailSettings();
            configuration.GetSection("MailSettings").Bind(mailSettings);
            services.AddSingleton(Options.Create(mailSettings));

            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddSingleton(Options.Create(jwtSettings));

            var googleAuthSettings = new GoogleAuthSettings();
            configuration.GetSection("GoogleAuthSettings").Bind(googleAuthSettings);
            services.AddSingleton(Options.Create(googleAuthSettings));

            // 4. تفعيل الـ In-Memory Cache للـ OTP
            services.AddMemoryCache();

            // 5. الوصول لسياق الطلب الحالي (المنطقة الزمنية وهوية المستخدم)
            services.AddHttpContextAccessor();

            // 6. تسجيل الخدمات والمستودعات
            services.AddScoped<IUserAccountService, UserAccountService>();
            services.AddScoped<ICredentialService, CredentialService>();
            services.AddScoped<IExternalLoginService, ExternalLoginService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<ICenterRepository, CenterRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IDirectoryRepository, DirectoryRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<IGroupSessionRepository, GroupSessionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<ISessionValidator, SessionValidator>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICurrentTenantAccessor, CurrentTenantAccessor>();
            services.AddScoped<IUserTimeZoneAccessor, HttpUserTimeZoneAccessor>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IOtpService, OtpService>();
            services.AddTransient<IResetGrantService, ResetGrantService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();

            return services;
        }
    }
}