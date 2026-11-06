// جديد — TeacherERP.Application/Features/SessionContent/DeleteSessionAttachment/DeleteSessionAttachmentCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment
{
    // المدرس والمساعد يحذفون أي ملف، والطالب المسؤول ملفاته هو فقط وفي نافذة وقت الحصة
    public record DeleteSessionAttachmentCommand(Guid AttachmentId) : IRequest<Result<bool>>;
}