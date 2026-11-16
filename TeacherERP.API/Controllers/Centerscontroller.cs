using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.API.Extensions;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Centers.CreateCenter;
using TeacherERP.Application.Features.Centers.Invitations;
using TeacherERP.Application.Features.Centers.GetMyCenters;
using TeacherERP.Application.Features.Centers.JoinRequests;
using TeacherERP.Application.Features.Centers.UpdateCenter;

namespace TeacherERP.API.Controllers
{
    // السناتر الفعلية (الأماكن). لا تحتاج مدرس محدد، والملكية تُتحقق في الـ Handlers
    [Authorize]
    public class CentersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public CentersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // السناتر التي أملكها أو أدرّس فيها
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyCenters(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyCentersQuery(), cancellationToken);
            return ToResponse(result);
        }

        [HttpPost]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [EnableRateLimiting(RateLimitPolicies.CenterMembershipWrite)]
        public async Task<IActionResult> CreateMyCenter([FromBody] CreateMyCenterCommand command, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(command, cancellationToken));

        [HttpGet("join-requests/mine")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        public async Task<IActionResult> GetMyJoinRequests([FromQuery] GetMyCenterJoinRequestsQuery query, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(query, cancellationToken));

        [HttpGet("join-requests/received")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        public async Task<IActionResult> GetReceivedJoinRequests([FromQuery] GetCenterJoinRequestsQuery query, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(query, cancellationToken));

        [HttpPost("{centerId:guid}/join-requests")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [EnableRateLimiting(RateLimitPolicies.CenterMembershipWrite)]
        public async Task<IActionResult> JoinCenter(Guid centerId, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(new JoinCenterCommand(centerId), cancellationToken));

        [HttpPost("join-requests/{requestId:guid}/resend")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [EnableRateLimiting(RateLimitPolicies.CenterMembershipWrite)]
        public async Task<IActionResult> ResendJoinRequest(Guid requestId, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(new ResendCenterJoinRequestCommand(requestId), cancellationToken));

        [HttpDelete("join-requests/{requestId:guid}")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [EnableRateLimiting(RateLimitPolicies.CenterMembershipWrite)]
        public async Task<IActionResult> CancelJoinRequest(Guid requestId, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(new CancelCenterJoinRequestCommand(requestId), cancellationToken));

        [HttpPost("join-requests/{requestId:guid}/approve")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [EnableRateLimiting(RateLimitPolicies.CenterMembershipWrite)]
        public async Task<IActionResult> ApproveJoinRequest(Guid requestId, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(new ApproveCenterJoinRequestCommand(requestId), cancellationToken));

        [HttpPost("join-requests/{requestId:guid}/reject")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [EnableRateLimiting(RateLimitPolicies.CenterMembershipWrite)]
        public async Task<IActionResult> RejectJoinRequest(Guid requestId, [FromBody] RejectCenterJoinRequestCommand command, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(command with { RequestId = requestId }, cancellationToken));

        [HttpPost("{centerId:guid}/invitations")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [EnableRateLimiting(RateLimitPolicies.CenterMembershipWrite)]
        public async Task<IActionResult> CreateInvitation(Guid centerId, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(new CreateCenterInvitationCommand(centerId), cancellationToken));

        [HttpPost("invitations/accept")]
        [Authorize(Policy = AuthPolicies.TeacherOnly)]
        [EnableRateLimiting(RateLimitPolicies.CenterMembershipWrite)]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptCenterInvitationCommand command, CancellationToken cancellationToken)
            => ToResponse(await _mediator.Send(command, cancellationToken));

        // تعديل بيانات السنتر وموقعه (لصاحبه فقط)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCenter(Guid id, [FromBody] UpdateCenterRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCenterCommand(
                id, request.Name, request.Address, request.Latitude, request.Longitude, request.GeofenceRadiusMeters);

            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }
    }
}
