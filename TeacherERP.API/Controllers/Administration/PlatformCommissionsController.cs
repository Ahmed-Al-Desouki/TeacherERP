using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.API.Controllers;

[Authorize(Policy = AuthPolicies.AdministratorOnly)]
[Route("api/administration/platform-commissions")]
public sealed class PlatformCommissionsController : BaseApiController
{
    private readonly IPlatformCommissionService _commissions;
    private readonly IPlatformCommissionSettingsService _settings;

    public PlatformCommissionsController(IPlatformCommissionService commissions,
        IPlatformCommissionSettingsService settings)
    {
        _commissions = commissions;
        _settings = settings;
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken) =>
        Ok(await _settings.GetAsync(cancellationToken));

    [HttpPost("request-month")]
    public async Task<IActionResult> RequestMonth([FromBody] RequestPlatformCommissionCollection request,
        CancellationToken cancellationToken)
    {
        var actor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(actor, out var administratorId)) return Unauthorized();
        var result = await _commissions.RequestMonthlyCollectionAsync(administratorId,
            request.TenantId, request.Year, request.Month, cancellationToken);
        return ToResponse(result);
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdatePlatformCommissionSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var actor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(actor, out var administratorId)) return Unauthorized();
        var result = await _settings.UpdateAsync(administratorId, request.PerSessionAmount,
            request.MonthlySubscriptionAmount, cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        if (year is < 2000 or > 9999 || month is < 1 or > 12)
            return BadRequest(new { errorCode = "Finance.InvalidCommissionPeriod", errorMessage = "A valid year and month are required." });
        return Ok(await _commissions.GetSummariesAsync(year, month, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> GetMonth([FromQuery] int year, [FromQuery] int month,
        [FromQuery] bool unsettledOnly = true, [FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize, CancellationToken cancellationToken = default)
    {
        if (year is < 2000 or > 9999 || month is < 1 or > 12 || page < 1
            || pageSize is < 1 or > PageRequest.MaxPageSize)
            return BadRequest(new { errorCode = "Finance.InvalidCommissionPeriod", errorMessage = "A valid month and page are required." });

        var result = await _commissions.GetPageAsync(year, month, unsettledOnly,
            new PageRequest(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpPost("settle-month")]
    public async Task<IActionResult> SettleMonth([FromBody] SettlePlatformCommissionRequest request,
        CancellationToken cancellationToken)
    {
        var actor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(actor, out var administratorId)) return Unauthorized();
        var result = await _commissions.SettleMonthAsync(administratorId, request.TenantId,
            request.Year, request.Month, request.SettlementReference ?? string.Empty, cancellationToken);
        return ToResponse(result);
    }
}

public sealed record SettlePlatformCommissionRequest(Guid TenantId, int Year, int Month,
    string? SettlementReference);

public sealed record UpdatePlatformCommissionSettingsRequest(decimal PerSessionAmount,
    decimal MonthlySubscriptionAmount);

public sealed record RequestPlatformCommissionCollection(Guid TenantId, int Year, int Month);
