using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Unit.Common;

public sealed class BaseEntityTimeProviderTests
{
    [Fact]
    public void BaseEntity_UsesInjectedTimeProviderForCreationAndUpdates()
    {
        var now = new DateTime(2030, 4, 5, 6, 7, 8, DateTimeKind.Utc);
        var timeProvider = new FixedTimeProvider(now);
        var entity = new TestEntity(timeProvider);

        Assert.Equal(now, entity.CreatedAt);

        entity.MarkAsUpdated();

        Assert.Equal(now, entity.UpdatedAt);
    }

    private sealed class TestEntity(TimeProvider timeProvider) : BaseEntity(timeProvider)
    {
    }
}
