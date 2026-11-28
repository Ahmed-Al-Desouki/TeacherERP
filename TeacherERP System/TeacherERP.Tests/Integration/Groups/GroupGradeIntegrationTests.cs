using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Integration;

public sealed class GroupGradeIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GroupGradeIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateAndUpdateGroup_PersistStructuredStageAndGrade()
    {
        SeededTeacher teacher = null!;
        await _factory.SeedAsync(async db => teacher = await TestData.CreateTeacherAsync(db));
        var client = _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId);

        var created = await client.PostAsJsonAsync("api/Groups", new
        {
            name = $"Structured {Guid.NewGuid():N}",
            grade = new { stage = "Preparatory", number = 2 },
            centerId = teacher.CenterId,
            room = "Room 2",
            capacity = 20,
            whatsappGroupLink = (string?)null,
            slots = new[] { new { day = "Monday", startTime = "15:00:00", durationMinutes = 60 } }
        });

        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        var body = await created.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var groupId = body.GetProperty("id").GetGuid();
        var createdGrade = body.GetProperty("grade");
        Assert.Equal("Preparatory", createdGrade.GetProperty("stage").GetString());
        Assert.Equal(2, createdGrade.GetProperty("number").GetInt32());

        var updated = await client.PutAsJsonAsync($"api/Groups/{groupId}", new
        {
            name = "Structured Grade Updated",
            grade = new { stage = "Secondary", number = 1 },
            centerId = teacher.CenterId,
            room = "Room 3",
            capacity = 25,
            whatsappGroupLink = (string?)null,
            isActive = true
        });

        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var persisted = await _factory.QueryAsync(db => db.Groups.IgnoreQueryFilters().SingleAsync(g => g.Id == groupId));
        Assert.Equal(EducationStage.Secondary, persisted.EducationStage);
        Assert.Equal(1, persisted.GradeNumber);
        Assert.Equal("Secondary 1", persisted.GradeLevel);
    }

    [Fact]
    public async Task CreateGroup_RejectsGradeNumberOutsideItsStage()
    {
        SeededTeacher teacher = null!;
        await _factory.SeedAsync(async db => teacher = await TestData.CreateTeacherAsync(db));

        var response = await _factory.CreateClientFor(teacher.OwnerId, teacher.TenantId)
            .PostAsJsonAsync("api/Groups", new
            {
                name = $"Invalid {Guid.NewGuid():N}",
                grade = new { stage = "Preparatory", number = 6 },
                centerId = teacher.CenterId,
                room = (string?)null,
                capacity = (int?)null,
                whatsappGroupLink = (string?)null,
                slots = new[] { new { day = "Monday", startTime = "15:00:00", durationMinutes = 60 } }
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Database_RejectsPartiallyConfiguredStructuredGrade()
    {
        SeededTeacher teacher = null!;
        await _factory.SeedAsync(async db => teacher = await TestData.CreateTeacherAsync(db));

        var exception = await Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() =>
            _factory.SeedAsync(db => db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Groups SET EducationStage = NULL, GradeNumber = 1 WHERE Id = {teacher.GroupId}")));

        Assert.Contains("CK_Groups_SchoolGrade", exception.Message);
    }
}
