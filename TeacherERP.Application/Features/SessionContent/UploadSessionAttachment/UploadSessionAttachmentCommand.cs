// جديد — TeacherERP.Application/Features/SessionContent/UploadSessionAttachment/UploadSessionAttachmentCommand.cs
using MediatR;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.SessionContent.UploadSessionAttachment
{
    // رفع ملف لحصة: المدرس والمساعد في أي وقت، والطالب المسؤول لحصص مجموعته حول وقتها.
    // الـ Stream يملكه المستدعي (الـ Controller) ويتخلص منه
    public record UploadSessionAttachmentCommand(
        Guid SessionId,
        SessionAttachmentKind Kind,
        string? Caption,
        string FileName,
        Stream Content) : IRequest<Result<SessionAttachmentDto>>;
}