using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace TeacherERP.Infrastructure.Data
{



    namespace TeacherERP.Infrastructure.Data
    {
        public static class ProductionDbSeeder
        {
            public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
            {
                using var scope = serviceProvider.CreateScope();
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                    // 1. التأكد من إنشاء الأدوار الأساسية
                    string[] roles = { "SuperAdmin", "Admin", "Student" };
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
                        // إنشاء كيان المستخدم مباشرة كإداري
                        var superAdminUser = ApplicationUser.CreateStudent(fullName, email, phoneNumber);
                        superAdminUser.EmailConfirmed = true; // تفعيل الحساب تلقائياً للإنتاج

                        var createResult = await userManager.CreateAsync(superAdminUser, password);
                        if (createResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
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
}
 
    
