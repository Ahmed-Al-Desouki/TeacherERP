// جديد — TeacherERP.Infrastructure/Repositories/SessionAttachmentRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    // داخل المدرس الحالي (الفلتر العام يعمل على الجدول)
    public class SessionAttachmentRepository : ISessionAttachmentRepository
    {
        private readonly AppDbContext _context;

        public SessionAttachmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(SessionAttachment attachment) => _context.SessionAttachments.Add(attachment);

        public void Remove(SessionAttachment attachment) => _context.SessionAttachments.Remove(attachment);

        public Task<SessionAttachment?> GetByIdAsync(Guid attachmentId, CancellationToken cancellationToken)
        {
            return _context.SessionAttachments.FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken);
        }

        public Task<int> CountAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            return _context.SessionAttachments.CountAsync(a => a.SessionId == sessionId, cancellationToken);
        }

        public Task<PagedResult<SessionAttachmentItem>> GetPageAsync(
            Guid sessionId, SessionAttachmentKind? kind, PageRequest page, CancellationToken cancellationToken)
        {
            // اسم الرافع بـ Left Join (الحساب ممكن يتمسح والسجل يفضل)
            var query = from a in _context.SessionAttachments.AsNoTracking()
                        join u in _context.Users.AsNoTracking() on a.UploadedByUserId equals u.Id into uploaders
                        from u in uploaders.DefaultIfEmpty()
                        where a.SessionId == sessionId && (kind == null || a.Kind == kind)
                        orderby a.CreatedAt descending, a.Id
                        select new SessionAttachmentItem(
                            a.Id,
                            a.SessionId,
                            a.Kind,
                            a.Url,
                            a.FileName,
                            a.ContentType,
                            a.SizeBytes,
                            a.Caption,
                            a.UploadedByUserId,
                            u == null ? null : u.FullName,
                            a.CreatedAt);

            return query.ToPagedAsync(page, cancellationToken);
        }

        public Task<List<SessionAttachmentItem>> GetForSessionsAsync(Guid tenantId,
            IReadOnlyCollection<Guid> sessionIds, CancellationToken cancellationToken)
        {
            if (sessionIds.Count == 0) return Task.FromResult(new List<SessionAttachmentItem>());

            var query = from attachment in _context.SessionAttachments.IgnoreQueryFilters().AsNoTracking()
                        join uploader in _context.Users.IgnoreQueryFilters().AsNoTracking()
                            on attachment.UploadedByUserId equals uploader.Id into uploaders
                        from uploader in uploaders.DefaultIfEmpty()
                        where attachment.TenantId == tenantId && sessionIds.Contains(attachment.SessionId)
                        orderby attachment.CreatedAt, attachment.Id
                        select new SessionAttachmentItem(
                            attachment.Id,
                            attachment.SessionId,
                            attachment.Kind,
                            attachment.Url,
                            attachment.FileName,
                            attachment.ContentType,
                            attachment.SizeBytes,
                            attachment.Caption,
                            attachment.UploadedByUserId,
                            uploader == null ? null : uploader.FullName,
                            attachment.CreatedAt);

            return query.ToListAsync(cancellationToken);
        }
    }
}
