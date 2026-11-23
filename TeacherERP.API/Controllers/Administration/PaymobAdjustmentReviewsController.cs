using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.API.Controllers;

[Authorize(Policy = AuthPolicies.AdministratorOnly)]
[Route("api/administration/paymob-adjustments")]
public sealed class PaymobAdjustmentReviewsController : BaseApiController
{
    private readonly IPaymobAdjustmentReviewService _reviews;

    public PaymobAdjustmentReviewsController(IPaymobAdjustmentReviewService reviews) => _reviews = reviews;

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending([FromQuery] int page = 1,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > PageRequest.MaxPageSize)
            return BadRequest(new { errorCode = "Finance.InvalidPaymobAdjustmentPage", errorMessage = "A valid page is required." });
        return Ok(await _reviews.GetPendingPageAsync(new PageRequest(page, pageSize), cancellationToken));
    }

    [HttpPost("{reviewId:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid reviewId,
        [FromBody] ResolvePaymobAdjustmentBody body, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var administratorId))
            return Unauthorized();

        var result = await _reviews.ResolveAsync(administratorId, reviewId,
            new ResolvePaymobAdjustmentRequest(body.Approved, body.StudentDebtAdjustmentAmount,
                body.CommissionReversalAmount, body.DecisionNote ?? string.Empty), cancellationToken);
        return ToResponse(result);
    }
}

public sealed record ResolvePaymobAdjustmentBody(bool Approved,
    decimal StudentDebtAdjustmentAmount, decimal CommissionReversalAmount, string? DecisionNote);
