using TeacherERP.Application.Enrollment;

namespace TeacherERP.Tests.Unit;

public sealed class GroupJoinCodeServiceTests
{
    private readonly GroupJoinCodeService _service = new();
    private static readonly DateTime WindowStart = DateTime.UnixEpoch.AddSeconds(30 * 1_000_000);
    private static readonly Guid GroupId = Guid.Parse("71e30fe6-90b6-4e68-9bd3-2ff003a1ca55");
    private const string Secret = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    [Fact]
    public void DynamicCode_IsValidForCurrentWindow()
    {
        var code = _service.CreateDynamicCode(GroupId, Secret, WindowStart.AddSeconds(4));

        Assert.True(_service.IsValidDynamicCode(GroupId, Secret, code, WindowStart.AddSeconds(29)));
        Assert.False(_service.IsValidDynamicCode(Guid.NewGuid(), Secret, code, WindowStart.AddSeconds(4)));
    }

    [Fact]
    public void DynamicCode_AcceptsPreviousWindowOnlyDuringGraceWindow()
    {
        var previousCode = _service.CreateDynamicCode(GroupId, Secret, WindowStart.AddSeconds(2));
        var nextWindowStart = WindowStart.AddSeconds(30);

        Assert.True(_service.IsValidDynamicCode(GroupId, Secret, previousCode, nextWindowStart.AddSeconds(10)));
        Assert.False(_service.IsValidDynamicCode(GroupId, Secret, previousCode, nextWindowStart.AddSeconds(30)));
    }

    [Fact]
    public void DynamicCode_RejectsTamperingAndDifferentSecret()
    {
        var code = _service.CreateDynamicCode(GroupId, Secret, WindowStart.AddSeconds(4));
        var parts = code.Split('.');
        parts[3] = (parts[3][0] == '0' ? '1' : '0') + parts[3][1..];

        Assert.False(_service.IsValidDynamicCode(GroupId, Secret, string.Join('.', parts), WindowStart.AddSeconds(4)));
        Assert.False(_service.IsValidDynamicCode(GroupId, new string('a', 64), code, WindowStart.AddSeconds(4)));
    }

    [Fact]
    public void JoinUrl_EncodesCodeAndStaticNormalizationIgnoresFormatting()
    {
        const string staticCode = "A1B2C3D4E5F607182930";
        var dynamicCode = _service.CreateDynamicCode(GroupId, Secret, WindowStart);

        Assert.Equal("A1B2C3D4E5F607182930", _service.NormalizeStaticCode(" a1b2-c3d4 e5f6-0718 2930 "));
        Assert.Equal("smartteachererp://join-group?code=" + Uri.EscapeDataString(dynamicCode), _service.CreateJoinUrl(dynamicCode));
        Assert.InRange(_service.SecondsRemaining(WindowStart.AddSeconds(29)), 1, 1);
    }
}
