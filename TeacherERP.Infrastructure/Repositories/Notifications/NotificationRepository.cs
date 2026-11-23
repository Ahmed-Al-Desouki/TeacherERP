// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/NotificationRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Notification notification) => _context.Notifications.Add(notification);

        public Task<Guid> GetTeacherRecipientIdAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            return _context.Tenants.AsNoTracking()
                .Where(t => t.Id == tenantId)
                .Select(t => t.OwnerId)
                .FirstAsync(cancellationToken);
        }

        public async Task<List<Guid>> GetStaffRecipientIdsAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            var ownerId = await _context.Tenants
                .AsNoTracking()
                .Where(t => t.Id == tenantId)
                .Select(t => t.OwnerId)
                .FirstAsync(cancellationToken);

            // المدرس وكل مساعديه النشطين (IgnoreQueryFilters مقصود: tenantId صريح والـ Seeders/الطالب بلا سنتر محدد)
            var assistants = await _context.TenantMembers
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(m => m.TenantId == tenantId && m.Role == TenantRole.Assistant && m.Status == MembershipStatus.Active)
                .Select(m => m.UserId)
                .ToListAsync(cancellationToken);

            var recipients = new List<Guid> { ownerId };
            recipients.AddRange(assistants);

            return recipients.Distinct().ToList();
        }

        public Task<List<Guid>> GetGroupStudentRecipientIdsAsync(
            Guid tenantId, Guid groupId, CancellationToken cancellationToken)
        {
            var activeStudents = _context.TenantMembers
                .IgnoreQueryFilters()
                .Where(m => m.TenantId == tenantId
                    && m.Role == TenantRole.Student
                    && m.Status == MembershipStatus.Active);

            return _context.GroupEnrollments
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(e => e.TenantId == tenantId
                    && e.GroupId == groupId
                    && e.Status == EnrollmentStatus.Active)
                .Join(activeStudents, e => e.StudentId, member => member.UserId, (_, member) => member.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public Task<PagedResult<NotificationItem>> GetMinePageAsync(
            Guid userId, bool unreadOnly, PageRequest page, CancellationToken cancellationToken)
        {
            var query = _context.Notifications.AsNoTracking().Where(n => n.RecipientUserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return query
                .OrderByDescending(n => n.CreatedAt)
                .ThenBy(n => n.Id)
                .Select(n => new NotificationItem(n.Id, n.TenantId, n.Type, n.Message, n.RelatedId, n.IsRead, n.CreatedAt))
                .ToPagedAsync(page, cancellationToken);
        }

        public Task<Notification?> GetOwnTrackedAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken)
        {
            return _context.Notifications.FirstOrDefaultAsync(
                n => n.Id == notificationId && n.RecipientUserId == userId, cancellationToken);
        }

        public Task<List<Notification>> GetOwnUnreadTrackedAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.Notifications
                .Where(n => n.RecipientUserId == userId && !n.IsRead)
                .ToListAsync(cancellationToken);
        }

        public async Task<NotificationShareItem?> GetOwnSessionAttachmentShareAsync(
            Guid notificationId, Guid userId, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .AsNoTracking()
                .Where(n => n.Id == notificationId && n.RecipientUserId == userId
                    && n.RelatedId != null
                    && (n.Type == NotificationType.HomeworkUploaded
                        || n.Type == NotificationType.SessionAttachmentUploaded))
                .Select(n => new { n.TenantId, AttachmentId = n.RelatedId!.Value })
                .FirstOrDefaultAsync(cancellationToken);

            if (notification is null)
            {
                return null;
            }

            var attachment = await _context.SessionAttachments
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(a => a.Id == notification.AttachmentId && a.TenantId == notification.TenantId)
                .Select(a => new { a.Kind, a.SessionId, a.FileName, a.Caption, a.Url })
                .FirstOrDefaultAsync(cancellationToken);

            if (attachment is null)
            {
                return null;
            }

            return await (
                from session in _context.Sessions.IgnoreQueryFilters().AsNoTracking()
                join targetGroup in _context.Groups.IgnoreQueryFilters().AsNoTracking()
                    on session.GroupId equals targetGroup.Id
                where session.Id == attachment.SessionId
                    && session.TenantId == notification.TenantId
                    && targetGroup.TenantId == notification.TenantId
                    && _context.GroupEnrollments.IgnoreQueryFilters().Any(enrollment =>
                        enrollment.TenantId == notification.TenantId
                        && enrollment.GroupId == targetGroup.Id
                        && enrollment.StudentId == userId
                        && enrollment.Status == EnrollmentStatus.Active)
                    && _context.TenantMembers.IgnoreQueryFilters().Any(member =>
                        member.TenantId == notification.TenantId
                        && member.UserId == userId
                        && member.Role == TenantRole.Student
                        && member.Status == MembershipStatus.Active)
                select new NotificationShareItem(
                    attachment.Kind,
                    targetGroup.Name,
                    attachment.FileName,
                    attachment.Caption,
                    attachment.Url,
                    targetGroup.WhatsAppGroupLink))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
