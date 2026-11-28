// موجود (تعديل) — TeacherERP.API/Controllers/DirectoryController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Common;
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

        // ?term=&page=1&pageSize=20 : الصفحات على مستوى السناتر، والسنتر أولاً ثم مدرسوه
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string term,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = PageRequest.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new SearchDirectoryQuery(term, page, pageSize), cancellationToken);
            return ToResponse(result);
        }

        // ?gradeLevel= اختياري، و?page=&pageSize= على مجموعات المدرس ومواعيدها
        [HttpGet("teachers/{id:guid}")]
        public async Task<IActionResult> GetTeacher(
            Guid id,
            [FromQuery] string? gradeLevel,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = PageRequest.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTeacherProfileQuery(id, gradeLevel, page, pageSize), cancellationToken);
            return ToResponse(result);
        }
    }
}
