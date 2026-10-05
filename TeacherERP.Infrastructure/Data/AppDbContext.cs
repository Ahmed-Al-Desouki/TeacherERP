using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TeacherERP.Infrastructure.Data
{
    // توريث من IdentityDbContext لدمج جداول الـ Security جاهزة
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // تسجيل الجداول في قاعدة البيانات
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // سطر إجباري لتوليد جداول الـ Identity بشكل سليم
            base.OnModelCreating(builder);

            // 1. علاقة المستخدم مع RefreshTokens (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // إذا حُذف المستخدم، تُحذف الجلسات

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
        }
    }
}