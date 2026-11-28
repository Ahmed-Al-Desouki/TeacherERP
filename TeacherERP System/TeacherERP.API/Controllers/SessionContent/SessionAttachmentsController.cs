// موجود (تعديل) — TeacherERP.API/Controllers/SessionAttachmentsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeacherERP.API.Extensions;
using TeacherERP.API.Security;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.SessionContent;
using TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment;
using TeacherERP.Application.Features.SessionContent.GetSessionAttachments;
using TeacherERP.Application.Features.SessionContent.UploadSessionAttachment;
using TeacherERP.Application.SessionContent;
using TeacherERP.Domain.Entities;

namespace TeacherERP.API.Controllers
{
    // محتوى الحصص (سبورة، شرح، واجب...). تحتاج تحديد المدرس (X-Teacher-Id أو عضوية واحدة).
    // TenantMember عمداً: الطالب المسؤول يدخل هنا، والـ Handlers هي اللي تتحقق من صلاحيته على المجموعة
    [Authorize]
    public class SessionAttachmentsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public SessionAttachmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // multipart/form-data: file (الملف) + kind (Board/Explanation/Homework/HomeworkAnswer/Other) + caption (اختياري)
        [HttpPost("~/api/Schedule/sessions/{sessionId:guid}/attachments")]
        [Authorize(Policy = AuthPolicies.TenantMember)]
        [EnableRateLimiting(RateLimitPolicies.ContentUpload)]
        [RequestSizeLimit(SessionContentPolicy.MaxRequestBytes)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(
            Guid sessionId, [FromForm] UploadSessionAttachmentForm form, CancellationToken cancellationToken)
        {
            if (form.File is null)
            {
                return ToErrorResponse(SessionContentErrors.FileRequired.Code, SessionContentErrors.FileRequired.Message);
            }

            if (form.Kind is null)
            {
                return ToErrorResponse(SessionContentErrors.KindRequired.Code, SessionContentErrors.KindRequired.Message);
            }

            await using var stream = form.File.OpenReadStream();

            var result = await _mediator.Send(
                new UploadSessionAttachmentCommand(sessionId, form.Kind.Value, form.Caption, form.File.FileName, stream),
                cancellationToken);

            return ToResponse(result);
        }

        // ?kind=Homework&page=1&pageSize=20
        [HttpGet("~/api/Schedule/sessions/{sessionId:guid}/attachments")]
        [Authorize(Policy = AuthPolicies.TenantMember)]
        public async Task<IActionResult> GetAll(
            Guid sessionId,
            [FromQuery] SessionAttachmentKind? kind,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = PageRequest.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetSessionAttachmentsQuery(sessionId, kind, page, pageSize), cancellationToken);
            return ToResponse(result);
        }

        [HttpDelete("~/api/Schedule/attachments/{id:guid}")]
        [Authorize(Policy = AuthPolicies.TenantMember)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteSessionAttachmentCommand(id), cancellationToken);
            return ToResponse(result, new { Message = "The file was deleted." });
        }
    }

    // حقول الفورم (الأسماء غير حساسة لحالة الحروف)
    public sealed class UploadSessionAttachmentForm
    {
        public IFormFile? File { get; set; }
        public SessionAttachmentKind? Kind { get; set; }
        public string? Caption { get; set; }
    }
}
