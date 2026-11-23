using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces;

public sealed record PlatformCommissionSettlementResult(Guid TenantId, int Year, int Month,
    int CommissionCount, decimal Amount, DateTime SettledAtUtc, string SettlementReference);

public interface IPlatformCommissionService
{
    Task<PagedResult<PlatformCommissionItem>> GetPageAsync(int year, int month, bool unsettledOnly,
        PageRequest page, CancellationToken cancellationToken);
    Task<List<PlatformCommissionSummary>> GetSummariesAsync(int year, int month,
        CancellationToken cancellationToken, Guid? tenantId = null);
    Task<PagedResult<PlatformCommissionItem>> GetTeacherPageAsync(Guid tenantId, int year, int month,
        bool unsettledOnly, PageRequest page, CancellationToken cancellationToken);
    Task<Result<PlatformCommissionCollectionRequestResult>> RequestMonthlyCollectionAsync(Guid administratorId,
        Guid tenantId, int year, int month, CancellationToken cancellationToken);
    Task<Result<PlatformCommissionSettlementResult>> SettleMonthAsync(Guid administratorId,
        Guid tenantId, int year, int month, string settlementReference, CancellationToken cancellationToken);
}

public sealed record PlatformCommissionCollectionRequestResult(Guid TenantId, int Year, int Month,
    int CommissionCount, decimal Amount, DateTime RequestedAtUtc);
