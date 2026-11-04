// موجود (تعديل) — TeacherERP.API/Controllers/TeachersController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.API.Security;
using TeacherERP.Application.Features.Tenants.GetCurrentTenant;
using TeacherERP.Application.Features.Tenants.GetMyTenants;
using TeacherERP.Application.Features.Tenants.GetTenantSettings;
using TeacherERP.Application.Features.Tenants.UpdateTenantProfile;
using TeacherERP.Application.Features.Tenants.UpdateTenantSettings;

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

        // القيم الحالية لسياسات المدرس (الظهور في البحث، والموافقات التلقائية) (للمدرس صاحب المساحة فقط)
        [HttpGet("current/settings")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> GetCurrentSettings(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTenantSettingsQuery(), cancellationToken);
            return ToResponse(result);
        }

        // ظهوري في بحث الطلاب والموافقات التلقائية (للمدرس صاحب المساحة فقط). استبدال كامل
        [HttpPut("current/settings")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> UpdateCurrentSettings([FromBody] UpdateTenantSettingsCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }

        // اسم المدرس ومادته (للمدرس صاحب المساحة فقط)
        [HttpPut("current/profile")]
        [Authorize(Policy = AuthPolicies.TenantOwner)]
        public async Task<IActionResult> UpdateCurrentProfile([FromBody] UpdateTenantProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result, new { Message = "The teacher profile was updated." });
        }
    }
}