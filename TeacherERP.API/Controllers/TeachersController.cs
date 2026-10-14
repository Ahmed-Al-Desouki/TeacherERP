using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;

namespace TeacherERP.API.Controllers
{
    // مساحات المدرسين التي أنا عضو فيها (المسار: /api/Teachers). الاسم الداخلي Tenant، وللعميل "مدرس"
    [Authorize]
    public class TeachersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TeachersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // كل المدرسين اللي أنا عضو عندهم (لا تحتاج مدرس محدد)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyTeachers(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyTenantsQuery(), cancellationToken);
            return ToResponse(result);
        }

        // المدرس الحالي للطلب (بالـ Header X-Teacher-Id أو تلقائياً لو عضوية واحدة)
        [HttpGet("current")]
        [Authorize(Policy = AuthPolicies.TenantMember)]
        public async Task<IActionResult> GetCurrentTeacher(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentTenantQuery(), cancellationToken);
            return ToResponse(result);
        }
    }
}