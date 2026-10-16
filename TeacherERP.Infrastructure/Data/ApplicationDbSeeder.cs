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
                string[] roles = { RoleNames.SuperAdmin, RoleNames.Admin, RoleNames.Student };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                        logger.LogInformation($"Role '{role}' created successfully.");
                    }
                }

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

            if (!string.Equals(timeZoneId, "UTC", StringComparison.OrdinalIgnoreCase) && !UserTime.IsValid(timeZoneId))
            {
                logger.LogWarning("Time zone '{TimeZoneId}' is not valid on this machine. Falling back to UTC.", timeZoneId);
                timeZoneId = "UTC";
            }

            // مساحة المدرس باسمه، والسنتر المكاني باسم الإعدادات، ويربطهما CenterTeacher (المدرس مؤسس السنتر)
            var tenant = Tenant.Create(owner.FullName, owner.Id, timeZoneId);
            var center = Center.Create(centerName, owner.Id);

            context.Tenants.Add(tenant);
            context.Centers.Add(center);
            context.CenterTeachers.Add(CenterTeacher.CreateFounder(center.Id, tenant.Id));
            context.TenantMembers.Add(TenantMember.Create(tenant.Id, owner.Id, TenantRole.Owner));
            context.TenantSettings.Add(TenantSettings.Create(tenant.Id));

            await context.SaveChangesAsync();

            logger.LogInformation("Center '{CenterName}' and the teacher space were created for the default SuperAdmin.", centerName);
        }
    }
}