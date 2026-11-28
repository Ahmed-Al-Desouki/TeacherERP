// جديد — TeacherERP.Application/Interfaces/ISessionAttachmentRepository.cs
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public record SessionAttachmentItem(
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
        DateTime CreatedAt);

    // داخل المدرس الحالي (الفلتر العام يعمل). الحفظ يتم عبر IUnitOfWork
    public interface ISessionAttachmentRepository
    {
        void Add(SessionAttachment attachment);
        void Remove(SessionAttachment attachment);

        Task<SessionAttachment?> GetByIdAsync(Guid attachmentId, CancellationToken cancellationToken);
        Task<int> CountAsync(Guid sessionId, CancellationToken cancellationToken);

        Task<PagedResult<SessionAttachmentItem>> GetPageAsync(
            Guid sessionId, SessionAttachmentKind? kind, PageRequest page, CancellationToken cancellationToken);

        Task<List<SessionAttachmentItem>> GetForSessionsAsync(Guid tenantId,
            IReadOnlyCollection<Guid> sessionIds, CancellationToken cancellationToken);
    }
}
