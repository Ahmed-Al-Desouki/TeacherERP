using System.Data;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Finance;

public sealed class PlatformCommissionService : IPlatformCommissionService
{
    private readonly IFinancialLedgerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditRepository _auditRepository;
    private readonly TimeProvider _timeProvider;

    public PlatformCommissionService(IFinancialLedgerRepository repository, IUnitOfWork unitOfWork,
        IAuditRepository auditRepository, TimeProvider timeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _auditRepository = auditRepository;
        _timeProvider = timeProvider;
    }

    public Task<PagedResult<PlatformCommissionItem>> GetPageAsync(int year, int month,
        bool unsettledOnly, PageRequest page, CancellationToken cancellationToken)
    {
        if (year is < 2000 or > 9999 || month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "A valid year and month are required.");
        return _repository.GetPlatformCommissionsPageAsync(year, month, unsettledOnly, page, cancellationToken);
    }

    public Task<List<PlatformCommissionSummary>> GetSummariesAsync(int year, int month,
        CancellationToken cancellationToken)
    {
        if (year is < 2000 or > 9999 || month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "A valid year and month are required.");
        return _repository.GetPlatformCommissionSummariesAsync(year, month, cancellationToken);
    }

    public Task<Result<PlatformCommissionSettlementResult>> SettleMonthAsync(Guid administratorId,
        Guid tenantId, int year, int month, string settlementReference, CancellationToken cancellationToken)
    {
        settlementReference = settlementReference?.Trim() ?? string.Empty;
        if (administratorId == Guid.Empty || tenantId == Guid.Empty
            || year is < 2000 or > 9999 || month is < 1 or > 12
            || settlementReference.Length is < 1 or > 128)
            return Task.FromResult(Result<PlatformCommissionSettlementResult>.Failure(CommissionErrors.InvalidSettlement));

        var fromUtc = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var toUtc = fromUtc.AddMonths(1);
        var settledAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var due = await _repository.GetUnsettledPlatformCommissionsAsync(
                tenantId, fromUtc, toUtc, cancellationToken);
            if (due.Count == 0)
                return Result<PlatformCommissionSettlementResult>.Failure(CommissionErrors.NothingDue);

            var total = 0m;
            foreach (var commission in due)
            {
                if (commission.Settle(administratorId, settledAtUtc, settlementReference))
                    total += commission.Amount;
            }

            _auditRepository.Add(AuditLog.Create(tenantId, administratorId,
                AuditAction.PlatformCommissionSettled, null,
                $"Platform commission settlement: {year:D4}-{month:D2}; count={due.Count}; amount={total:0.00}; reference={settlementReference}."));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<PlatformCommissionSettlementResult>.Success(new PlatformCommissionSettlementResult(
                tenantId, year, month, due.Count, total, settledAtUtc, settlementReference));
        }, IsolationLevel.Serializable, cancellationToken);
    }
}

public static class CommissionErrors
{
    public static readonly Error InvalidSettlement = new("Finance.InvalidCommissionSettlement",
        "The tenant, month, administrator, or settlement reference is invalid.");
    public static readonly Error NothingDue = new("Finance.NoCommissionDue",
        "There are no unpaid platform commissions for this teacher and month.");
}
