// جديد — TeacherERP.API/Controllers/DirectoryController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Directory.GetTeacherProfile;
using TeacherERP.Application.Features.Directory.SearchDirectory;

namespace TeacherERP.API.Controllers
{
    // دليل الطلاب: البحث عن مدرس أو سنتر وعرض مواعيد المدرس (لأي مستخدم مسجّل، بدون تحديد مدرس)
    [Authorize]
    public class DirectoryController : BaseApiController
    {
        private readonly IMediator _mediator;

        public DirectoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ?term= : السنتر أولاً ثم مدرسوه
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SearchDirectoryQuery(term), cancellationToken);
            return ToResponse(result);
        }

        // ?gradeLevel= اختياري: مجموعات المدرس ومواعيدها
        [HttpGet("teachers/{id:guid}")]
        public async Task<IActionResult> GetTeacher(Guid id, [FromQuery] string? gradeLevel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTeacherProfileQuery(id, gradeLevel), cancellationToken);
            return ToResponse(result);
        }
    }
}