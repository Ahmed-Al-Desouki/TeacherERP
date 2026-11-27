using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.API.Controllers;

[Authorize(Policy = AuthPolicies.TenantOwner)]
[Route("api/finance/monthly-collections")]
public sealed class TeacherMonthlyCollectionReportsController : BaseApiController
{
    private readonly ICurrentTenantAccessor _tenant;
    private readonly IMonthlyCollectionReportService _reports;

    public TeacherMonthlyCollectionReportsController(ICurrentTenantAccessor tenant,
        IMonthlyCollectionReportService reports)
    {
        _tenant = tenant;
        _reports = reports;
    }

    [HttpGet]
    public async Task<IActionResult> GetMonthlyCollections([FromQuery] int year, [FromQuery] int month,
        [FromQuery] int page = 1, [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        [FromQuery] int debtPage = 1, [FromQuery] int debtPageSize = PageRequest.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (!ValidPage(page, pageSize) || !ValidPage(debtPage, debtPageSize))
            return ToErrorResponse("Finance.InvalidCollectionReportRequest", "Page values are invalid.");

        var report = await _reports.GetAsync(year, month, new PageRequest(page, pageSize),
            new PageRequest(debtPage, debtPageSize), cancellationToken, _tenant.TenantId);
        return ToResponse(report);
    }

    private static bool ValidPage(int page, int pageSize) =>
        page >= 1 && pageSize is >= 1 and <= PageRequest.MaxPageSize;
}
