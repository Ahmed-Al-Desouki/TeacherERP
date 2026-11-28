// جديد — TeacherERP.Infrastructure/Repositories/TeacherAccessRequestRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    // طلبات انضمام المدرسين: الكيان غير تابع لمساحة (لا فلتر عام)، فتعمل بلا سنتر محدد
    public class TeacherAccessRequestRepository : ITeacherAccessRequestRepository
    {
        private readonly AppDbContext _context;

        public TeacherAccessRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<TeacherAccessRequest?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
        {
            return _context.TeacherAccessRequests.FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);
        }

        public Task<TeacherAccessRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.TeacherAccessRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public Task<List<TeacherAccessRequest>> GetPendingByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return _context.TeacherAccessRequests
                .Where(r => r.Email == email && r.Status == TeacherAccessRequestStatus.Pending)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountApprovedByReviewerSinceAsync(Guid reviewerUserId, DateTime sinceUtc, CancellationToken cancellationToken)
        {
            return _context.TeacherAccessRequests.CountAsync(
                r => r.Status == TeacherAccessRequestStatus.Approved
                     && r.ReviewedByUserId == reviewerUserId
                     && r.ReviewedAtUtc >= sinceUtc,
                cancellationToken);
        }

        public Task<PagedResult<TeacherAccessRequestListItem>> GetPageAsync(
            TeacherAccessRequestStatus? status, DateTime nowUtc, PageRequest page, CancellationToken cancellationToken)
        {
            var query = _context.TeacherAccessRequests.AsNoTracking();

            // الحالة الفعلية: المعلّق الذي انتهت مدته يُحسب Expired
            if (status is { } wanted)
            {
                query = wanted switch
                {
                    TeacherAccessRequestStatus.Pending => query.Where(r => r.Status == TeacherAccessRequestStatus.Pending && r.ExpiresAtUtc > nowUtc),
                    TeacherAccessRequestStatus.Expired => query.Where(r => r.Status == TeacherAccessRequestStatus.Expired
                        || (r.Status == TeacherAccessRequestStatus.Pending && r.ExpiresAtUtc <= nowUtc)),
                    _ => query.Where(r => r.Status == wanted)
                };
            }

            return query
                .OrderByDescending(r => r.CreatedAt)
                .ThenBy(r => r.Id)
                .Select(r => new TeacherAccessRequestListItem(
                    r.Id,
                    r.FullName,
                    r.Email,
                    r.Subject,
                    r.Note,
                    r.Status == TeacherAccessRequestStatus.Pending && r.ExpiresAtUtc <= nowUtc
                        ? TeacherAccessRequestStatus.Expired
                        : r.Status,
                    r.CreatedAt,
                    r.ExpiresAtUtc,
                    r.ReviewedByUserId,
                    r.ReviewedAtUtc))
                .ToPagedAsync(page, cancellationToken);
        }

        public void Add(TeacherAccessRequest request) => _context.TeacherAccessRequests.Add(request);
    }
}
