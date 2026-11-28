using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.API.Controllers;

[Authorize(Policy = AuthPolicies.AdministratorOnly)]
[Route("api/administration/financial-reports")]
public sealed class MonthlyCollectionReportsController : BaseApiController
{
    private readonly IMonthlyCollectionReportService _reports;

    public MonthlyCollectionReportsController(IMonthlyCollectionReportService reports) =>
        _reports = reports;

    [HttpGet("monthly-collections")]
    public async Task<IActionResult> GetMonthlyCollections([FromQuery] int year, [FromQuery] int month,
        [FromQuery] Guid? tenantId = null, [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize, [FromQuery] int debtPage = 1,
        [FromQuery] int debtPageSize = PageRequest.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (!ValidPage(page, pageSize) || !ValidPage(debtPage, debtPageSize))
            return ToErrorResponse("Finance.InvalidCollectionReportRequest", "Page values are invalid.");

        var report = await _reports.GetAsync(year, month, new PageRequest(page, pageSize),
            new PageRequest(debtPage, debtPageSize), cancellationToken, tenantId);
        return ToResponse(report);
    }

    private static bool ValidPage(int page, int pageSize) =>
        page >= 1 && pageSize is >= 1 and <= PageRequest.MaxPageSize;
}
