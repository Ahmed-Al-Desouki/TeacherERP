using System.Data;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Finance;

public sealed class PaymobAdjustmentReviewService : IPaymobAdjustmentReviewService
{
    private readonly IFinancialLedgerRepository _repository;
    private readonly IFinancialLedgerService _ledger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditRepository _auditRepository;
    private readonly TimeProvider _timeProvider;

    public PaymobAdjustmentReviewService(IFinancialLedgerRepository repository,
        IFinancialLedgerService ledger, IUnitOfWork unitOfWork, IAuditRepository auditRepository,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _ledger = ledger;
        _unitOfWork = unitOfWork;
        _auditRepository = auditRepository;
        _timeProvider = timeProvider;
    }

    public Task<PagedResult<PaymobAdjustmentReviewItem>> GetPendingPageAsync(PageRequest page,
        CancellationToken cancellationToken) =>
        _repository.GetPendingPaymobAdjustmentReviewsPageAsync(page, cancellationToken);

    public Task<Result<PaymobAdjustmentResolution>> ResolveAsync(Guid administratorId, Guid reviewId,
        ResolvePaymobAdjustmentRequest request, CancellationToken cancellationToken)
    {
        if (administratorId == Guid.Empty || reviewId == Guid.Empty
            || request.StudentDebtAdjustmentAmount < 0 || request.CommissionReversalAmount < 0
            || decimal.Round(request.StudentDebtAdjustmentAmount, 2) != request.StudentDebtAdjustmentAmount
            || decimal.Round(request.CommissionReversalAmount, 2) != request.CommissionReversalAmount
            || string.IsNullOrWhiteSpace(request.DecisionNote) || request.DecisionNote.Trim().Length > 300)
            return Task.FromResult(Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.InvalidDecision));

        var reviewedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var review = await _repository.GetPaymobAdjustmentReviewTrackedAsync(reviewId, cancellationToken);
            if (review is null)
                return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.NotFound);
            if (review.Status != PaymobAdjustmentReviewStatus.Pending)
                return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.AlreadyResolved);
            var oldestPendingId = await _repository.GetOldestPendingPaymobAdjustmentReviewIdAsync(
                review.TransactionId, cancellationToken);
            if (oldestPendingId != reviewId)
                return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.ResolveInOrder);
            if (request.StudentDebtAdjustmentAmount > review.ProviderAmount)
                return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.AdjustmentExceedsRefund);

            if (!request.Approved && (request.StudentDebtAdjustmentAmount != 0
                                      || request.CommissionReversalAmount != 0))
                return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.RejectedAdjustmentHasAmounts);

            var hasMorePendingAdjustments = await _repository.HasOtherPendingPaymobAdjustmentReviewAsync(
                review.TransactionId, review.Id, cancellationToken);
            var payment = await _repository.GetTransactionTrackedAsync(review.TenantId,
                review.TransactionId, cancellationToken);
            if (payment is null || payment.Status != TransactionStatus.PaymobAdjustmentReview
                || (review.Type == PaymobAdjustmentType.Refund
                    && payment.ProviderRefundedAmount + review.ProviderAmount > payment.Amount))
                return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.TransactionNotReviewable);

            if (request.Approved)
            {
                var paymentCommission = await _repository.GetPlatformCommissionByTransactionTrackedAsync(
                    review.TransactionId, cancellationToken);
                if (request.CommissionReversalAmount > 0
                    && (paymentCommission is null || request.CommissionReversalAmount > paymentCommission.Amount))
                    return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.InvalidCommissionReversal);

                var debtResult = await _ledger.ApplyPaymobRefundDebtAsync(review.TenantId,
                    review.TransactionId, request.StudentDebtAdjustmentAmount, cancellationToken);
                if (debtResult.IsFailure)
                    return Result<PaymobAdjustmentResolution>.Failure(debtResult.Error);

                if (request.CommissionReversalAmount > 0
                    && !paymentCommission!.ApplyRefundReversal(request.CommissionReversalAmount))
                    return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.InvalidCommissionReversal);

            }

            // Provider state must reflect a verified refund/void even when the administrator
            // rejects its ledger adjustments. Only student debt and platform commission
            // adjustments are conditional on approval.
            if (!payment.ResolvePaymobAdjustment(review.Type, review.ProviderAmount,
                    hasMorePendingAdjustments))
                return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.TransactionNotReviewable);

            if (!review.Resolve(administratorId, request.Approved,
                    request.StudentDebtAdjustmentAmount, request.CommissionReversalAmount,
                    request.DecisionNote, reviewedAtUtc))
                return Result<PaymobAdjustmentResolution>.Failure(PaymobAdjustmentErrors.AlreadyResolved);

            _auditRepository.Add(AuditLog.Create(review.TenantId, administratorId,
                AuditAction.PaymobAdjustmentReviewed, payment.StudentId,
                $"Paymob {review.Type} adjustment {review.Id} {review.Status}; providerAmount={review.ProviderAmount:0.00}; studentDebtAdjustment={review.StudentDebtAdjustmentAmount:0.00}; commissionReversal={review.CommissionReversalAmount:0.00}; note={review.DecisionNote}"));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<PaymobAdjustmentResolution>.Success(new PaymobAdjustmentResolution(
                review.Id, review.Status, review.ProviderAmount, review.StudentDebtAdjustmentAmount,
                review.CommissionReversalAmount, reviewedAtUtc));
        }, IsolationLevel.Serializable, cancellationToken);
    }
}

public static class PaymobAdjustmentErrors
{
    public static readonly Error InvalidDecision = new("Finance.InvalidPaymobAdjustmentDecision",
        "A valid administrator, decision note, and non-negative amounts with at most two decimals are required.");
    public static readonly Error NotFound = new("Finance.PaymobAdjustmentNotFound",
        "The Paymob adjustment review was not found.");
    public static readonly Error AlreadyResolved = new("Finance.PaymobAdjustmentAlreadyResolved",
        "The Paymob adjustment review has already been resolved.");
    public static readonly Error AdjustmentExceedsRefund = new("Finance.PaymobAdjustmentExceedsRefund",
        "The student debt adjustment cannot exceed the provider-confirmed refund amount.");
    public static readonly Error RejectedAdjustmentHasAmounts = new("Finance.RejectedPaymobAdjustmentHasAmounts",
        "A rejected adjustment cannot change student debt or platform commission.");
    public static readonly Error InvalidCommissionReversal = new("Finance.InvalidPaymobCommissionReversal",
        "The commission reversal exceeds the platform commission recorded for this payment.");
    public static readonly Error TransactionNotReviewable = new("Finance.PaymobTransactionNotReviewable",
        "The Paymob transaction cannot be reconciled in its current state.");
    public static readonly Error ResolveInOrder = new("Finance.PaymobAdjustmentResolveInOrder",
        "Resolve older Paymob adjustments for this payment before resolving this one.");
}
