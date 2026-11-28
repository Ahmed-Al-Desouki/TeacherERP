using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Integration;

public sealed class GroupJoinCodeIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GroupJoinCodeIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task OwnerCanReadCodes_AndStudentsCanRequestByStaticAndDynamicCode()
    {
        SeededTeacher teacher = default!;
        Guid secondGroupId = Guid.Empty;
        Guid staticStudentId = Guid.Empty;
        Guid dynamicStudentId = Guid.Empty;

        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            secondGroupId = await TestData.AddGroupAsync(db, teacher);
            staticStudentId = await AddCompleteStudentAsync(db, teacher.TenantId);
            dynamicStudentId = await AddCompleteStudentAsync(db, teacher.TenantId);
        });

        using var ownerClient = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var staticCodesResponse = await ownerClient.GetAsync($"/api/Groups/{teacher.GroupId}/join-codes");
        var dynamicCodesResponse = await ownerClient.GetAsync($"/api/Groups/{secondGroupId}/join-codes");

        Assert.Equal(HttpStatusCode.OK, staticCodesResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, dynamicCodesResponse.StatusCode);
        var staticCodes = await staticCodesResponse.Content.ReadFromJsonAsync<JoinCodesResponse>();
        var dynamicCodes = await dynamicCodesResponse.Content.ReadFromJsonAsync<JoinCodesResponse>();
        Assert.NotNull(staticCodes?.StaticCode);
        Assert.NotNull(staticCodes?.StaticJoinUrl);
        Assert.StartsWith("D.", dynamicCodes?.DynamicCode);
        Assert.NotNull(dynamicCodes?.DynamicJoinUrl);
        Assert.InRange(dynamicCodes!.DynamicExpiresInSeconds, 1, 30);

        using var staticStudentClient = _factory.CreateClientFor(staticStudentId, role: "Student");
        using var dynamicStudentClient = _factory.CreateClientFor(dynamicStudentId, role: "Student");
        var staticRequest = await staticStudentClient.PostAsJsonAsync(
            "/api/Enrollment/requests/by-code", new { code = staticCodes!.StaticCode });
        var dynamicRequest = await dynamicStudentClient.PostAsJsonAsync(
            "/api/Enrollment/requests/by-code", new { code = dynamicCodes!.DynamicCode });

        Assert.Equal(HttpStatusCode.OK, staticRequest.StatusCode);
        Assert.Equal(HttpStatusCode.OK, dynamicRequest.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.EnrollmentRequests.IgnoreQueryFilters()
            .AnyAsync(request => request.TenantId == teacher.TenantId && request.StudentId == staticStudentId
                && request.GroupId == teacher.GroupId)));
        Assert.True(await _factory.QueryAsync(db => db.EnrollmentRequests.IgnoreQueryFilters()
            .AnyAsync(request => request.TenantId == teacher.TenantId && request.StudentId == dynamicStudentId
                && request.GroupId == secondGroupId)));
    }

    [Fact]
    public async Task DisabledStaticCode_IsRejectedWithoutCreatingEnrollmentRequest()
    {
        SeededTeacher teacher = default!;
        Guid studentId = Guid.Empty;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await AddCompleteStudentAsync(db, teacher.TenantId);
        });

        using var ownerClient = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var codeResponse = await ownerClient.GetAsync($"/api/Groups/{teacher.GroupId}/join-codes");
        var codes = await codeResponse.Content.ReadFromJsonAsync<JoinCodesResponse>();
        Assert.Equal(HttpStatusCode.OK, codeResponse.StatusCode);

        var updateResponse = await ownerClient.PutAsJsonAsync(
            $"/api/Groups/{teacher.GroupId}/join-codes",
            new { staticEnabled = false, dynamicEnabled = true });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using var studentClient = _factory.CreateClientFor(studentId, role: "Student");
        var joinResponse = await studentClient.PostAsJsonAsync(
            "/api/Enrollment/requests/by-code", new { code = codes!.StaticCode });

        Assert.Equal(HttpStatusCode.NotFound, joinResponse.StatusCode);
        Assert.False(await _factory.QueryAsync(db => db.EnrollmentRequests.IgnoreQueryFilters()
            .AnyAsync(request => request.TenantId == teacher.TenantId && request.StudentId == studentId)));
    }

    [Fact]
    public async Task RotatingDynamicCode_InvalidatesOldValueAndKeepsStaticCodeStable()
    {
        SeededTeacher teacher = default!;
        Guid studentId = Guid.Empty;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            studentId = await AddCompleteStudentAsync(db, teacher.TenantId);
        });

        using var ownerClient = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);
        var beforeResponse = await ownerClient.GetAsync($"/api/Groups/{teacher.GroupId}/join-codes");
        Assert.Equal(HttpStatusCode.OK, beforeResponse.StatusCode);
        var before = await beforeResponse.Content.ReadFromJsonAsync<JoinCodesResponse>();

        var rotateResponse = await ownerClient.PutAsJsonAsync(
            $"/api/Groups/{teacher.GroupId}/join-codes",
            new { staticEnabled = true, dynamicEnabled = true, rotateDynamicCode = true });
        Assert.Equal(HttpStatusCode.OK, rotateResponse.StatusCode);
        var after = await rotateResponse.Content.ReadFromJsonAsync<JoinCodesResponse>();
        Assert.Equal(before!.StaticCode, after!.StaticCode);
        Assert.NotEqual(before.DynamicCode, after.DynamicCode);

        using var studentClient = _factory.CreateClientFor(studentId, role: "Student");
        var oldCodeResponse = await studentClient.PostAsJsonAsync(
            "/api/Enrollment/requests/by-code", new { code = before.DynamicCode });
        Assert.Equal(HttpStatusCode.NotFound, oldCodeResponse.StatusCode);
        Assert.False(await _factory.QueryAsync(db => db.EnrollmentRequests.IgnoreQueryFilters()
            .AnyAsync(request => request.TenantId == teacher.TenantId && request.StudentId == studentId)));
    }

    private static async Task<Guid> AddCompleteStudentAsync(
        TeacherERP.Infrastructure.Data.AppDbContext db, Guid tenantId)
    {
        var studentId = await TestData.AddStudentAsync(db, tenantId, withMembership: false);
        var student = await db.Users.IgnoreQueryFilters().SingleAsync(user => user.Id == studentId);
        student.CompleteProfile(student.PhoneNumber!,
            GuardianInfo.Create("Guardian", "+201155500001", GuardianRelation.Father), null, null);
        await db.SaveChangesAsync();
        return studentId;
    }

    private sealed record JoinCodesResponse(
        Guid GroupId,
        bool StaticEnabled,
        string? StaticCode,
        string? StaticJoinUrl,
        bool DynamicEnabled,
        string? DynamicCode,
        string? DynamicJoinUrl,
        int DynamicExpiresInSeconds,
        DateTime? DynamicExpiresAtUtc);
}
