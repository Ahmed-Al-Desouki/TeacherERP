// جديد — TeacherERP.Application/Features/SessionContent/SessionContentDtos.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.SessionContent
{
    // الأوقات UTC. Kind يظهر كنص
    public record SessionAttachmentDto(
        Guid Id,
        Guid SessionId,
        SessionAttachmentKind Kind,
        string Url,
        string FileName,
        string ContentType,
        long SizeBytes,
        string? Caption,
        Guid UploadedByUserId,
        string? UploadedByName,
        DateTime CreatedAt)
    {
        public static SessionAttachmentDto From(SessionAttachmentItem item)
            => new(item.Id, item.SessionId, item.Kind, item.Url, item.FileName, item.ContentType, item.SizeBytes,
                   item.Caption, item.UploadedByUserId, item.UploadedByName, item.CreatedAt);

        public static SessionAttachmentDto From(SessionAttachment entity, string? uploadedByName)
            => new(entity.Id, entity.SessionId, entity.Kind, entity.Url, entity.FileName, entity.ContentType, entity.SizeBytes,
                   entity.Caption, entity.UploadedByUserId, uploadedByName, entity.CreatedAt);
    }
}