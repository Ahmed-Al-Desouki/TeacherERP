namespace TeacherERP.Domain.Entities;

/// <summary>
/// A verified Paymob refund or void that must be reconciled by an administrator
/// before it affects the student or platform financial ledgers.
/// </summary>
public sealed class PaymobAdjustmentReview : BaseEntity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid TransactionId { get; private set; }
    public string ProviderEventId { get; private set; } = string.Empty;
    public PaymobAdjustmentType Type { get; private set; }
    public decimal ProviderAmount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public PaymobAdjustmentReviewStatus Status { get; private set; }
    public decimal StudentDebtAdjustmentAmount { get; private set; }
    public decimal CommissionReversalAmount { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public string? DecisionNote { get; private set; }

    private PaymobAdjustmentReview() { }

    public static PaymobAdjustmentReview Create(Guid tenantId, Guid transactionId,
        string providerEventId, PaymobAdjustmentType type, decimal providerAmount, string currency)
    {
        if (tenantId == Guid.Empty || transactionId == Guid.Empty)
            throw new ArgumentException("Tenant and transaction are required.");
        if (string.IsNullOrWhiteSpace(providerEventId) || providerEventId.Length > 128)
            throw new ArgumentException("A provider event ID of at most 128 characters is required.", nameof(providerEventId));
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (providerAmount <= 0 || decimal.Round(providerAmount, 2) != providerAmount)
            throw new ArgumentOutOfRangeException(nameof(providerAmount));
        if (string.IsNullOrWhiteSpace(currency) || currency.Length > 8)
            throw new ArgumentException("A currency code of at most 8 characters is required.", nameof(currency));

        return new PaymobAdjustmentReview
        {
            TenantId = tenantId,
            TransactionId = transactionId,
            ProviderEventId = providerEventId,
            Type = type,
            ProviderAmount = providerAmount,
            Currency = currency.Trim().ToUpperInvariant(),
            Status = PaymobAdjustmentReviewStatus.Pending
        };
    }

    public bool Resolve(Guid administratorId, bool approved, decimal studentDebtAdjustmentAmount,
        decimal commissionReversalAmount, string decisionNote, DateTime reviewedAtUtc)
    {
        if (Status != PaymobAdjustmentReviewStatus.Pending) return false;
        if (administratorId == Guid.Empty) throw new ArgumentException("An administrator is required.", nameof(administratorId));
        if (studentDebtAdjustmentAmount < 0 || commissionReversalAmount < 0
            || decimal.Round(studentDebtAdjustmentAmount, 2) != studentDebtAdjustmentAmount
            || decimal.Round(commissionReversalAmount, 2) != commissionReversalAmount)
            throw new ArgumentOutOfRangeException(nameof(studentDebtAdjustmentAmount));
        if (!approved && (studentDebtAdjustmentAmount != 0 || commissionReversalAmount != 0))
            throw new ArgumentException("Rejected adjustments cannot change financial balances.");
        if (string.IsNullOrWhiteSpace(decisionNote) || decisionNote.Trim().Length > 300)
            throw new ArgumentException("A decision note of at most 300 characters is required.", nameof(decisionNote));

        Status = approved ? PaymobAdjustmentReviewStatus.Approved : PaymobAdjustmentReviewStatus.Rejected;
        StudentDebtAdjustmentAmount = studentDebtAdjustmentAmount;
        CommissionReversalAmount = commissionReversalAmount;
        ReviewedByUserId = administratorId;
        ReviewedAtUtc = DateTime.SpecifyKind(reviewedAtUtc, DateTimeKind.Utc);
        DecisionNote = decisionNote.Trim();
        MarkAsUpdated();
        return true;
    }
}

public enum PaymobAdjustmentType { Refund, Void }
public enum PaymobAdjustmentReviewStatus { Pending, Approved, Rejected }
