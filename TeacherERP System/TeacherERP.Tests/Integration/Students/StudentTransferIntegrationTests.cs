using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Integration;

public sealed class StudentTransferIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    [Fact]
    public async Task EnrollmentRequestDecisionsNotifyTheStudentOnce()
    {
        SeededTeacher teacher = null!;
        Guid approvedStudentId = default, rejectedStudentId = default;
        Guid approvedRequestId = default, rejectedRequestId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            approvedStudentId = await TestData.AddStudentAsync(db, teacher.TenantId);
            rejectedStudentId = await TestData.AddStudentAsync(db, teacher.TenantId);
            var approvedRequest = EnrollmentRequest.Create(
                teacher.TenantId, approvedStudentId, teacher.GroupId, EnrollmentRequestType.Join);
            var rejectedRequest = EnrollmentRequest.Create(
                teacher.TenantId, rejectedStudentId, teacher.GroupId, EnrollmentRequestType.Join);
            db.EnrollmentRequests.AddRange(approvedRequest, rejectedRequest);
            await db.SaveChangesAsync();
            approvedRequestId = approvedRequest.Id;
            rejectedRequestId = rejectedRequest.Id;
        });

        var staff = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var approved = await staff.PostAsync($"/api/Enrollment/requests/{approvedRequestId}/approve", null);
        var rejected = await staff.PostAsJsonAsync($"/api/Enrollment/requests/{rejectedRequestId}/reject", new
        {
            note = "The group is not suitable"
        });

        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        Assert.Equal(HttpStatusCode.OK, rejected.StatusCode);
        Assert.Equal(1, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(n =>
            n.TenantId == teacher.TenantId && n.RecipientUserId == approvedStudentId
            && n.Type == NotificationType.EnrollmentApproved && n.RelatedId == approvedRequestId)));
        Assert.Equal(1, await _factory.QueryAsync(db => db.Notifications.IgnoreQueryFilters().CountAsync(n =>
            n.TenantId == teacher.TenantId && n.RecipientUserId == rejectedStudentId
            && n.Type == NotificationType.EnrollmentRejected && n.RelatedId == rejectedRequestId)));
    }

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
        var old = Assert.Single(rows, e => e.GroupId == teacher.GroupId);
        var current = Assert.Single(rows, e => e.Status == EnrollmentStatus.Active);
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
    public async Task MoveAndApprovedStudentTransfer_RejectDifferentStructuredGradesWithoutChangingEnrollment()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid targetGroupId = default;
        Guid requestId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var target = Group.Create(teacher.TenantId, $"Secondary {Guid.NewGuid():N}",
                new SchoolGrade(EducationStage.Secondary, 1), teacher.CenterId, null, null, null);
            db.Groups.Add(target);
            var pending = EnrollmentRequest.Create(teacher.TenantId, studentId, target.Id, EnrollmentRequestType.Transfer);
            db.EnrollmentRequests.Add(pending);
            await db.SaveChangesAsync();
            targetGroupId = target.Id;
            requestId = pending.Id;
        });
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var directMove = await client.PostAsJsonAsync(
            $"api/Enrollment/students/{studentId}/move", new { targetGroupId });
        var requestApproval = await client.PostAsync($"api/Enrollment/requests/{requestId}/approve", null);

        Assert.Equal(HttpStatusCode.BadRequest, directMove.StatusCode);
        Assert.Contains("Enrollment.GradeLevelMismatch", await directMove.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.BadRequest, requestApproval.StatusCode);
        Assert.Contains("Enrollment.GradeLevelMismatch", await requestApproval.Content.ReadAsStringAsync());
        Assert.True(await _factory.QueryAsync(db => db.GroupEnrollments.IgnoreQueryFilters().AnyAsync(e =>
            e.TenantId == teacher.TenantId && e.StudentId == studentId && e.GroupId == teacher.GroupId && e.Status == EnrollmentStatus.Active)));
        Assert.Equal(EnrollmentRequestStatus.Pending, await _factory.QueryAsync(db => db.EnrollmentRequests.IgnoreQueryFilters()
            .Where(r => r.Id == requestId).Select(r => r.Status).SingleAsync()));
    }

    [Fact]
    public async Task Move_RejectsLegacyGroupsUntilStructuredGradeIsSet()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid targetGroupId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var target = Group.Create(teacher.TenantId, $"Structured {Guid.NewGuid():N}",
                new SchoolGrade(EducationStage.Primary, 1), teacher.CenterId, null, null, null);
            db.Groups.Add(target);
            targetGroupId = target.Id;
            var legacySource = Group.Create(teacher.TenantId, $"Source {Guid.NewGuid():N}",
                "Prep 1", teacher.CenterId, null, null, null);
            db.Groups.Add(legacySource);
            // Preserve this scenario as a legacy source by moving the enrollment before the HTTP operation.
            var oldEnrollment = await db.GroupEnrollments.IgnoreQueryFilters().SingleAsync(e => e.StudentId == studentId);
            oldEnrollment.End(DateTime.UtcNow);
            await db.SaveChangesAsync();
            db.GroupEnrollments.Add(GroupEnrollment.Create(teacher.TenantId, legacySource.Id, studentId, DateTime.UtcNow, null));
            await db.SaveChangesAsync();
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Enrollment.GradeLevelNotConfigured", await response.Content.ReadAsStringAsync());
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
            var fullGroup = Group.Create(teacher.TenantId, $"Full {Guid.NewGuid():N}", new SchoolGrade(EducationStage.Primary, 1), teacher.CenterId, null, 1, null);
            db.Groups.Add(fullGroup);
            targetGroupId = fullGroup.Id;
            await TestData.AddStudentAsync(db, teacher.TenantId, targetGroupId);
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
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

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
            var target = Group.Create(teacher.TenantId, $"Archived {Guid.NewGuid():N}", new SchoolGrade(EducationStage.Primary, 1), teacher.CenterId, null, null, null);
            target.SetActive(false);
            db.Groups.Add(target);
            await db.SaveChangesAsync();
            targetGroupId = target.Id;
        });

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync($"api/Enrollment/students/{studentId}/move", new { targetGroupId });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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
        Assert.Equal(HttpStatusCode.Forbidden, notEnrolled.StatusCode);
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
