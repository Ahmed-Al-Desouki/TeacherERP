
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Integration;

public sealed class StudentExperienceIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public StudentExperienceIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task GetMyCard_ReturnsTheAuthenticatedStudentsQr_AndRestrictsTheEndpointToStudents()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        Guid otherStudentId = default;
        string expectedQrCode = string.Empty;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            otherStudentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            expectedQrCode = await db.Users.IgnoreQueryFilters().Where(user => user.Id == studentId)
                .Select(user => user.StaticQrCode).SingleAsync();
        });

        var studentResponse = await _factory.CreateClientFor(studentId, teacher.TenantId, "Student")
            .GetAsync("/api/students/me/card");
        Assert.Equal(HttpStatusCode.OK, studentResponse.StatusCode);
        var card = await studentResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(studentId, card.GetProperty("studentId").GetGuid());
        Assert.Equal(expectedQrCode, card.GetProperty("qrCode").GetString());

        var otherStudentResponse = await _factory.CreateClientFor(otherStudentId, teacher.TenantId, "Student")
            .GetAsync("/api/students/me/card");
        Assert.Equal(HttpStatusCode.OK, otherStudentResponse.StatusCode);
        var otherCard = await otherStudentResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(otherStudentId, otherCard.GetProperty("studentId").GetGuid());
        Assert.NotEqual(expectedQrCode, otherCard.GetProperty("qrCode").GetString());

        var staffResponse = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .GetAsync("/api/students/me/card");
        Assert.Equal(HttpStatusCode.Forbidden, staffResponse.StatusCode);

        var anonymous = await _factory.CreateClient().GetAsync("/api/students/me/card");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);
    }

    [Fact]
    public async Task GetMySchedule_ReturnsOnlyActiveGroupsInTheSelectedTenantWithWeeklySlotsOrderedByDayAndTime()
    {
        SeededTeacher teacher = null!;
        SeededTeacher otherTeacher = null!;
        Guid studentId = default;
        Guid activeGroupWithSlotsId = default;
        Guid endedGroupId = default;
        Guid unmemberedStudentId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            otherTeacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);

            var activeGroupWithSlots = await db.Groups.IgnoreQueryFilters()
                .SingleAsync(group => group.Id == teacher.GroupId);
            db.GroupScheduleSlots.Add(activeGroupWithSlots.AddSlot(
                DayOfWeek.Sunday, TimeSpan.FromHours(16), 60));
            db.GroupScheduleSlots.Add(activeGroupWithSlots.AddSlot(
                DayOfWeek.Saturday, TimeSpan.FromHours(17), 90));
            db.GroupScheduleSlots.Add(activeGroupWithSlots.AddSlot(
                DayOfWeek.Saturday, TimeSpan.FromHours(9), 75));

            endedGroupId = await TestData.AddGroupAsync(db, teacher);
            var endedEnrollment = GroupEnrollment.Create(teacher.TenantId, endedGroupId,
                studentId, DateTime.UtcNow, teacher.OwnerId);
            endedEnrollment.End(DateTime.UtcNow);
            db.GroupEnrollments.Add(endedEnrollment);

            db.TenantMembers.Add(TenantMember.Create(otherTeacher.TenantId, studentId, TenantRole.Student));
            db.GroupEnrollments.Add(GroupEnrollment.Create(otherTeacher.TenantId, otherTeacher.GroupId,
                studentId, DateTime.UtcNow, otherTeacher.OwnerId));
            var otherGroup = await db.Groups.IgnoreQueryFilters()
                .SingleAsync(group => group.Id == otherTeacher.GroupId);
            db.GroupScheduleSlots.Add(otherGroup.AddSlot(
                DayOfWeek.Monday, TimeSpan.FromHours(12), 60));
            unmemberedStudentId = await TestData.AddStudentAsync(
                db, teacher.TenantId, withMembership: false);
            await db.SaveChangesAsync();

            activeGroupWithSlotsId = activeGroupWithSlots.Id;
        });

        var response = await _factory.CreateClientFor(studentId, teacher.TenantId, "Student")
            .GetAsync("/api/students/me/schedule");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var schedule = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("UTC", schedule.GetProperty("timeZoneId").GetString());
        var groups = schedule.GetProperty("groups").EnumerateArray().ToArray();
        Assert.Contains(groups, group => group.GetProperty("groupId").GetGuid() == activeGroupWithSlotsId);
        Assert.DoesNotContain(groups, group => group.GetProperty("groupId").GetGuid() == endedGroupId);
        Assert.DoesNotContain(groups, group => group.GetProperty("groupId").GetGuid() == otherTeacher.GroupId);

        var activeGroup = Assert.Single(groups, group =>
            group.GetProperty("groupId").GetGuid() == activeGroupWithSlotsId);
        var slots = activeGroup.GetProperty("slots").EnumerateArray().ToArray();
        Assert.Equal(3, slots.Length);
        Assert.Equal("Saturday", slots[0].GetProperty("day").GetString());
        Assert.Equal("09:00:00", slots[0].GetProperty("startTime").GetString());
        Assert.Equal(75, slots[0].GetProperty("durationMinutes").GetInt32());
        Assert.Equal("Saturday", slots[1].GetProperty("day").GetString());
        Assert.Equal("17:00:00", slots[1].GetProperty("startTime").GetString());
        Assert.Equal("Sunday", slots[2].GetProperty("day").GetString());

        var unmemberedStudentResponse = await _factory.CreateClientFor(
                unmemberedStudentId, teacher.TenantId, "Student")
            .GetAsync("/api/students/me/schedule");
        Assert.Equal(HttpStatusCode.Forbidden, unmemberedStudentResponse.StatusCode);
    }

    [Fact]
    public async Task GetMySchedule_IncludesAnActiveEnrolledGroupWithoutWeeklySlots()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
        });

        var response = await _factory.CreateClientFor(studentId, teacher.TenantId, "Student")
            .GetAsync("/api/students/me/schedule");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var schedule = await response.Content.ReadFromJsonAsync<JsonElement>();
        var group = Assert.Single(schedule.GetProperty("groups").EnumerateArray());
        Assert.Equal(teacher.GroupId, group.GetProperty("groupId").GetGuid());
        Assert.Empty(group.GetProperty("slots").EnumerateArray());
    }

    [Fact]
    public async Task GetMySchedule_ExcludesAnArchivedGroupEvenWhenTheStudentHasAnActiveEnrollment()
    {
        SeededTeacher teacher = null!;
        Guid studentId = default;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            var group = await db.Groups.IgnoreQueryFilters()
                .SingleAsync(candidate => candidate.Id == teacher.GroupId);
            group.SetActive(false);
            await db.SaveChangesAsync();
        });

        var response = await _factory.CreateClientFor(studentId, teacher.TenantId, "Student")
            .GetAsync("/api/students/me/schedule");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var schedule = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Empty(schedule.GetProperty("groups").EnumerateArray());
    }

    [Fact]
    public async Task GetStudentDetails_ReturnsProfileAndActiveGroupsOnlyToStaffInTheSameTenant()
    {
        SeededTeacher teacher = null!;
        SeededTeacher otherTeacher = null!;
        Guid studentId = default;
        Guid otherTenantStudentId = default;
        string expectedEmail = string.Empty;

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            otherTeacher = await TestData.CreateTeacherAsync(db);
            studentId = await TestData.AddStudentAsync(db, teacher.TenantId, teacher.GroupId);
            otherTenantStudentId = await TestData.AddStudentAsync(
                db, otherTeacher.TenantId, otherTeacher.GroupId);

            var student = await db.Users.IgnoreQueryFilters()
                .SingleAsync(user => user.Id == studentId);
            expectedEmail = student.Email!;
            student.CompleteProfile(
                "+201000000001",
                GuardianInfo.Create("Guardian", "+201000000002", GuardianRelation.Father),
                Gender.Female,
                new DateOnly(2010, 5, 12));
            await db.SaveChangesAsync();
        });

        var staffClient = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var response = await staffClient.GetAsync($"/api/students/{studentId}/details");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var details = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(studentId, details.GetProperty("studentId").GetGuid());
        Assert.Equal(expectedEmail, details.GetProperty("email").GetString());
        Assert.Equal("+201000000001", details.GetProperty("phoneNumber").GetString());
        Assert.Equal("Guardian", details.GetProperty("guardianName").GetString());
        Assert.Equal("+201000000002", details.GetProperty("guardianPhoneNumber").GetString());
        Assert.Equal(100, details.GetProperty("profileCompletionPercentage").GetInt32());
        var groups = details.GetProperty("activeGroups").EnumerateArray().ToArray();
        var group = Assert.Single(groups);
        Assert.Equal(teacher.GroupId, group.GetProperty("groupId").GetGuid());

        var unrelatedTenantStudent = await staffClient.GetAsync(
            $"/api/students/{otherTenantStudentId}/details");
        Assert.Equal(HttpStatusCode.NotFound, unrelatedTenantStudent.StatusCode);

        var studentClient = _factory.CreateClientFor(studentId, teacher.TenantId, "Student");
        var forbidden = await studentClient.GetAsync($"/api/students/{studentId}/details");
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);

        var anonymous = await _factory.CreateClient()
            .GetAsync($"/api/students/{studentId}/details");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);
    }
}
