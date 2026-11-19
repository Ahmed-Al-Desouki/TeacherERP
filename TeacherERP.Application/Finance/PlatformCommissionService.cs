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
    private readonly INotificationService _notifications;
    private readonly TimeProvider _timeProvider;

    public PlatformCommissionService(IFinancialLedgerRepository repository, IUnitOfWork unitOfWork,
        IAuditRepository auditRepository, INotificationService notifications, TimeProvider timeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _auditRepository = auditRepository;
        _notifications = notifications;
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
        CancellationToken cancellationToken, Guid? tenantId = null)
    {
        if (year is < 2000 or > 9999 || month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "A valid year and month are required.");
        return _repository.GetPlatformCommissionSummariesAsync(year, month, cancellationToken, tenantId);
    }

    public Task<PagedResult<PlatformCommissionItem>> GetTeacherPageAsync(Guid tenantId, int year, int month,
        bool unsettledOnly, PageRequest page, CancellationToken cancellationToken)
    {
        if (tenantId == Guid.Empty || year is < 2000 or > 9999 || month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "A valid teacher and month are required.");
        return _repository.GetPlatformCommissionsPageAsync(year, month, unsettledOnly, page,
            cancellationToken, tenantId);
    }

    public Task<Result<PlatformCommissionCollectionRequestResult>> RequestMonthlyCollectionAsync(
        Guid administratorId, Guid tenantId, int year, int month, CancellationToken cancellationToken)
    {
        if (administratorId == Guid.Empty || tenantId == Guid.Empty
            || year is < 2000 or > 9999 || month is < 1 or > 12)
            return Task.FromResult(Result<PlatformCommissionCollectionRequestResult>.Failure(CommissionErrors.InvalidSettlement));

        var fromUtc = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var toUtc = fromUtc.AddMonths(1);
        var requestedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var due = await _repository.GetUnsettledPlatformCommissionsAsync(
                tenantId, fromUtc, toUtc, cancellationToken);
            var notYetRequested = due.Where(x => !x.CollectionRequestedAtUtc.HasValue).ToList();
            if (notYetRequested.Count == 0)
                return Result<PlatformCommissionCollectionRequestResult>.Failure(CommissionErrors.AlreadyRequested);

            var total = notYetRequested.Sum(x => x.Amount);
            foreach (var commission in notYetRequested)
                commission.RequestCollection(administratorId, requestedAtUtc);

            await _notifications.NotifyTeacherAsync(tenantId,
                NotificationType.PlatformCommissionPaymentRequested,
                $"Platform commission payment requested for {year:D4}-{month:D2}: {total:0.00} EGP. Please contact the platform administrator for payment instructions, then share your transfer reference.",
                null, cancellationToken);
            _auditRepository.Add(AuditLog.Create(tenantId, administratorId,
                AuditAction.PlatformCommissionCollectionRequested, null,
                $"Platform commission collection requested: {year:D4}-{month:D2}; count={notYetRequested.Count}; amount={total:0.00}."));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<PlatformCommissionCollectionRequestResult>.Success(
                new PlatformCommissionCollectionRequestResult(tenantId, year, month,
                    notYetRequested.Count, total, requestedAtUtc));
        }, IsolationLevel.Serializable, cancellationToken);
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
    public static readonly Error AlreadyRequested = new("Finance.CommissionCollectionAlreadyRequested",
        "A collection request has already been sent for all unpaid commissions in this month.");
}
