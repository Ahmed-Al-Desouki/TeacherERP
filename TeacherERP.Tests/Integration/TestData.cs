using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Tests.Integration;

public sealed record SeededTeacher(Guid TenantId, Guid OwnerId, Guid CenterId, Guid GroupId);

// بناء بيانات الاختبار مباشرة في الداتابيز (كل اختبار ياخد مدرسه الخاص فلا تتداخل البيانات)
public static class TestData
{
    private static int _counter;

    private static int Next() => Interlocked.Increment(ref _counter);

    private static string NextPhone() => $"+20100{Next():D7}";

    // مدرس كامل: حساب المالك، مساحته (Tenant)، سنتره المكاني، عضوية Owner، إعدادات الحضور، ومجموعة واحدة
    public static async Task<SeededTeacher> CreateTeacherAsync(AppDbContext db)
    {
        var n = Next();

        var owner = ApplicationUser.CreateAdmin($"Teacher {n}", $"teacher{n}@test.local", NextPhone());
        db.Users.Add(owner);

        var tenant = Tenant.Create(owner.FullName, owner.Id, "UTC");
        var center = Center.Create($"Center {n}", owner.Id);
        db.Tenants.Add(tenant);
        db.Centers.Add(center);

        db.CenterTeachers.Add(CenterTeacher.CreateFounder(center.Id, tenant.Id));
        db.TenantMembers.Add(TenantMember.Create(tenant.Id, owner.Id, TenantRole.Owner));
        db.TenantSettings.Add(TenantSettings.Create(tenant.Id));

        var group = Group.Create(tenant.Id, $"Group {n}", "Grade 1", center.Id, null, null, null);
        db.Groups.Add(group);

        await db.SaveChangesAsync();

        return new SeededTeacher(tenant.Id, owner.Id, center.Id, group.Id);
    }

    public static async Task<Guid> AddGroupAsync(AppDbContext db, SeededTeacher teacher)
    {
        var group = Group.Create(teacher.TenantId, $"Group {Next()}", "Grade 1", teacher.CenterId, null, null, null);
        db.Groups.Add(group);
        await db.SaveChangesAsync();
        return group.Id;
    }

    // طالب بعضوية عند المدرس، ومسجّل في مجموعة لو اتحددت (الحساب عادي، والدور يحدده TenantMember)
    public static async Task<Guid> AddStudentAsync(
        AppDbContext db, Guid tenantId, Guid? enrolledGroupId = null, bool withMembership = true)
    {
        var n = Next();

        var student = ApplicationUser.CreateAdmin($"Student {n}", $"student{n}@test.local", NextPhone());
        db.Users.Add(student);

        if (withMembership)
        {
            db.TenantMembers.Add(TenantMember.Create(tenantId, student.Id, TenantRole.Student));
        }

        if (enrolledGroupId is { } groupId)
        {
            db.GroupEnrollments.Add(GroupEnrollment.Create(tenantId, groupId, student.Id, DateTime.UtcNow, null));
        }

        await db.SaveChangesAsync();
        return student.Id;
    }

    public static async Task<Guid> AddSessionAsync(
        AppDbContext db, Guid tenantId, Guid groupId, DateTime startUtc, TimeSpan? duration = null)
    {
        var session = Session.Create(tenantId, groupId, startUtc, startUtc.Add(duration ?? TimeSpan.FromHours(1)));
        db.Sessions.Add(session);
        await db.SaveChangesAsync();
        return session.Id;
    }

    // الطالب حضر sessionId كتعويض معلّق عن compensatesSessionId (حصة مجموعته الأصلية)
    public static async Task<Guid> AddPendingCompensationAsync(
        AppDbContext db, Guid tenantId, Guid sessionId, Guid studentId, Guid? compensatesSessionId)
    {
        var record = AttendanceRecord.RecordPresence(
            tenantId, sessionId, studentId, ScanMethod.SelfScan, 30.0444, 31.2357,
            null, AttendanceStatus.PendingCompensation, compensatesSessionId);
        db.AttendanceRecords.Add(record);
        await db.SaveChangesAsync();
        return record.Id;
    }
}
