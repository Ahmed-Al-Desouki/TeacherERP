using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration;

public sealed class StudentTransferIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public StudentTransferIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task StaffMove_EndsOldEnrollment_CancelsPendingRequest_AndPreservesMembershipHistory()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid targetGroupId = default;
        Guid pendingRequestId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            targetGroupId = await TestData.AddGroupAsync(db, teacher);
            var oldEnrollment = await db.GroupEnrollments.IgnoreQueryFilters()
                .SingleAsync(e => e.TenantId == teacher.TenantId && e.StudentId == studentId);
            oldEnrollment.AppointAsGroupAdmin(DateTime.UtcNow);
            var pending = EnrollmentRequest.Create(teacher.TenantId, studentId, targetGroupId, EnrollmentRequestType.Transfer);
            db.EnrollmentRequests.Add(pending);
            await db.SaveChangesAsync();
            pendingRequestId = pending.Id;
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rows = await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters()
            .Where(e => e.TenantId == teacher.TenantId && e.StudentId == studentId).ToListAsync());
        var old = Assert.Single(rows.Where(e => e.GroupId == teacher.GroupId));
        var current = Assert.Single(rows.Where(e => e.Status == EnrollmentStatus.Active));
        Assert.Equal(EnrollmentStatus.Ended, old.Status);
        Assert.NotNull(old.EndedAt);
        Assert.False(old.IsGroupAdmin);
        Assert.Equal(targetGroupId, current.GroupId);
        Assert.Equal(teacher.OwnerId, current.ApprovedByUserId);
        Assert.True(await _factory.QueryAsync(db => db.TenantMembers.IgnoreQueryFilters()
            .AnyAsync(m => m.TenantId == teacher.TenantId && m.UserId == studentId)));
        Assert.Equal(EnrollmentRequestStatus.Cancelled, await _factory.QueryAsync(db => db.EnrollmentRequests
            .IgnoreQueryFilters().Where(r => r.Id == pendingRequestId).Select(r => r.Status).SingleAsync()));
    }

    [Fact]
    public async Task AssistantCanMoveStudentInTheirTeacherSpace()
    {
        SeededTeacher teacher = null!;
        Guid assistantId = default;
        Guid studentId = default;
        Guid targetGroupId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            targetGroupId = await TestData.AddGroupAsync(db, teacher);
            var assistant = ApplicationUser.CreateAdmin("Assistant", $"assistant-{Guid.NewGuid():N}@test.local", $"+201{Random.Shared.Next(100_000_000, 1_000_000_000)}");
            assistantId = assistant.Id;
            db.Users.Add(assistant);
            db.TenantMembers.Add(TenantMember.Create(teacher.TenantId, assistant.Id, TenantRole.Assistant));
            await db.SaveChangesAsync();
        });

        var response = await _factory.CreateClientFor(assistantId, teacher.TenantId, "Assistant")
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Move_RejectsFullTargetAndKeepsCurrentEnrollment()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid targetGroupId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var fullGroup = Group.Create(teacher.TenantId, $"Full {Guid.NewGuid():N}", "Grade 1", teacher.CenterId, null, 1, null);
            db.Groups.Add(fullGroup);
            targetGroupId = fullGroup.Id;
            await TestData.AddStudentAsync(db, teacher.TenantId, targetGroupId);
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Enrollment.GroupFull", await response.Content.ReadAsStringAsync());
        Assert.True(await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters().AnyAsync(e =>
            e.TenantId == teacher.TenantId && e.StudentId == studentId && e.GroupId == teacher.GroupId && e.Status == EnrollmentStatus.Active)));
    }

    [Fact]
    public async Task Move_RejectsTargetFromDifferentTeacher()
    {
        SeededTeacher teacher = null!;
        SeededTeacher otherTeacher = null!;
        Guid studentId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            otherTeacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId = otherTeacher.GroupId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Enrollment.GroupNotFound", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Move_RejectsArchivedTarget()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid targetGroupId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var target = Group.Create(teacher.TenantId, $"Archived {Guid.NewGuid():N}", "Grade 1", teacher.CenterId, null, null, null);
            target.SetActive(false);
            db.Groups.Add(target);
            await db.SaveChangesAsync();
            targetGroupId = target.Id;
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Enrollment.GroupNotFound", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Move_RejectsBlacklistedMembership()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid targetGroupId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            targetGroupId = await TestData.AddGroupAsync(db, teacher);
            var member = await db.TenantMembers.IgnoreQueryFilters()
                .SingleAsync(m => m.TenantId == teacher.TenantId && m.UserId == studentId);
            member.Blacklist();
            await db.SaveChangesAsync();
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Enrollment.NotAllowed", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Move_RejectsSameGroupAndStudentWithoutCurrentEnrollment()
    {
        SeededTeacher teacher = null!;
        Guid enrolledStudentId = default;
        Guid unenrolledStudentId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            enrolledStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            unenrolledStudentId = await TestData.AddStudentAsync(db, teacher.TenantId);
        });
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var sameGroup = await client.PostAsJsonAsync(
            $"api/Enrollment/students/{enrolledStudentId}/move", new { targetGroupId = teacher.GroupId });
        var notEnrolled = await client.PostAsJsonAsync(
            $"api/Enrollment/students/{unenrolledStudentId}/move", new { targetGroupId = teacher.GroupId });

        Assert.Equal(HttpStatusCode.BadRequest, sameGroup.StatusCode);
        Assert.Contains("Enrollment.SameGroup", await sameGroup.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.BadRequest, notEnrolled.StatusCode);
        Assert.Contains("Enrollment.NotEnrolled", await notEnrolled.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Move_RequiresTenantStaffAndAuthentication()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid targetGroupId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            targetGroupId = await TestData.AddGroupAsync(db, teacher);
        });
        var route = $"api/Enrollment/students/{studentId}/move";

        var studentResponse = await _factory.CreateClientFor(studentId, teacher.TenantId, "Student")
            .PostAsJsonAsync(route, new { targetGroupId });
        var anonymousResponse = await _factory.CreateClient()
            .PostAsJsonAsync(route, new { targetGroupId });

        Assert.Equal(HttpStatusCode.Forbidden, studentResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);
    }
}
