// موجود (تعديل) — TeacherERP.Infrastructure/Data/ApplicationDbSeeder.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Infrastructure.Data
{
    public static class ProductionDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ProductionDbSeeder));

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var context = services.GetRequiredService<AppDbContext>();

                // 1. التأكد من إنشاء الأدوار الأساسية
                string[] roles = { RoleNames.Administrator, RoleNames.SuperAdmin, RoleNames.Assistant, RoleNames.Student };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                        logger.LogInformation($"Role '{role}' created successfully.");
                    }
                }

                // 1.5 مدير النظام (حساب واحد) من قسم AdministratorSettings، مستقل عن إعدادات المدرس
                await EnsureAdministratorAsync(userManager, configuration.GetSection("AdministratorSettings"), logger);

                // 2. قراءة بيانات الـ Super Admin من ملف الإعدادات (أو الـ Environment Variables)
                var adminConfig = configuration.GetSection("SuperAdminSettings");
                var email = adminConfig["Email"];
                var password = adminConfig["Password"];
                var fullName = adminConfig["FullName"] ?? "Super Admin";
                var phoneNumber = adminConfig["PhoneNumber"] ?? "+201000000000";

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    logger.LogWarning("SuperAdmin settings are missing in configuration.");
                    return;
                }

                // 3. التحقق هل الـ Super Admin موجود مسبقاً؟
                var superAdmin = await userManager.FindByEmailAsync(email);
                if (superAdmin == null)
                {
                    // إنشاء كيان المستخدم كإداري (وليس كطالب)، وصلاحيته المالية تأتي من دور Owner في عضوية سنتره
                    var superAdminUser = ApplicationUser.CreateAdmin(fullName, email, phoneNumber);
                    superAdminUser.EmailConfirmed = true; // تفعيل الحساب تلقائياً للإنتاج

                    var createResult = await userManager.CreateAsync(superAdminUser, password);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        logger.LogError($"Failed to create default SuperAdmin: {errors}");
                        return;
                    }

                    await userManager.AddToRoleAsync(superAdminUser, RoleNames.SuperAdmin);
                    logger.LogInformation("Default SuperAdmin created and assigned successfully.");
                    superAdmin = superAdminUser;
                }

                // 4. التأكد من وجود سنتر (Tenant) للمدرس الرئيسي
                await EnsureOwnerTenantAsync(context, superAdmin, adminConfig, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the production database.");
                throw;
            }
        }

        // مدير النظام: حساب واحد بالضبط. لو وُجد حساب بالدور فعلاً لا نُنشئ غيره مهما تغيّرت الإعدادات
        private static async Task EnsureAdministratorAsync(
            UserManager<ApplicationUser> userManager,
            IConfigurationSection adminConfig,
            ILogger logger)
        {
            var email = adminConfig["Email"];
            var password = adminConfig["Password"];
            var fullName = adminConfig["FullName"] ?? "System Administrator";

            var existing = await userManager.GetUsersInRoleAsync(RoleNames.Administrator);
            if (existing.Count > 0)
            {
                if (!string.IsNullOrEmpty(email)
                    && !existing.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
                {
                    logger.LogWarning("An Administrator account already exists. The system allows exactly one, so the configured account was not created.");
                }

                return;
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                logger.LogWarning("AdministratorSettings are missing in configuration. No Administrator account was created.");
                return;
            }

            // لا نرقّي حساباً موجوداً (مثلاً مدرس) إلى مدير نظام بالخطأ
            if (await userManager.FindByEmailAsync(email) is not null)
            {
                logger.LogWarning("The configured Administrator email already belongs to another account. No Administrator account was created.");
                return;
            }

            var administrator = ApplicationUser.CreateAdministrator(fullName, email);

            var createResult = await userManager.CreateAsync(administrator, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                logger.LogError("Failed to create the Administrator account: {Errors}", errors);
                return;
            }

            await userManager.AddToRoleAsync(administrator, RoleNames.Administrator);
            logger.LogInformation("The Administrator account was created successfully.");
        }

        // سنتر المدرس: الاسم والتوقيت من الإعدادات (CenterName وTimeZoneId) أو القيم الافتراضية
        private static async Task EnsureOwnerTenantAsync(
            AppDbContext context,
            ApplicationUser owner,
            IConfigurationSection adminConfig,
            ILogger logger)
        {
            if (await context.Tenants.AnyAsync(t => t.OwnerId == owner.Id))
            {
                return;
            }

            var centerName = adminConfig["CenterName"] ?? "Main Center";
            var timeZoneId = adminConfig["TimeZoneId"] ?? "Africa/Cairo";
            var hasLatitude = double.TryParse(adminConfig["CenterLatitude"], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var latitude);
            var hasLongitude = double.TryParse(adminConfig["CenterLongitude"], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var longitude);

            if (!string.Equals(timeZoneId, "UTC", StringComparison.OrdinalIgnoreCase) && !UserTime.IsValid(timeZoneId))
            {
                logger.LogWarning("Time zone '{TimeZoneId}' is not valid on this machine. Falling back to UTC.", timeZoneId);
                timeZoneId = "UTC";
            }

            // مساحة المدرس باسمه، والسنتر المكاني باسم الإعدادات، ويربطهما CenterTeacher (المدرس مؤسس السنتر)
            var tenant = Tenant.Create(owner.FullName, owner.Id, timeZoneId);
            context.Tenants.Add(tenant);
            context.TenantMembers.Add(TenantMember.Create(tenant.Id, owner.Id, TenantRole.Owner));
            context.TenantSettings.Add(TenantSettings.Create(tenant.Id));

            if (hasLatitude && hasLongitude && latitude is >= -90 and <= 90 && longitude is >= -180 and <= 180)
            {
                var center = Center.Create(centerName, owner.Id, latitude, longitude,
                    adminConfig["CenterAddress"], Center.DefaultGeofenceRadiusMeters);
                context.Centers.Add(center);
                context.CenterTeachers.Add(CenterTeacher.CreateFounder(center.Id, tenant.Id));
            }
            else
            {
                logger.LogWarning("Default center was not created because SuperAdminSettings:CenterLatitude and CenterLongitude are missing or invalid.");
            }

            await context.SaveChangesAsync();

            logger.LogInformation("Teacher space for the default SuperAdmin was created. Center configured: {CenterConfigured}.", hasLatitude && hasLongitude);
        }
    }
}
