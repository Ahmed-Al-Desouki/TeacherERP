using Microsoft.AspNetCore.Mvc;
using TeacherERP.Domain.Shared;

namespace TeacherERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult ToResponse<T>(Result<T> result, object? successData = null)
        {
            if (result.IsSuccess)
            {
                return successData != null ? Ok(successData) : Ok(result.Value);
            }

            // تحويل الـ Error الموحد إلى استجابة خطأ واضحة ومنظمة
            return BadRequest(new
            {
                errorCode = result.Error.Code,
                errorMessage = result.Error.Message
            });
        }
    }
}