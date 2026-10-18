// موجود (تعديل) — TeacherERP.Infrastructure/Data/AppDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeacherERP.Domain.Entities;
using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Infrastructure.Data
{
    // توريث من IdentityDbContext لدمج جداول الـ Security جاهزة
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        // كل DateTime في النظام UTC: نضمن أن يُقرأ بصفته UTC حتى يظهر في الـ JSON بالـ Z (لا تغيير في شكل الجداول)
        private static readonly ValueConverter<DateTime, DateTime> UtcConverter = new(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        private static readonly ValueConverter<DateTime?, DateTime?> NullableUtcConverter = new(
            v => v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        private readonly ICurrentTenantAccessor? _currentTenant;

        // اختياري عمداً: أدوات الـ Migrations (AppDbContextFactory) تنشئ الـ Context بدونه.
        // وبدون سنتر محدد يكون الفلتر العام آمناً (يرجّع نتيجة فاضية).
        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantAccessor? currentTenant = null) : base(options)
        {
            _currentTenant = currentTenant;
        }

        // السنتر الحالي للفلتر العام (يُقرأ لكل Query، وGuid.Empty يعني لا سنتر فلا نتائج)
        private Guid CurrentTenantId => _currentTenant?.TenantId ?? Guid.Empty;

        // تسجيل الجداول في قاعدة البيانات
        public DbSet<LoginSession> LoginSessions { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantMember> TenantMembers { get; set; }
        public DbSet<Center> Centers { get; set; }
        public DbSet<CenterTeacher> CenterTeachers { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupScheduleSlot> GroupScheduleSlots { get; set; }
        public DbSet<GroupEnrollment> GroupEnrollments { get; set; }
        public DbSet<EnrollmentRequest> EnrollmentRequests { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // سطر إجباري لتوليد جداول الـ Identity بشكل سليم
            base.OnModelCreating(builder);

            // 1. علاقة المستخدم مع جلسات الدخول (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.LoginSessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade); // إذا حُذف المستخدم، تُحذف جلساته

            builder.Entity<LoginSession>(entity =>
            {
                entity.Property(s => s.DeviceId).HasMaxLength(128).IsRequired();
                entity.Property(s => s.DeviceName).HasMaxLength(256).IsRequired();
                entity.Property(s => s.RefreshTokenHash).HasMaxLength(64).IsRequired();
                entity.Property(s => s.PreviousRefreshTokenHash).HasMaxLength(64);

                // للبحث السريع عن جلسات المستخدم وجهازه
                entity.HasIndex(s => new { s.UserId, s.DeviceId });
            });

            // بروفايل الطالب: الجنس وبيانات ولي الأمر (في نفس جدول المستخدمين) وفرادة موبايل الطالب
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.Gender).HasConversion<string>().HasMaxLength(16);

                entity.OwnsOne(u => u.Guardian, guardian =>
                {
                    guardian.Property(g => g.Name).HasMaxLength(100).IsRequired();
                    guardian.Property(g => g.PhoneNumber).HasMaxLength(20).IsRequired();
                    guardian.Property(g => g.Relation).HasConversion<string>().HasMaxLength(32).IsRequired();
                });

                // Identity يجعل PhoneNumber بطول max، ولازم طول محدد لإنشاء Index عليه
                entity.Property(u => u.PhoneNumber).HasMaxLength(20);

                // فهرس فريد مُرشَّح (الحقل اختياري، فالقيم الفارغة لا تتعارض)
                entity.HasIndex(u => u.PhoneNumber).IsUnique().HasFilter("[PhoneNumber] IS NOT NULL");
            });

            // 2. السنتر (Tenant): مالكه المدرس (SuperAdmin)، ولكل مدرس سنتر واحد فقط
            builder.Entity<Tenant>(entity =>
            {
                entity.Property(t => t.Name).HasMaxLength(150).IsRequired();
                entity.Property(t => t.Subject).HasMaxLength(100);
                entity.Property(t => t.TimeZoneId).HasMaxLength(64).IsRequired();
                entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(16);

                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(t => t.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => t.OwnerId).IsUnique();
            });

            // 2.1 السنتر (المكان): صاحبه مدرس، ويضم مدرسين عبر CenterTeacher (غير معزول بالـ Tenant عمداً)
            builder.Entity<Center>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(150).IsRequired();
                entity.Property(c => c.Address).HasMaxLength(300);

                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(c => c.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.OwnerUserId);
            });

            // 2.2 مدرس في سنتر: المدرس الواحد مرة واحدة في السنتر الواحد
            builder.Entity<CenterTeacher>(entity =>
            {
                entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
                entity.Property(x => x.Origin).HasConversion<string>().HasMaxLength(16);

                entity.HasOne(x => x.Center)
                    .WithMany()
                    .HasForeignKey(x => x.CenterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Tenant>()
                    .WithMany()
                    .HasForeignKey(x => x.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.CenterId, x.TenantId }).IsUnique();
                entity.HasIndex(x => x.TenantId);
            });

            // 3. عضوية المستخدم في السنتر (الحساب واحد، والعضويات بعدد السناتر)
            builder.Entity<TenantMember>(entity =>
            {
                entity.Property(m => m.Role).HasConversion<string>().HasMaxLength(16);
                entity.Property(m => m.Status).HasConversion<string>().HasMaxLength(16);
                entity.Property(m => m.CurrentBalance).HasColumnType("decimal(18,2)");

                entity.HasOne(m => m.Tenant)
                    .WithMany()
                    .HasForeignKey(m => m.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.User)
                    .WithMany()
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // إذا حُذف المستخدم، تُحذف عضوياته

                entity.HasIndex(m => new { m.TenantId, m.UserId }).IsUnique();
                entity.HasIndex(m => m.UserId);
            });

            // 4. علاقة إعدادات السنتر بالسنتر (One-to-One)
            builder.Entity<TenantSettings>()
                .HasOne<Tenant>()
                .WithOne()
                .HasForeignKey<TenantSettings>(ts => ts.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. ارتباط الكيانات التابعة للسنتر به (Restrict لمنع حذف سنتر بيانات تابعة له بالخطأ)
            builder.Entity<Group>()
                .HasOne<Tenant>().WithMany().HasForeignKey(g => g.TenantId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Session>()
                .HasOne<Tenant>().WithMany().HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttendanceRecord>()
                .HasOne<Tenant>().WithMany().HasForeignKey(a => a.TenantId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne<Tenant>().WithMany().HasForeignKey(t => t.TenantId).OnDelete(DeleteBehavior.Restrict);

            // 5.1 المجموعة: تنعقد في سنتر، واسمها فريد داخل المدرس الواحد
            builder.Entity<Group>(entity =>
            {
                entity.Property(g => g.Name).HasMaxLength(150).IsRequired();
                entity.Property(g => g.GradeLevel).HasMaxLength(50).IsRequired();
                entity.Property(g => g.Room).HasMaxLength(50);
                entity.Property(g => g.WhatsAppGroupLink).HasMaxLength(500);

                entity.HasOne<Center>()
                    .WithMany()
                    .HasForeignKey(g => g.CenterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(g => new { g.TenantId, g.Name }).IsUnique();
                entity.HasIndex(g => g.CenterId);
            });

            // 5.2 قالب الجدول الأسبوعي: الحصة الواحدة (يوم وبداية) لا تتكرر داخل المجموعة
            builder.Entity<GroupScheduleSlot>(entity =>
            {
                entity.Property(s => s.Day).HasConversion<string>().HasMaxLength(16);

                entity.HasOne(s => s.Group)
                    .WithMany(g => g.Slots)
                    .HasForeignKey(s => s.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Tenant>()
                    .WithMany()
                    .HasForeignKey(s => s.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => new { s.GroupId, s.Day, s.StartTime }).IsUnique();
                entity.HasIndex(s => s.TenantId);
            });

            // 5.3 الحصة الفعلية: مرتبطة بحصة القالب (Restrict لتفادي تعارض مسارات الحذف)،
            // ولا تتكرر لنفس المجموعة في نفس اللحظة (يضمن أن التوليد المتكرر آمن)
            builder.Entity<Session>(entity =>
            {
                entity.HasOne<GroupScheduleSlot>()
                    .WithMany()
                    .HasForeignKey(s => s.SlotId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => new { s.GroupId, s.StartTime }).IsUnique();
                entity.HasIndex(s => new { s.TenantId, s.StartTime });
            });

            // 5.4 تسجيل الطالب في مجموعة: تسجيل نشط واحد فقط للطالب عند كل مدرس
            builder.Entity<GroupEnrollment>(entity =>
            {
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(16);

                entity.HasOne(e => e.Group).WithMany().HasForeignKey(e => e.GroupId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Tenant>().WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.TenantId, e.StudentId }).IsUnique().HasFilter("[Status] = 'Active'");
                entity.HasIndex(e => new { e.GroupId, e.Status });
                entity.HasIndex(e => e.StudentId);
            });

            // 5.5 طلب انضمام أو نقل: طلب معلّق واحد فقط للطالب عند كل مدرس
            builder.Entity<EnrollmentRequest>(entity =>
            {
                entity.Property(r => r.Type).HasConversion<string>().HasMaxLength(16);
                entity.Property(r => r.Status).HasConversion<string>().HasMaxLength(16);
                entity.Property(r => r.DecisionNote).HasMaxLength(300);

                entity.HasOne(r => r.Group).WithMany().HasForeignKey(r => r.GroupId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Student).WithMany().HasForeignKey(r => r.StudentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Tenant>().WithMany().HasForeignKey(r => r.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(r => r.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => new { r.TenantId, r.StudentId }).IsUnique().HasFilter("[Status] = 'Pending'");
                entity.HasIndex(r => new { r.TenantId, r.Status });
                entity.HasIndex(r => r.StudentId);
            });

            // 6. علاقة المجموعات مع المساعدين (Many-to-Many)
            // المساعد يقدر يشرف على كذا مجموعة، والمجموعة يقدر يشرف عليها كذا مساعد
            builder.Entity<Group>()
                .HasMany(g => g.AssignedAdmins)
                .WithMany()
                .UsingEntity(j => j.ToTable("GroupAdmins")); // توليد جدول وسيط تلقائي

            // 7. علاقة المجموعة بالحصص (One-to-Many)
            builder.Entity<Group>()
                .HasMany(g => g.Sessions)
                .WithOne(s => s.Group)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // 8. علاقة الحصة مع سجلات الحضور (One-to-Many)
            builder.Entity<Session>()
                .HasMany(s => s.Attendances)
                .WithOne(a => a.Session)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict لمنع تعارضات الحذف في SQL Server

            // 9. علاقة الطالب مع سجلات حضوره (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Attendances)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttendanceRecord>(entity =>
            {
                // الطالب يُسجَّل في الحصة الواحدة مرة واحدة فقط
                entity.HasIndex(a => new { a.SessionId, a.StudentId }).IsUnique();

                // من سجّل الحضور (للتوثيق والمساءلة)
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(a => a.RecordedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 10. علاقة المعاملات المالية بالطالب (One-to-Many)
            builder.Entity<Transaction>()
                .HasOne(t => t.Student)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 11. علاقة المعاملات المالية بالمساعد الذي استلم الأموال (One-to-Many)
            builder.Entity<Transaction>()
                .HasOne<ApplicationUser>()
                .WithMany() // بدون Navigation Property عكسية لمنع تضخم الكلاس
                .HasForeignKey(t => t.ProcessedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // ضبط نوع البيانات للأموال لتكون دقيقة ولا تسبب مشاكل (Precision and Scale)
            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Transaction>()
                .HasOne<Session>()
                .WithMany() // المعاملة مرتبطة بجلسة، ولكن الجلسة لا تحتاج لقائمة بجميع معاملاتها
                .HasForeignKey(t => t.RelatedSessionId)
                .OnDelete(DeleteBehavior.SetNull); // إذا حذفت الجلسة، يبقى السجل المالي

            // فلتر عزل السناتر على كل كيان يطبّق ITenantOwned (تلقائي، فلا يُنسى كيان جديد)
            ApplyTenantQueryFilters(builder);

            // آخر خطوة: تطبيق قاعدة UTC على كل خصائص DateTime في كل الكيانات
            ApplyUtcDateTimeConverters(builder);
        }

        private static readonly MethodInfo SetTenantFilterMethod = typeof(AppDbContext)
            .GetMethod(nameof(SetTenantFilter), BindingFlags.Instance | BindingFlags.NonPublic)!;

        private void ApplyTenantQueryFilters(ModelBuilder builder)
        {
            var tenantOwnedTypes = builder.Model.GetEntityTypes()
                .Select(entityType => entityType.ClrType)
                .Where(clrType => typeof(ITenantOwned).IsAssignableFrom(clrType))
                .ToList();

            foreach (var clrType in tenantOwnedTypes)
            {
                SetTenantFilterMethod.MakeGenericMethod(clrType).Invoke(this, new object[] { builder });
            }
        }

        private void SetTenantFilter<TEntity>(ModelBuilder builder) where TEntity : class
        {
            builder.Entity<TEntity>()
                .HasQueryFilter(e => EF.Property<Guid>(e, nameof(ITenantOwned.TenantId)) == CurrentTenantId);
        }

        // شبكة أمان للكتابة: لو في سنتر محدد، يُمنع حفظ أي كيان تابع لسنتر آخر
        // (بدون سنتر محدد، مثل الـ Seeder، لا يتدخل لأن الكود هو من يحدد TenantId صراحةً)
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            EnsureTenantIsolation();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            EnsureTenantIsolation();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void EnsureTenantIsolation()
        {
            var currentTenantId = CurrentTenantId;
            if (currentTenantId == Guid.Empty)
            {
                return;
            }

            var crossTenantEntry = ChangeTracker.Entries<ITenantOwned>()
                .FirstOrDefault(entry =>
                    entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
                    && entry.Entity.TenantId != currentTenantId);

            if (crossTenantEntry is not null)
            {
                throw new InvalidOperationException(
                    $"Cross-tenant write blocked for {crossTenantEntry.Entity.GetType().Name}.");
            }
        }

        private static void ApplyUtcDateTimeConverters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(UtcConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(NullableUtcConverter);
                    }
                }
            }
        }
    }
}