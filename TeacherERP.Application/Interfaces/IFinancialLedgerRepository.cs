using TeacherERP.Domain.Entities;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Interfaces
{
    public sealed record FinancialSessionInfo(Guid TenantId, Guid StudentId, Guid GroupId, Guid SessionId,
        string GroupName, string TenantTimeZoneId, DateTime SessionStartUtc, decimal? PerSessionPrice, decimal? MonthlyPrice);
    public sealed record FinancialTransactionInfo(Guid Id, decimal Amount, PaymentType Type, TransactionStatus Status,
        DateTime CreatedAt, Guid? RelatedSessionId);
    public sealed record PendingExternalPaymentItem(Guid Id, Guid StudentId, string StudentName, decimal Amount,
        PaymentType Type, DateTime CreatedAt, Guid SessionId, string ReceiptImageUrl);

    public interface IFinancialLedgerRepository
    {
        Task<FinancialSessionInfo?> GetSessionInfoAsync(Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken);
        Task<bool> HasPresentAttendanceAsync(Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken);
        Task<List<FinancialObligation>> GetOutstandingObligationsAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken);
        Task<FinancialObligation?> GetObligationAsync(Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken);
        Task<bool> HasMonthlySubscriptionAsync(Guid tenantId, Guid studentId, Guid groupId, int year, int month, CancellationToken cancellationToken);
        Task<List<FinancialTransactionInfo>> GetRecentPaymentsAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken);
        void AddObligation(FinancialObligation obligation);
        void AddSubscription(MonthlySubscription subscription);
        void AddPayment(Transaction transaction);

        Task<TeacherPaymentSettings?> GetTeacherPaymentSettingsAsync(Guid tenantId, CancellationToken cancellationToken);
        void AddTeacherPaymentSettings(TeacherPaymentSettings settings);
        Task<bool> HasPendingExternalPaymentAsync(Guid tenantId, Guid studentId, PaymentType type, Guid contextSessionId, CancellationToken cancellationToken);
        Task<Transaction?> GetTransactionTrackedAsync(Guid tenantId, Guid transactionId, CancellationToken cancellationToken);
        Task<PagedResult<PendingExternalPaymentItem>> GetPendingExternalPaymentsPageAsync(
            Guid tenantId, PageRequest page, CancellationToken cancellationToken);

        Task<Transaction?> GetPaymobPaymentByIdempotencyAsync(Guid tenantId, Guid studentId,
            string idempotencyKey, CancellationToken cancellationToken);
        Task<Transaction?> GetPaymobPaymentForCallbackAsync(string? specialReference,
            string? providerOrderId, CancellationToken cancellationToken);
        Task<bool> HasPaymentEventAsync(PaymentProvider provider, string providerEventId,
            CancellationToken cancellationToken);
        Task<bool> HasOpenExternalPaymentAsync(Guid tenantId, Guid studentId, PaymentType type,
            Guid contextSessionId, CancellationToken cancellationToken);
        void AddPaymentEvent(PaymentEvent paymentEvent);
    }
}
