using TeacherERP.Application.Features.Enrollment.MoveStudent;

namespace TeacherERP.Tests.Unit;

public sealed class MoveStudentTests
{
    [Fact]
    public void Validator_RequiresBothStudentAndTargetGroupIds()
    {
        var validator = new MoveStudentValidator();

        Assert.False(validator.Validate(new MoveStudentCommand(Guid.Empty, Guid.NewGuid())).IsValid);
        Assert.False(validator.Validate(new MoveStudentCommand(Guid.NewGuid(), Guid.Empty)).IsValid);
        Assert.True(validator.Validate(new MoveStudentCommand(Guid.NewGuid(), Guid.NewGuid())).IsValid);
    }
}
