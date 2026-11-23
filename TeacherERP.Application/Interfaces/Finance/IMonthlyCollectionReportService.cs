using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces;

public interface IMonthlyCollectionReportService
{
    Task<Result<MonthlyCollectionReport>> GetAsync(int year, int month, PageRequest transactionPage,
        PageRequest debtPage, CancellationToken cancellationToken, Guid? tenantId = null);
}
