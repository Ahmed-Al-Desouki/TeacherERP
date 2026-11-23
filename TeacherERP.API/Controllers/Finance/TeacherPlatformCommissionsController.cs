using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.API.Controllers;

[Authorize(Policy = AuthPolicies.TenantOwner)]
[Route("api/finance/platform-commissions")]
public sealed class TeacherPlatformCommissionsController : BaseApiController
{
    private readonly ICurrentTenantAccessor _tenant;
    private readonly IPlatformCommissionService _commissions;

    public TeacherPlatformCommissionsController(ICurrentTenantAccessor tenant,
        IPlatformCommissionService commissions)
    {
        _tenant = tenant;
        _commissions = commissions;
    }

    [HttpGet]
    public async Task<IActionResult> GetMonth([FromQuery] int year, [FromQuery] int month,
        [FromQuery] bool unsettledOnly = false, [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize, CancellationToken cancellationToken = default)
    {
        if (year is < 2000 or > 9999 || month is < 1 or > 12 || page < 1
            || pageSize is < 1 or > PageRequest.MaxPageSize)
            return BadRequest(new { errorCode = "Finance.InvalidCommissionPeriod", errorMessage = "A valid month and page are required." });

        var result = await _commissions.GetTeacherPageAsync(_tenant.TenantId, year, month,
            unsettledOnly, new PageRequest(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] int year, [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        if (year is < 2000 or > 9999 || month is < 1 or > 12)
            return BadRequest(new { errorCode = "Finance.InvalidCommissionPeriod", errorMessage = "A valid month is required." });

        var summary = (await _commissions.GetSummariesAsync(year, month, cancellationToken, _tenant.TenantId))
            .SingleOrDefault();
        if (summary is not null) return Ok(summary);
        return Ok(new
        {
            tenantId = _tenant.TenantId,
            year,
            month,
            entryCount = 0,
            totalAmount = 0m,
            settledCount = 0,
            settledAmount = 0m,
            outstandingCount = 0,
            outstandingAmount = 0m
        });
    }
}
