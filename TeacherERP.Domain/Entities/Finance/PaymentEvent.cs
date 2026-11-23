namespace TeacherERP.Domain.Entities;

/// <summary>
/// Append-only record of payment-provider events. Never update or delete these records.
/// </summary>
public sealed class PaymentEvent : BaseEntity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid TransactionId { get; private set; }
    public PaymentProvider Provider { get; private set; }
    public string ProviderEventId { get; private set; } = string.Empty;
    public string EventType { get; private set; } = string.Empty;
    public TransactionStatus? PreviousStatus { get; private set; }
    public TransactionStatus? CurrentStatus { get; private set; }
    public string? ProviderOrderId { get; private set; }
    public DateTime? ProviderOccurredAtUtc { get; private set; }
    public string? PayloadSha256 { get; private set; }

    private PaymentEvent() { }

    public static PaymentEvent Create(Guid tenantId, Guid transactionId, PaymentProvider provider,
        string providerEventId, string eventType, TransactionStatus? previousStatus,
        TransactionStatus? currentStatus, string? providerOrderId = null,
        DateTime? providerOccurredAtUtc = null, string? payloadSha256 = null)
    {
        if (tenantId == Guid.Empty || transactionId == Guid.Empty)
            throw new ArgumentException("Payment event ownership and transaction are required.");
        if (string.IsNullOrWhiteSpace(providerEventId) || providerEventId.Length > 128)
            throw new ArgumentException("A provider event ID of at most 128 characters is required.", nameof(providerEventId));
        if (string.IsNullOrWhiteSpace(eventType) || eventType.Length > 64)
            throw new ArgumentException("An event type of at most 64 characters is required.", nameof(eventType));

        return new PaymentEvent
        {
            TenantId = tenantId,
            TransactionId = transactionId,
            Provider = provider,
            ProviderEventId = providerEventId,
            EventType = eventType,
            PreviousStatus = previousStatus,
            CurrentStatus = currentStatus,
            ProviderOrderId = providerOrderId,
            ProviderOccurredAtUtc = providerOccurredAtUtc,
            PayloadSha256 = payloadSha256
        };
    }
}
