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
    }
}