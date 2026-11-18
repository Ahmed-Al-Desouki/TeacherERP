using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Tests.Unit;

public sealed class GroupPricingTests
{
    [Fact]
    public void Create_RejectsNegativePrices()
    {
        var grade = new SchoolGrade(EducationStage.Primary, 1);

        Assert.Throws<ArgumentOutOfRangeException>(() => Group.Create(
            Guid.NewGuid(), "Group", grade, Guid.NewGuid(), null, null, null, -1m, null));
        Assert.Throws<ArgumentOutOfRangeException>(() => Group.Create(
            Guid.NewGuid(), "Group", grade, Guid.NewGuid(), null, null, null, null, -1m));
    }

    [Fact]
    public void UpdatePricing_AllowsUnsetAndFreePrices()
    {
        var group = Group.Create(
            Guid.NewGuid(), "Group", new SchoolGrade(EducationStage.Primary, 1),
            Guid.NewGuid(), null, null, null);

        Assert.Throws<ArgumentOutOfRangeException>(() => group.UpdatePricing(-1m, null));

        group.UpdatePricing(null, 0m);

        Assert.Null(group.PerSessionPrice);
        Assert.Equal(0m, group.MonthlyPrice);
    }
}
