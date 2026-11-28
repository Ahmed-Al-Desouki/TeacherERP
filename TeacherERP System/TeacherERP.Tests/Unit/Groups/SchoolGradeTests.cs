using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Features.Groups.CreateGroup;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Unit;

public sealed class SchoolGradeTests
{
    [Theory]
    [InlineData(EducationStage.Primary, 1, true)]
    [InlineData(EducationStage.Primary, 6, true)]
    [InlineData(EducationStage.Preparatory, 3, true)]
    [InlineData(EducationStage.Secondary, 1, true)]
    [InlineData(EducationStage.Primary, 7, false)]
    [InlineData(EducationStage.Preparatory, 0, false)]
    [InlineData(EducationStage.Secondary, 4, false)]
    [InlineData(EducationStage.Unspecified, 1, false)]
    public void GradeValidity_UsesStageSpecificRange(EducationStage stage, int number, bool expected)
        => Assert.Equal(expected, new SchoolGrade(stage, number).IsValid());

    [Fact]
    public void CreateGroupValidator_RequiresAValidStructuredGrade()
    {
        var validator = new CreateGroupValidator();
        var command = new CreateGroupCommand(
            "Group", new SchoolGrade(EducationStage.Preparatory, 5), Guid.NewGuid(), null, null, null,
            new List<GroupSlotInput> { new(DayOfWeek.Monday, TimeSpan.FromHours(15), 60) });

        Assert.Contains(validator.Validate(command).Errors, error => error.PropertyName == "Grade");
    }
}
