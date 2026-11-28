namespace TeacherERP.Domain.Entities;

/// <summary>حالة مصادقة مؤقتة محفوظة حتى انتهاء عمرها.</summary>
public sealed class TransientAuthState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string? OtpHash { get; set; }
    public string? GrantHash { get; set; }
    public string? PayloadJson { get; set; }
    public int FailedAttempts { get; set; }
    public bool IsLocked { get; set; }
    public int IssueCount { get; set; }
    public DateTime IssueWindowEndsAtUtc { get; set; }
    public DateTime CodeExpiresAtUtc { get; set; }
    public DateTime ResendAvailableAtUtc { get; set; }
    public DateTime GrantExpiresAtUtc { get; set; }
    public DateTime PayloadExpiresAtUtc { get; set; }
}
