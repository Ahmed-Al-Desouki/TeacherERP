using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Infrastructure.Data
{
    /// <summary>
    /// بيانات اختبار كاملة لمدرس ثانٍ: حسابه وسنتره ومساحته ومساعد وطلاب ومجموعات وحصص.
    /// لا تعمل إلا في بيئة Development وبتفعيل صريح (TestData:Enabled)، والباسورد من الإعدادات وليس من الكود.
    /// آمنة للتكرار (تتخطى ما هو موجود). كل حسابات الاختبار بنهاية @test.teachererp.local لسهولة حذفها.
    /// </summary>
    public static class TestDataSeeder
    {
        private const string EmailDomain = "test.teachererp.local";
        private const string TimeZoneId = "Africa/Cairo";

        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(TestDataSeeder));

            var password = configuration["TestData:Password"];
            if (string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("TestData:Password is missing in configuration. Test data was not seeded.");
                return;
            }

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var context = services.GetRequiredService<AppDbContext>();

                // 1. الحسابات: مدرس (SuperAdmin) ومساعد (Admin) و3 طلاب ببروفايل مكتمل
                var teacher = await EnsureStaffAsync(userManager, "teacher.math", "أ. محمد علي", "+201000000011", RoleNames.SuperAdmin, password);
                var assistant = await EnsureStaffAsync(userManager, "assistant.math", "أ. سارة حسن", "+201000000012", RoleNames.Admin, password);

                var students = new List<ApplicationUser>();
                for (var i = 1; i <= 3; i++)
                {
                    students.Add(await EnsureStudentAsync(userManager, i, password));
                }

                // 2. مساحة المدرس وسنتره الخاص (هو مؤسسه) وإعداداته
                var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.OwnerId == teacher.Id);
                if (tenant is null)
                {
                    tenant = Tenant.Create(teacher.FullName, teacher.Id, TimeZoneId);
                    tenant.UpdateProfile(teacher.FullName, "Mathematics");
                    context.Tenants.Add(tenant);
                }

                var ownCenter = await context.Centers.FirstOrDefaultAsync(c => c.OwnerUserId == teacher.Id);
                if (ownCenter is null)
                {
                    ownCenter = Center.Create("Test Center", teacher.Id);
                    ownCenter.UpdateDetails("Test Center", "Nasr City, Cairo", 30.0561, 31.3301, Center.DefaultGeofenceRadiusMeters);
                    context.Centers.Add(ownCenter);
                }

                if (!await context.CenterTeachers.AnyAsync(l => l.CenterId == ownCenter.Id && l.TenantId == tenant.Id))
                {
                    context.CenterTeachers.Add(CenterTeacher.CreateFounder(ownCenter.Id, tenant.Id));
                }

                // 3. يدرّس كمان في سنتر المدرس الرئيسي (سنتر يضم أكتر من مدرس)
                var mainCenter = await FindMainCenterAsync(userManager, context, configuration);
                if (mainCenter is not null
                    && !await context.CenterTeachers.AnyAsync(l => l.CenterId == mainCenter.Id && l.TenantId == tenant.Id))
                {
                    context.CenterTeachers.Add(CenterTeacher.CreateActive(mainCenter.Id, tenant.Id, CenterTeacherOrigin.Invitation));
                }

                if (!await context.TenantSettings.IgnoreQueryFilters().AnyAsync(s => s.TenantId == tenant.Id))
                {
                    context.TenantSettings.Add(TenantSettings.Create(tenant.Id));
                }

                // 4. العضويات: المدرس Owner، والمساعد بتفويض مالي، والطلاب
                await EnsureMemberAsync(context, tenant.Id, teacher.Id, TenantRole.Owner, false);
                await EnsureMemberAsync(context, tenant.Id, assistant.Id, TenantRole.Assistant, true);
                foreach (var student in students)
                {
                    await EnsureMemberAsync(context, tenant.Id, student.Id, TenantRole.Student, false);
                }

                await context.SaveChangesAsync();

                // 5. مجموعتان بجدول أسبوعي: السبت في سنتره (المساعد مخصص لها)، والثلاثاء في السنتر الرئيسي
                await EnsureGroupAsync(
                    context, tenant.Id, "رياضيات - ثالثة ثانوي - السبت", "Third Secondary", ownCenter.Id, assistant,
                    (DayOfWeek.Saturday, new TimeSpan(16, 0, 0), 120));

                await EnsureGroupAsync(
                    context, tenant.Id, "رياضيات - ثالثة ثانوي - الثلاثاء", "Third Secondary", mainCenter?.Id ?? ownCenter.Id, null,
                    (DayOfWeek.Tuesday, new TimeSpan(18, 0, 0), 90));

                await context.SaveChangesAsync();

                // 6. توليد الحصص القادمة، بـ Scope مستقل وسنتر محدد صراحةً
                using var generationScope = serviceProvider.CreateScope();
                generationScope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>()
                    .Set(new TenantContext(tenant.Id, TenantRole.Owner, MembershipStatus.Active));

                var generated = await generationScope.ServiceProvider
                    .GetRequiredService<ISessionGenerator>()
                    .GenerateForAllGroupsAsync(CancellationToken.None);

                logger.LogInformation(
                    "Test data is ready ({Generated} new sessions). Logins: teacher.math@{Domain}, assistant.math@{Domain}, student1..3@{Domain}.",
                    generated, EmailDomain, EmailDomain);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the test data.");
                throw;
            }
        }

        private static async Task<ApplicationUser> EnsureStaffAsync(
            UserManager<ApplicationUser> userManager, string emailLocalPart, string fullName, string phone, string role, string password)
        {
            var email = $"{emailLocalPart}@{EmailDomain}";
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
            {
                return existing;
            }

            var user = ApplicationUser.CreateAdmin(fullName, email, phone);
            user.EmailConfirmed = true;

            await CreateAsync(userManager, user, password, role);
            return user;
        }

        private static async Task<ApplicationUser> EnsureStudentAsync(UserManager<ApplicationUser> userManager, int index, string password)
        {
            var email = $"student{index}@{EmailDomain}";
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
            {
                return existing;
            }

            var fullName = $"طالب اختبار {index}";
            var user = ApplicationUser.CreateStudent(fullName, email);
            user.EmailConfirmed = true;

            // بروفايل مكتمل (موبايل الطالب مختلف عن موبايل ولي الأمر) حتى تعمل كل الـ features
            var guardian = GuardianInfo.Create($"ولي أمر {fullName}", $"+2010000002{index:00}", GuardianRelation.Father);
            var profile = user.CompleteProfile($"+2010000001{index:00}", guardian, Gender.Male, null);
            if (profile.IsFailure)
            {
                throw new InvalidOperationException($"Test student profile is invalid: {profile.Error.Message}");
            }

            await CreateAsync(userManager, user, password, RoleNames.Student);
            return user;
        }

        private static async Task CreateAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string role)
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create test user {user.Email}: {errors}");
            }

            await userManager.AddToRoleAsync(user, role);
        }

        // سنتر المدرس الرئيسي (المزروع من SuperAdminSettings)، أو null لو غير موجود
        private static async Task<Center?> FindMainCenterAsync(
            UserManager<ApplicationUser> userManager, AppDbContext context, IConfiguration configuration)
        {
            var mainEmail = configuration["SuperAdminSettings:Email"];
            if (string.IsNullOrWhiteSpace(mainEmail))
            {
                return null;
            }

            var mainOwner = await userManager.FindByEmailAsync(mainEmail);
            if (mainOwner is null)
            {
                return null;
            }

            return await context.Centers.FirstOrDefaultAsync(c => c.OwnerUserId == mainOwner.Id);
        }

        private static async Task EnsureMemberAsync(AppDbContext context, Guid tenantId, Guid userId, TenantRole role, bool financialDelegation)
        {
            // IgnoreQueryFilters مقصود: الـ Seeder يعمل بدون سنتر محدد فالفلتر العام يخفي كل شيء
            if (await context.TenantMembers.IgnoreQueryFilters().AnyAsync(m => m.TenantId == tenantId && m.UserId == userId))
            {
                return;
            }

            var member = TenantMember.Create(tenantId, userId, role);
            if (financialDelegation)
            {
                member.SetFinancialDelegation(true);
            }

            context.TenantMembers.Add(member);
        }

        private static async Task EnsureGroupAsync(
            AppDbContext context,
            Guid tenantId,
            string name,
            string gradeLevel,
            Guid centerId,
            ApplicationUser? assignedAssistant,
            params (DayOfWeek Day, TimeSpan Start, int DurationMinutes)[] slots)
        {
            if (await context.Groups.IgnoreQueryFilters().AnyAsync(g => g.TenantId == tenantId && g.Name == name))
            {
                return;
            }

            var group = Group.Create(tenantId, name, gradeLevel, centerId, "Hall 1", 30, null);

            foreach (var slot in slots)
            {
                group.AddSlot(slot.Day, slot.Start, slot.DurationMinutes);
            }

            if (assignedAssistant is not null)
            {
                group.AddAdmin(assignedAssistant);
            }

            context.Groups.Add(group);
        }
    }
}