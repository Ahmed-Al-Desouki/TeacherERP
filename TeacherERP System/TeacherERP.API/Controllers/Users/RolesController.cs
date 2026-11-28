using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TeacherERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
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
