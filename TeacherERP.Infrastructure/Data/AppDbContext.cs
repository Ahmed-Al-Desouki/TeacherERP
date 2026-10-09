using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeacherERP.Domain.Entities;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // تسجيل الجداول في قاعدة البيانات
        public DbSet<LoginSession> LoginSessions { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }
        public DbSet<Group> Groups { get; set; }
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

            // 2. علاقة إعدادات السنتر مع المدرس/SuperAdmin (One-to-One)
            builder.Entity<TenantSettings>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<TenantSettings>(ts => ts.SuperAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. علاقة المجموعات مع المساعدين (Many-to-Many)
            // المساعد يقدر يشرف على كذا مجموعة، والمجموعة يقدر يشرف عليها كذا مساعد
            builder.Entity<Group>()
                .HasMany(g => g.AssignedAdmins)
                .WithMany()
                .UsingEntity(j => j.ToTable("GroupAdmins")); // توليد جدول وسيط تلقائي

            // 4. علاقة المجموعة بالحصص (One-to-Many)
            builder.Entity<Group>()
                .HasMany(g => g.Sessions)
                .WithOne(s => s.Group)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. علاقة الحصة مع سجلات الحضور (One-to-Many)
            builder.Entity<Session>()
                .HasMany(s => s.Attendances)
                .WithOne(a => a.Session)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict لمنع تعارضات الحذف في SQL Server

            // 6. علاقة الطالب مع سجلات حضوره (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Attendances)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 7. علاقة المعاملات المالية بالطالب (One-to-Many)
            builder.Entity<Transaction>()
                .HasOne(t => t.Student)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 8. علاقة المعاملات المالية بالمساعد الذي استلم الأموال (One-to-Many)
            builder.Entity<Transaction>()
                .HasOne<ApplicationUser>()
                .WithMany() // بدون Navigation Property عكسية لمنع تضخم الكلاس
                .HasForeignKey(t => t.ProcessedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // ضبط نوع البيانات للأموال لتكون دقيقة ولا تسبب مشاكل (Precision and Scale)
            builder.Entity<ApplicationUser>()
                .Property(u => u.CurrentBalance)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Transaction>()
                .HasOne<Session>()
                .WithMany() // المعاملة مرتبطة بجلسة، ولكن الجلسة لا تحتاج لقائمة بجميع معاملاتها
                .HasForeignKey(t => t.RelatedSessionId)
                .OnDelete(DeleteBehavior.SetNull); // إذا حذفت الجلسة، يبقى السجل المالي

            // آخر خطوة: تطبيق قاعدة UTC على كل خصائص DateTime في كل الكيانات
            ApplyUtcDateTimeConverters(builder);
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