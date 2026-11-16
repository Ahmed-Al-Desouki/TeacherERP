using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    public class CenterMembershipRepository : ICenterMembershipRepository
    {
        private readonly AppDbContext _context;

        public CenterMembershipRepository(AppDbContext context) => _context = context;

        public Task<int> CountPendingRequestsAsync(Guid tenantId, CancellationToken cancellationToken)
            => _context.CenterJoinRequests.CountAsync(
                r => r.TenantId == tenantId && r.Status == CenterJoinRequestStatus.Pending, cancellationToken);

        public Task<CenterJoinRequest?> GetPendingRequestAsync(Guid centerId, Guid tenantId, CancellationToken cancellationToken)
            => _context.CenterJoinRequests.FirstOrDefaultAsync(
                r => r.CenterId == centerId && r.TenantId == tenantId && r.Status == CenterJoinRequestStatus.Pending,
                cancellationToken);

        public Task<CenterJoinRequest?> GetRequestForTeacherAsync(Guid requestId, Guid tenantId, CancellationToken cancellationToken)
            => _context.CenterJoinRequests.FirstOrDefaultAsync(
                r => r.Id == requestId && r.TenantId == tenantId, cancellationToken);

        public Task<CenterJoinRequest?> GetRequestForCenterOwnerAsync(Guid requestId, Guid ownerUserId, CancellationToken cancellationToken)
            => (from request in _context.CenterJoinRequests
                join center in _context.Centers on request.CenterId equals center.Id
                where request.Id == requestId && center.OwnerUserId == ownerUserId
                select request).FirstOrDefaultAsync(cancellationToken);

        public async Task<PagedResult<CenterJoinRequestListItem>> GetRequestsForOwnerAsync(
            Guid ownerUserId, CenterJoinRequestStatus? status, PageRequest page, CancellationToken cancellationToken)
        {
            var query = from request in _context.CenterJoinRequests.AsNoTracking()
                        join center in _context.Centers.AsNoTracking() on request.CenterId equals center.Id
                        join tenant in _context.Tenants.AsNoTracking() on request.TenantId equals tenant.Id
                        join user in _context.Users.AsNoTracking() on tenant.OwnerId equals user.Id
                        where center.OwnerUserId == ownerUserId
                        select new { request, center, tenant, user };

            if (status is { } value)
            {
                query = query.Where(x => x.request.Status == value);
            }

            return await query
                .OrderByDescending(x => x.request.CreatedAt)
                .ThenBy(x => x.request.Id)
                .Select(x => new CenterJoinRequestListItem(
                    x.request.Id, x.center.Id, x.center.Name, x.tenant.Id, x.user.FullName, x.user.Email,
                    x.tenant.Subject, x.request.Status, x.request.CreatedAt, x.request.LastNotifiedAtUtc,
                    x.request.ReviewedAtUtc, x.request.DecisionNote))
                .ToPagedAsync(page, cancellationToken);
        }

        public async Task<PagedResult<CenterJoinRequestListItem>> GetRequestsForTeacherAsync(
            Guid tenantId, PageRequest page, CancellationToken cancellationToken)
        {
            var query = from request in _context.CenterJoinRequests.AsNoTracking()
                        join center in _context.Centers.AsNoTracking() on request.CenterId equals center.Id
                        join tenant in _context.Tenants.AsNoTracking() on request.TenantId equals tenant.Id
                        join user in _context.Users.AsNoTracking() on tenant.OwnerId equals user.Id
                        where request.TenantId == tenantId
                        select new { request, center, tenant, user };

            return await query
                .OrderByDescending(x => x.request.CreatedAt)
                .ThenBy(x => x.request.Id)
                .Select(x => new CenterJoinRequestListItem(
                    x.request.Id, x.center.Id, x.center.Name, x.tenant.Id, x.user.FullName, x.user.Email,
                    x.tenant.Subject, x.request.Status, x.request.CreatedAt, x.request.LastNotifiedAtUtc,
                    x.request.ReviewedAtUtc, x.request.DecisionNote))
                .ToPagedAsync(page, cancellationToken);
        }

        public Task<CenterInvitation?> GetInvitationByHashAsync(string tokenHash, CancellationToken cancellationToken)
            => _context.CenterInvitations.FirstOrDefaultAsync(i => i.TokenHash == tokenHash, cancellationToken);

        public void Add(CenterJoinRequest request) => _context.CenterJoinRequests.Add(request);

        public void Add(CenterInvitation invitation) => _context.CenterInvitations.Add(invitation);

        public async Task<int> PurgeClosedDataBeforeAsync(DateTime cutoffUtc, CancellationToken cancellationToken)
        {
            // الطلبات المعلقة تبقى حتى يقرر صاحب السنتر أو يلغيها المدرس.
            var requests = await _context.CenterJoinRequests
                .Where(r => r.Status != CenterJoinRequestStatus.Pending
                    && (r.UpdatedAt ?? r.CreatedAt) <= cutoffUtc)
                .ToListAsync(cancellationToken);
            _context.CenterJoinRequests.RemoveRange(requests);

            var invitations = await _context.CenterInvitations
                .Where(i => (i.Status == CenterInvitationStatus.Pending && i.ExpiresAtUtc <= cutoffUtc)
                    || (i.Status != CenterInvitationStatus.Pending && (i.UpdatedAt ?? i.ExpiresAtUtc) <= cutoffUtc))
                .ToListAsync(cancellationToken);
            _context.CenterInvitations.RemoveRange(invitations);

            return requests.Count + invitations.Count;
        }
    }
}
