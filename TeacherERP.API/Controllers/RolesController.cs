using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Users.Commands.PromoteToAdmin;
using TeacherERP.Application.Features.Users.Commands.PromoteToSuperAdmin;

namespace TeacherERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ISender _mediator;

        public RolesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}/promote-to-admin")]
        [Authorize(Roles = "SuperAdmin")] // حماية تامة للمدرس الرئيسي فقط
        public async Task<IActionResult> PromoteToAdmin(Guid id, CancellationToken cancellationToken)
        {
            var command = new PromoteToAdminCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new { Message = "User has been successfully promoted to Admin." });
        }

        [HttpPost("{id}/promote-to-super-admin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> PromoteToSuperAdmin(Guid id, CancellationToken cancellationToken)// أو IActionResult حسب طريقة الـ Response عندك
        {
            var command = new PromoteToSuperAdminCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new { Message = "User has been successfully promoted to Super Admin." });
        }

        //[Authorize(Policy = AuthPolicies.CompletedProfile)]
        [HttpGet("my-roles")]
        [Authorize]
        public IActionResult GetMyRoles()
        {
            var roles = User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new { Email = User.Identity?.Name, Roles = roles });
        }
    }
}