using TeacherERP.Domain.Shared;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public sealed record FinancialDebtItem(Guid SessionId, Guid GroupId, string GroupName, DateTime SessionStartUtc,
        decimal OriginalAmount, decimal PaidAmount, decimal OutstandingAmount);
    public sealed record FinancialTransactionItem(Guid Id, decimal Amount, PaymentType Type,
        TransactionStatus Status, DateTime CreatedAt, Guid? RelatedSessionId);

    public sealed record FinancialPaymentOptions(Guid StudentId, Guid GroupId, Guid SessionId, string GroupName,
        decimal? PerSessionPrice, decimal? MonthlyPrice, IReadOnlyList<FinancialDebtItem> Debts,
        decimal TotalOutstanding, decimal AccountBalance, decimal WalletCredit, IReadOnlyList<FinancialTransactionItem> RecentPayments,
        bool HasMonthlySubscription, int SubscriptionYear, int SubscriptionMonth);

    public sealed record CashPaymentRequest(Guid StudentId, Guid SessionId, PaymentType PaymentType,
        decimal Amount, PaymentProvider Method = PaymentProvider.Cash);
    public sealed record CashPaymentResult(Guid TransactionId, PaymentType PaymentType, decimal Amount,
        decimal RemainingDebt, bool MonthlyCoverageCreated);
    public sealed record TeacherPaymentSettingsDto(string InstaPayLink, string QrImageUrl);
    public sealed record StudentPaymentDetails(FinancialPaymentOptions FinancialOptions, TeacherPaymentSettingsDto? InstaPay);

    public interface IFinancialLedgerService
    {
        Task<Result<FinancialPaymentOptions>> GetOptionsAsync(Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken);
        Task RecordPresentAttendanceAsync(Guid tenantId, Guid studentId, Guid sessionId, Guid? compensatesSessionId,
            CancellationToken cancellationToken);
        Task<Result<CashPaymentResult>> CollectCashAsync(Guid tenantId, Guid processorId, CashPaymentRequest request,
            CancellationToken cancellationToken);
        Task<Result<CashPaymentResult>> ApproveExternalPaymentAsync(Guid tenantId, Guid reviewerId, Guid transactionId,
            CancellationToken cancellationToken);
        Task<Result<bool>> RejectExternalPaymentAsync(Guid tenantId, Guid reviewerId, Guid transactionId,
            CancellationToken cancellationToken);
        Task<Result<CashPaymentResult>> ApplyPaymobPaymentAsync(Guid tenantId, Guid transactionId,
            CancellationToken cancellationToken);
    }
}
