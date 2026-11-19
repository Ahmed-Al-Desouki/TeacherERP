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
    public sealed record PlatformCommissionItem(Guid Id, Guid TenantId, string TenantName, Guid TeacherId,
        string TeacherName, Guid StudentId, string StudentName, Guid SourceTransactionId, Guid? SessionId,
        PlatformCommissionType Type, decimal Amount, DateTime CreatedAt, DateTime? SettledAtUtc,
        string? SettlementReference, DateTime? CollectionRequestedAtUtc);
    public sealed record PlatformCommissionSummary(Guid TenantId, string TenantName, Guid TeacherId,
        string TeacherName, int EntryCount, decimal TotalAmount, int SettledCount,
        decimal SettledAmount, int OutstandingCount, decimal OutstandingAmount);
    public sealed record MonthlyPaymentMethodSummary(string Method, PaymentType Type,
        int SuccessfulCount, decimal SuccessfulAmount, int PendingCount, decimal PendingAmount,
        int ReviewCount, decimal ReviewAmount, int FailedCount, decimal FailedAmount);
    public sealed record MonthlyCollectionTotals(int SuccessfulCount, decimal SuccessfulAmount,
        int PendingCount, decimal PendingAmount, int ReviewCount, decimal ReviewAmount,
        int FailedCount, decimal FailedAmount, int OutstandingDebtCount, decimal OutstandingDebtAmount);
    public sealed record MonthlyCollectionItem(Guid TransactionId, Guid TenantId, string TenantName,
        Guid TeacherId, string TeacherName, Guid StudentId, string StudentName, PaymentType Type,
        string Method, TransactionStatus Status, decimal Amount, DateTime CreatedAt,
        DateTime? UpdatedAt, DateTime OccurredAtUtc, Guid? RelatedSessionId);
    public sealed record MonthlyOutstandingDebtItem(Guid TenantId, string TenantName, Guid TeacherId,
        string TeacherName, Guid StudentId, string StudentName, Guid SessionId, string GroupName,
        DateTime SessionStartUtc, decimal Amount, decimal PaidAmount, decimal OutstandingAmount);
    public sealed record MonthlyCollectionReport(int Year, int Month, MonthlyCollectionTotals Totals,
        IReadOnlyList<MonthlyPaymentMethodSummary> ByMethodAndType,
        PagedResult<MonthlyCollectionItem> Transactions,
        PagedResult<MonthlyOutstandingDebtItem> OutstandingDebts);

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
        Task<bool> HasSessionCommissionAsync(Guid tenantId, Guid studentId, Guid sessionId,
            CancellationToken cancellationToken);
        void AddPlatformCommission(PlatformCommission commission);
        Task<PagedResult<PlatformCommissionItem>> GetPlatformCommissionsPageAsync(int year, int month,
            bool unsettledOnly, PageRequest page, CancellationToken cancellationToken, Guid? tenantId = null);
        Task<List<PlatformCommissionSummary>> GetPlatformCommissionSummariesAsync(int year, int month,
            CancellationToken cancellationToken, Guid? tenantId = null);
        Task<List<PlatformCommission>> GetUnsettledPlatformCommissionsAsync(Guid tenantId,
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);
        Task<PlatformCommissionSettings?> GetPlatformCommissionSettingsAsync(CancellationToken cancellationToken);
        void AddPlatformCommissionSettings(PlatformCommissionSettings settings);
        Task<MonthlyCollectionReport> GetMonthlyCollectionReportAsync(int year, int month,
            PageRequest transactionPage, PageRequest debtPage, CancellationToken cancellationToken,
            Guid? tenantId = null);
    }
}
