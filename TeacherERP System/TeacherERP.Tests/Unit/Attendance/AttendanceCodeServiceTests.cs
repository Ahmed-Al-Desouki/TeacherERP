using System.Security.Cryptography;
using System.Text;
using TeacherERP.Application.Attendance;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Unit;

// F3: مفتاح الكود المتغير مشتق بـ HMAC من السر، والكود الثابت المطبوع (السر نفسه) يفضل شغال
public class AttendanceCodeServiceTests
{
    private const string Secret = "3f9c1a7be2d4480a9c6e5b1d7a2f0c84e1b3d5f7a9c2e4608b1d3f5a7c9e1b30";
    private const string OtherSecret = "aa11bb22cc33dd44ee55ff66aa11bb22cc33dd44ee55ff66aa11bb22cc33dd44";

    private static readonly Guid TenantId = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e");
    private static readonly Guid OtherTenantId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");

    // 10:00:00 UTC مضبوطة على بداية نافذة (30 ثانية تقسم الدقيقة)
    private static readonly DateTime WindowStart = new(2026, 11, 14, 10, 0, 0, DateTimeKind.Utc);

    private readonly AttendanceCodeService _service = new();

    // نفس الاشتقاق بشكل مستقل (للتأكد أن الخدمة تلتزم بالتصميم المتفق عليه)
    private static string ExpectedCode(Guid tenantId, string secret, DateTime atUtc)
    {
        var window = new DateTimeOffset(atUtc).ToUnixTimeSeconds() / AttendancePolicy.DynamicCodeWindowSeconds;
        var key = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(secret),
            Encoding.UTF8.GetBytes("attendance-dynamic-qr-v1"));
        var hash = HMACSHA256.HashData(key, Encoding.UTF8.GetBytes($"{tenantId:N}|{window}"));
        return Convert.ToHexString(hash)[..8];
    }

    [Fact]
    public void GenerateDynamicCode_MatchesHmacDerivation()
    {
        var code = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        Assert.Equal(ExpectedCode(TenantId, Secret, WindowStart), code);
        Assert.Equal(8, code.Length);
    }

    [Fact]
    public void GenerateDynamicCode_IsNotDerivedDirectlyFromTheStaticSecret()
    {
        var window = new DateTimeOffset(WindowStart).ToUnixTimeSeconds() / AttendancePolicy.DynamicCodeWindowSeconds;
        var naive = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(Secret),
            Encoding.UTF8.GetBytes($"{TenantId:N}|{window}")))[..8];

        var code = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        // من معه الكود الثابت المطبوع (السر) ما يقدرش يولّد الكود المتغير بالطريقة المباشرة
        Assert.NotEqual(naive, code);
        Assert.NotEqual(Secret[..8].ToUpperInvariant(), code.ToUpperInvariant());
    }

    [Fact]
    public void GenerateDynamicCode_IsStableInsideOneWindowAndChangesInTheNext()
    {
        var first = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);
        var sameWindow = _service.GenerateDynamicCode(TenantId, Secret, WindowStart.AddSeconds(29));
        var nextWindow = _service.GenerateDynamicCode(TenantId, Secret, WindowStart.AddSeconds(30));

        Assert.Equal(first, sameWindow);
        Assert.NotEqual(first, nextWindow);
    }

    [Fact]
    public void GenerateDynamicCode_DiffersPerTenantAndPerSecret()
    {
        var baseline = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        Assert.NotEqual(baseline, _service.GenerateDynamicCode(OtherTenantId, Secret, WindowStart));
        Assert.NotEqual(baseline, _service.GenerateDynamicCode(TenantId, OtherSecret, WindowStart));
    }

    [Fact]
    public void SecondsRemaining_CountsDownInsideTheWindow()
    {
        Assert.Equal(30, _service.SecondsRemaining(WindowStart));
        Assert.Equal(20, _service.SecondsRemaining(WindowStart.AddSeconds(10)));
        Assert.Equal(1, _service.SecondsRemaining(WindowStart.AddSeconds(29)));
    }

    [Fact]
    public void IsValid_Dynamic_AcceptsCurrentWindow()
    {
        var code = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        Assert.True(_service.IsValid(AttendanceMode.DynamicScreenQR, TenantId, Secret, code, WindowStart.AddSeconds(5)));
    }

    [Fact]
    public void IsValid_Dynamic_AcceptsPreviousWindow()
    {
        var code = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        Assert.True(_service.IsValid(AttendanceMode.DynamicScreenQR, TenantId, Secret, code, WindowStart.AddSeconds(35)));
    }

    [Fact]
    public void IsValid_Dynamic_RejectsCodeOlderThanPreviousWindow()
    {
        var code = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        Assert.False(_service.IsValid(AttendanceMode.DynamicScreenQR, TenantId, Secret, code, WindowStart.AddSeconds(65)));
    }

    [Fact]
    public void IsValid_Dynamic_RejectsCodeFromTheFutureWindow()
    {
        var future = _service.GenerateDynamicCode(TenantId, Secret, WindowStart.AddSeconds(30));

        Assert.False(_service.IsValid(AttendanceMode.DynamicScreenQR, TenantId, Secret, future, WindowStart));
    }

    [Fact]
    public void IsValid_Dynamic_IgnoresCaseAndSurroundingSpaces()
    {
        var code = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        Assert.True(_service.IsValid(
            AttendanceMode.DynamicScreenQR, TenantId, Secret, $"  {code.ToLowerInvariant()} ", WindowStart));
    }

    [Fact]
    public void IsValid_Dynamic_RejectsCodeOfAnotherTenant()
    {
        var foreign = _service.GenerateDynamicCode(OtherTenantId, Secret, WindowStart);

        Assert.False(_service.IsValid(AttendanceMode.DynamicScreenQR, TenantId, Secret, foreign, WindowStart));
    }

    [Fact]
    public void IsValid_Dynamic_RejectsTheStaticSecretItself()
    {
        Assert.False(_service.IsValid(AttendanceMode.DynamicScreenQR, TenantId, Secret, Secret, WindowStart));
        Assert.False(_service.IsValid(AttendanceMode.DynamicScreenQR, TenantId, Secret, Secret[..8], WindowStart));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_RejectsEmptyCodeInEveryMode(string code)
    {
        Assert.False(_service.IsValid(AttendanceMode.StaticGlobalQR, TenantId, Secret, code, WindowStart));
        Assert.False(_service.IsValid(AttendanceMode.DynamicScreenQR, TenantId, Secret, code, WindowStart));
    }

    [Fact]
    public void IsValid_Static_AcceptsThePrintedSecret()
    {
        Assert.True(_service.IsValid(AttendanceMode.StaticGlobalQR, TenantId, Secret, Secret, WindowStart));
    }

    [Fact]
    public void IsValid_Static_DoesNotDependOnTime()
    {
        // الكارت المطبوع قبل التعديل يفضل شغال في أي وقت
        Assert.True(_service.IsValid(AttendanceMode.StaticGlobalQR, TenantId, Secret, Secret, WindowStart.AddYears(1)));
        Assert.True(_service.IsValid(AttendanceMode.StaticGlobalQR, TenantId, Secret, Secret, WindowStart.AddYears(-1)));
    }

    [Fact]
    public void IsValid_Static_IgnoresCaseOfThePresentedCode()
    {
        Assert.True(_service.IsValid(AttendanceMode.StaticGlobalQR, TenantId, Secret, Secret.ToUpperInvariant(), WindowStart));
    }

    [Fact]
    public void IsValid_Static_RejectsWrongSecretAndDynamicCode()
    {
        var dynamicCode = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        Assert.False(_service.IsValid(AttendanceMode.StaticGlobalQR, TenantId, Secret, OtherSecret, WindowStart));
        Assert.False(_service.IsValid(AttendanceMode.StaticGlobalQR, TenantId, Secret, dynamicCode, WindowStart));
    }

    [Fact]
    public void IsValid_AdminOnly_IsAlwaysFalse()
    {
        var dynamicCode = _service.GenerateDynamicCode(TenantId, Secret, WindowStart);

        Assert.False(_service.IsValid(AttendanceMode.AdminOnly, TenantId, Secret, Secret, WindowStart));
        Assert.False(_service.IsValid(AttendanceMode.AdminOnly, TenantId, Secret, dynamicCode, WindowStart));
    }
}
