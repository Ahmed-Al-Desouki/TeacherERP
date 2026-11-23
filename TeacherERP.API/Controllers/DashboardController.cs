using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Dashboards;

namespace TeacherERP.API.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController : BaseApiController
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet("teacher")]
    [Authorize(Policy = AuthPolicies.TenantOwner)]
    public async Task<IActionResult> GetTeacherDashboard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTeacherDashboardQuery(), cancellationToken);
        return ToResponse(result);
    }

    [HttpGet("administrator")]
    [Authorize(Policy = AuthPolicies.AdministratorOnly)]
    public async Task<IActionResult> GetAdministratorDashboard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAdministratorDashboardQuery(), cancellationToken);
        return ToResponse(result);
    }
}
