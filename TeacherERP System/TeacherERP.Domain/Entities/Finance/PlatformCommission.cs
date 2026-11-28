namespace TeacherERP.Domain.Entities;

/// <summary>Commission owed to the platform for a successfully paid session or monthly subscription.</summary>
public sealed class PlatformCommission : BaseEntity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid SourceTransactionId { get; private set; }
    public Guid? SessionId { get; private set; }
    public PlatformCommissionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public int? SubscriptionYear { get; private set; }
    public int? SubscriptionMonth { get; private set; }
    public DateTime? CollectionRequestedAtUtc { get; private set; }
    public Guid? CollectionRequestedByUserId { get; private set; }
    public DateTime? SettledAtUtc { get; private set; }
    public Guid? SettledByUserId { get; private set; }
    public string? SettlementReference { get; private set; }
    public bool IsSettled => SettledAtUtc.HasValue;

    private PlatformCommission() { }

    public static PlatformCommission ForSession(Guid tenantId, Guid studentId, Guid groupId,
        Guid transactionId, Guid sessionId, decimal amount) => new()
    {
        TenantId = Require(tenantId, nameof(tenantId)),
        StudentId = Require(studentId, nameof(studentId)),
        GroupId = Require(groupId, nameof(groupId)),
        SourceTransactionId = Require(transactionId, nameof(transactionId)),
        SessionId = Require(sessionId, nameof(sessionId)),
        Type = PlatformCommissionType.PerSession,
        Amount = ValidateAmount(amount)
    };

    public static PlatformCommission ForMonthlySubscription(Guid tenantId, Guid studentId, Guid groupId,
        Guid transactionId, int year, int month, decimal amount) => new()
    {
        TenantId = Require(tenantId, nameof(tenantId)),
        StudentId = Require(studentId, nameof(studentId)),
        GroupId = Require(groupId, nameof(groupId)),
        SourceTransactionId = Require(transactionId, nameof(transactionId)),
        Type = PlatformCommissionType.MonthlySubscription,
        Amount = ValidateAmount(amount),
        SubscriptionYear = year is >= 2000 and <= 9999 ? year : throw new ArgumentOutOfRangeException(nameof(year)),
        SubscriptionMonth = month is >= 1 and <= 12 ? month : throw new ArgumentOutOfRangeException(nameof(month))
    };

    public bool Settle(Guid administratorId, DateTime settledAtUtc, string reference)
    {
        if (IsSettled) return false;
        if (administratorId == Guid.Empty) throw new ArgumentException("An administrator is required.", nameof(administratorId));
        if (string.IsNullOrWhiteSpace(reference) || reference.Trim().Length > 128)
            throw new ArgumentException("A settlement reference of at most 128 characters is required.", nameof(reference));
        SettledByUserId = administratorId;
        SettledAtUtc = DateTime.SpecifyKind(settledAtUtc, DateTimeKind.Utc);
        SettlementReference = reference.Trim();
        MarkAsUpdated();
        return true;
    }

    public bool ApplyRefundReversal(decimal amount)
    {
        if (amount <= 0 || decimal.Round(amount, 2) != amount || amount > Amount)
            return false;
        Amount -= amount;
        MarkAsUpdated();
        return true;
    }

    public bool RequestCollection(Guid administratorId, DateTime requestedAtUtc)
    {
        if (IsSettled || CollectionRequestedAtUtc.HasValue) return false;
        if (administratorId == Guid.Empty)
            throw new ArgumentException("An administrator is required.", nameof(administratorId));
        CollectionRequestedByUserId = administratorId;
        CollectionRequestedAtUtc = DateTime.SpecifyKind(requestedAtUtc, DateTimeKind.Utc);
        MarkAsUpdated();
        return true;
    }

    private static Guid Require(Guid value, string name) =>
        value == Guid.Empty ? throw new ArgumentException("A non-empty identifier is required.", name) : value;

    private static decimal ValidateAmount(decimal amount) =>
        amount <= 0 || decimal.Round(amount, 2) != amount
            ? throw new ArgumentOutOfRangeException(nameof(amount)) : amount;
}

public enum PlatformCommissionType { PerSession, MonthlySubscription }
