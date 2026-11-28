using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces;

public sealed record ResolvePaymobAdjustmentRequest(bool Approved,
    decimal StudentDebtAdjustmentAmount, decimal CommissionReversalAmount, string DecisionNote);

public sealed record PaymobAdjustmentResolution(Guid Id, PaymobAdjustmentReviewStatus Status,
    decimal ProviderAmount, decimal StudentDebtAdjustmentAmount, decimal CommissionReversalAmount,
    DateTime ReviewedAtUtc);

public interface IPaymobAdjustmentReviewService
{
    Task<PagedResult<PaymobAdjustmentReviewItem>> GetPendingPageAsync(PageRequest page,
        CancellationToken cancellationToken);
    Task<Result<PaymobAdjustmentResolution>> ResolveAsync(Guid administratorId, Guid reviewId,
        ResolvePaymobAdjustmentRequest request, CancellationToken cancellationToken);
}
