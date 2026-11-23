using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TeacherERP.Application.Attendance;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Enrollment;

public sealed class GroupJoinCodeService : IGroupJoinCodeService
{
    private const string DynamicPrefix = "D";
    private const string DynamicKeyLabel = "teachererp-group-join-v1";
    private const string JoinLinkPrefix = "smartteachererp://join-group?code=";
    private const int SignatureBytes = 12;

    public string CreateDynamicCode(Guid groupId, string secret, DateTime atUtc)
    {
        var window = WindowOf(atUtc);
        var signature = CreateSignature(groupId, secret, window);
        return $"{DynamicPrefix}.{groupId:N}.{window.ToString(CultureInfo.InvariantCulture)}.{signature}";
    }

    public bool TryGetDynamicGroupId(string code, out Guid groupId)
    {
        groupId = Guid.Empty;
        var parts = code.Split('.', StringSplitOptions.None);
        return parts.Length == 4
               && string.Equals(parts[0], DynamicPrefix, StringComparison.Ordinal)
               && Guid.TryParseExact(parts[1], "N", out groupId)
               && long.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out _)
               && parts[3].Length == SignatureBytes * 2
               && parts[3].All(Uri.IsHexDigit);
    }

    public bool IsValidDynamicCode(Guid groupId, string secret, string code, DateTime nowUtc)
    {
        if (!TryGetDynamicGroupId(code, out var codeGroupId) || codeGroupId != groupId)
        {
            return false;
        }

        var parts = code.Split('.', StringSplitOptions.None);
        if (!long.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var presentedWindow))
        {
            return false;
        }

        var currentWindow = WindowOf(nowUtc);
        var isCurrent = VerifyWindow(groupId, secret, presentedWindow, currentWindow, parts[3]);
        var isPrevious = VerifyWindow(groupId, secret, presentedWindow, currentWindow - 1, parts[3]);
        return isCurrent | isPrevious;
    }

    public int SecondsRemaining(DateTime atUtc)
    {
        var seconds = ToUnixSeconds(atUtc);
        return AttendancePolicy.DynamicCodeWindowSeconds - (int)(seconds % AttendancePolicy.DynamicCodeWindowSeconds);
    }

    public string CreateJoinUrl(string code)
        => JoinLinkPrefix + Uri.EscapeDataString(code);

    public string NormalizeStaticCode(string code)
        => code.Trim().Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

    private static bool VerifyWindow(Guid groupId, string secret, long presentedWindow, long expectedWindow, string signature)
    {
        var sameWindow = presentedWindow == expectedWindow;
        var expectedSignature = CreateSignature(groupId, secret, expectedWindow);
        var sameSignature = CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(expectedSignature),
            Encoding.ASCII.GetBytes(signature.ToUpperInvariant()));
        return sameWindow & sameSignature;
    }

    private static string CreateSignature(Guid groupId, string secret, long window)
    {
        var key = Convert.FromHexString(secret);
        var derivedKey = HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(DynamicKeyLabel));
        var payload = Encoding.UTF8.GetBytes($"{groupId:N}|{window.ToString(CultureInfo.InvariantCulture)}");
        return Convert.ToHexString(HMACSHA256.HashData(derivedKey, payload)[..SignatureBytes]);
    }

    private static long WindowOf(DateTime atUtc)
        => ToUnixSeconds(atUtc) / AttendancePolicy.DynamicCodeWindowSeconds;

    private static long ToUnixSeconds(DateTime atUtc)
        => new DateTimeOffset(DateTime.SpecifyKind(atUtc, DateTimeKind.Utc)).ToUnixTimeSeconds();
}
