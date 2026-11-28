using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Dashboards;

public sealed class GetAdministratorDashboardQueryHandler
    : IRequestHandler<GetAdministratorDashboardQuery, Result<AdministratorDashboardDto>>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IMonthlyCollectionReportService _collectionReports;
    private readonly IPlatformCommissionService _commissionService;
    private readonly TimeProvider _timeProvider;

    public GetAdministratorDashboardQueryHandler(
        IDashboardRepository dashboardRepository,
        IMonthlyCollectionReportService collectionReports,
        IPlatformCommissionService commissionService,
        TimeProvider timeProvider)
    {
        _dashboardRepository = dashboardRepository;
        _collectionReports = collectionReports;
        _commissionService = commissionService;
        _timeProvider = timeProvider;
    }

    public async Task<Result<AdministratorDashboardDto>> Handle(
        GetAdministratorDashboardQuery request, CancellationToken cancellationToken)
    {
        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var periodYear = nowUtc.Year;
        var periodMonth = nowUtc.Month;
        var operational = await _dashboardRepository.GetAdministratorStatsAsync(nowUtc, cancellationToken);

        var reportResult = await _collectionReports.GetAsync(
            periodYear, periodMonth, new PageRequest(1, 1), new PageRequest(1, 1), cancellationToken);
        if (reportResult.IsFailure)
            return Result<AdministratorDashboardDto>.Failure(reportResult.Error);

        var commissions = await _commissionService.GetSummariesAsync(
            periodYear, periodMonth, cancellationToken);
        var outstandingCommissionCount = commissions.Sum(item => item.OutstandingCount);
        var outstandingCommissionAmount = commissions.Sum(item => item.OutstandingAmount);
        var settledCommissionCount = commissions.Sum(item => item.SettledCount);
        var settledCommissionAmount = commissions.Sum(item => item.SettledAmount);
        var totals = reportResult.Value.Totals;

        return Result<AdministratorDashboardDto>.Success(new AdministratorDashboardDto(
            nowUtc,
            periodYear,
            periodMonth,
            operational.ActiveTeachers,
            operational.ActiveStudents,
            operational.Centers,
            operational.PendingTeacherRequests,
            operational.PendingCenterJoinRequests,
            operational.PendingPaymentReviews,
            operational.PendingPaymobAdjustments,
            totals.SuccessfulCount,
            totals.SuccessfulAmount,
            totals.PendingAmount,
            totals.OutstandingDebtCount,
            totals.OutstandingDebtAmount,
            outstandingCommissionCount,
            outstandingCommissionAmount,
            settledCommissionCount,
            settledCommissionAmount));
    }
}
