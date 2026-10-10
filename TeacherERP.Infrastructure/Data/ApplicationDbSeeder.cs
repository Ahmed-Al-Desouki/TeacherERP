using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
                var existingAdmin = await userManager.FindByEmailAsync(email);
                if (existingAdmin == null)
                {
                    // إنشاء كيان المستخدم كإداري (وليس كطالب)، والمدرس الرئيسي له صلاحية الأموال
                    var superAdminUser = ApplicationUser.CreateAdmin(fullName, email, phoneNumber, hasFinancialDelegation: true);
                    superAdminUser.EmailConfirmed = true; // تفعيل الحساب تلقائياً للإنتاج

                    var createResult = await userManager.CreateAsync(superAdminUser, password);
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdminUser, RoleNames.SuperAdmin);
                        logger.LogInformation("Default SuperAdmin created and assigned successfully.");
                    }
                    else
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        logger.LogError($"Failed to create default SuperAdmin: {errors}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the production database.");
                throw;
            }
        }
    }
}