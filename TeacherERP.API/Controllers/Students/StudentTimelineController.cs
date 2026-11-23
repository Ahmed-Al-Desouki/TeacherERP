using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Students.GetLearningTimeline;

namespace TeacherERP.API.Controllers;

[Authorize(Policy = AuthPolicies.TenantStudent)]
public sealed class StudentTimelineController : BaseApiController
{
    private readonly IMediator _mediator;

    public StudentTimelineController(IMediator mediator) => _mediator = mediator;

    [HttpGet("~/api/students/me/timeline")]
    public async Task<IActionResult> GetTimeline(
        [FromQuery] GetStudentLearningTimelineQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return ToResponse(result);
    }
}
