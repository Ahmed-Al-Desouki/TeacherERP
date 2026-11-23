using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Dashboards;

public sealed class GetTeacherDashboardQueryHandler
    : IRequestHandler<GetTeacherDashboardQuery, Result<TeacherDashboardDto>>
{
    private readonly ICurrentTenantAccessor _currentTenant;
    private readonly ITenantRepository _tenantRepository;
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IMonthlyCollectionReportService _collectionReports;
    private readonly IPlatformCommissionService _commissionService;
    private readonly TimeProvider _timeProvider;

    public GetTeacherDashboardQueryHandler(
        ICurrentTenantAccessor currentTenant,
        ITenantRepository tenantRepository,
        IDashboardRepository dashboardRepository,
        IMonthlyCollectionReportService collectionReports,
        IPlatformCommissionService commissionService,
        TimeProvider timeProvider)
    {
        _currentTenant = currentTenant;
        _tenantRepository = tenantRepository;
        _dashboardRepository = dashboardRepository;
        _collectionReports = collectionReports;
        _commissionService = commissionService;
        _timeProvider = timeProvider;
    }

    public async Task<Result<TeacherDashboardDto>> Handle(
        GetTeacherDashboardQuery request, CancellationToken cancellationToken)
    {
        if (_currentTenant.Current is null)
            return Result<TeacherDashboardDto>.Failure(TenantErrors.Required);

        var tenantId = _currentTenant.TenantId;
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null)
            return Result<TeacherDashboardDto>.Failure(TenantErrors.Forbidden);

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var (dayStartUtc, dayEndUtc) = GetTenantDayBounds(nowUtc, tenant.TimeZoneId);
        var operational = await _dashboardRepository.GetTeacherStatsAsync(
            tenantId, dayStartUtc, dayEndUtc, nowUtc, cancellationToken);

        var periodYear = nowUtc.Year;
        var periodMonth = nowUtc.Month;
        var reportResult = await _collectionReports.GetAsync(
            periodYear, periodMonth, new PageRequest(1, 1), new PageRequest(1, 1),
            cancellationToken, tenantId);
        if (reportResult.IsFailure)
            return Result<TeacherDashboardDto>.Failure(reportResult.Error);

        var commission = (await _commissionService.GetSummariesAsync(
                periodYear, periodMonth, cancellationToken, tenantId))
            .SingleOrDefault();
        var totals = reportResult.Value.Totals;

        return Result<TeacherDashboardDto>.Success(new TeacherDashboardDto(
            nowUtc,
            periodYear,
            periodMonth,
            operational.ActiveGroups,
            operational.ActiveStudents,
            operational.SessionsToday,
            operational.PendingEnrollmentRequests,
            operational.PendingPaymentReviews,
            totals.SuccessfulCount,
            totals.SuccessfulAmount,
            totals.PendingAmount,
            totals.OutstandingDebtCount,
            totals.OutstandingDebtAmount,
            commission?.OutstandingAmount ?? 0m,
            operational.UpcomingSessions));
    }

    private static (DateTime StartUtc, DateTime EndUtc) GetTenantDayBounds(DateTime nowUtc, string timeZoneId)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localNow = TimeZoneInfo.ConvertTime(new DateTimeOffset(nowUtc), timeZone).DateTime;
            var localStart = DateTime.SpecifyKind(localNow.Date, DateTimeKind.Unspecified);
            var localEnd = localStart.AddDays(1);
            return (TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone),
                TimeZoneInfo.ConvertTimeToUtc(localEnd, timeZone));
        }
        catch (TimeZoneNotFoundException)
        {
            return UtcDayBounds(nowUtc);
        }
        catch (InvalidTimeZoneException)
        {
            return UtcDayBounds(nowUtc);
        }
        catch (ArgumentException)
        {
            return UtcDayBounds(nowUtc);
        }
    }

    private static (DateTime StartUtc, DateTime EndUtc) UtcDayBounds(DateTime nowUtc)
    {
        var start = DateTime.SpecifyKind(nowUtc.Date, DateTimeKind.Utc);
        return (start, start.AddDays(1));
    }
}
