// جديد — TeacherERP.Infrastructure/Repositories/NotificationRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

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

        public async Task<List<Guid>> GetStaffRecipientIdsAsync(Guid tenantId, Guid? groupId, CancellationToken cancellationToken)
        {
            var ownerId = await _context.Tenants
                .AsNoTracking()
                .Where(t => t.Id == tenantId)
                .Select(t => t.OwnerId)
                .FirstAsync(cancellationToken);

            var recipients = new List<Guid> { ownerId };

            if (groupId.HasValue)
            {
                var assistants = await _context.Groups
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Where(g => g.Id == groupId.Value && g.TenantId == tenantId)
                    .SelectMany(g => g.AssignedAdmins.Select(admin => admin.Id))
                    .ToListAsync(cancellationToken);

                recipients.AddRange(assistants);
            }

            return recipients.Distinct().ToList();
        }

        public Task<List<NotificationItem>> GetMineAsync(Guid userId, bool unreadOnly, CancellationToken cancellationToken)
        {
            var query = _context.Notifications.AsNoTracking().Where(n => n.RecipientUserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return query
                .OrderByDescending(n => n.CreatedAt)
                .Take(100)
                .Select(n => new NotificationItem(n.Id, n.TenantId, n.Type, n.Message, n.RelatedId, n.IsRead, n.CreatedAt))
                .ToListAsync(cancellationToken);
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
    }
}