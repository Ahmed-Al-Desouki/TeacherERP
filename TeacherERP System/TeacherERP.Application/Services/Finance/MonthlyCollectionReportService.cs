using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Finance;

public sealed class MonthlyCollectionReportService : IMonthlyCollectionReportService
{
    private readonly IFinancialLedgerRepository _repository;

    public MonthlyCollectionReportService(IFinancialLedgerRepository repository) =>
        _repository = repository;

    public Task<Result<MonthlyCollectionReport>> GetAsync(int year, int month,
        PageRequest transactionPage, PageRequest debtPage, CancellationToken cancellationToken,
        Guid? tenantId = null)
    {
        if (year is < 2000 or > 9999 || month is < 1 or > 12
            || transactionPage.Page < 1 || transactionPage.PageSize is < 1 or > PageRequest.MaxPageSize
            || debtPage.Page < 1 || debtPage.PageSize is < 1 or > PageRequest.MaxPageSize
            || tenantId == Guid.Empty)
            return Task.FromResult(Result<MonthlyCollectionReport>.Failure(
                new Error("Finance.InvalidCollectionReportRequest", "A valid month, tenant, and page are required.")));

        return GetCoreAsync(year, month, transactionPage, debtPage, cancellationToken, tenantId);
    }

    private async Task<Result<MonthlyCollectionReport>> GetCoreAsync(int year, int month,
        PageRequest transactionPage, PageRequest debtPage, CancellationToken cancellationToken,
        Guid? tenantId)
    {
        var report = await _repository.GetMonthlyCollectionReportAsync(year, month,
            transactionPage, debtPage, cancellationToken, tenantId);
        return Result<MonthlyCollectionReport>.Success(report);
    }
}
