using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Features.DataRetention;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly AppDbContext _context;

        public SessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(LoginSession session)
        {
            _context.LoginSessions.Add(session);
        }

        public Task<List<LoginSession>> GetActiveByUserAsync(Guid userId, DateTime nowUtc, CancellationToken cancellationToken)
        {
            return _context.LoginSessions
                .Where(s => s.UserId == userId
                            && s.RevokedAt == null
                            && s.RefreshExpiresAt > nowUtc
                            && s.AbsoluteExpiresAt > nowUtc)
                .ToListAsync(cancellationToken);
        }

        public Task<LoginSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            return _context.LoginSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
        }

        public async Task<bool> TryRotateRefreshTokenAsync(
            LoginSession rotatedSession,
            string expectedCurrentHash,
            DateTime nowUtc,
            CancellationToken cancellationToken)
        {
            var affectedRows = await _context.LoginSessions
                .Where(s => s.Id == rotatedSession.Id
                            && s.RefreshTokenHash == expectedCurrentHash
                            && s.RevokedAt == null
                            && s.RefreshExpiresAt > nowUtc
                            && s.AbsoluteExpiresAt > nowUtc)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.PreviousRefreshTokenHash, rotatedSession.PreviousRefreshTokenHash)
                    .SetProperty(s => s.RefreshTokenHash, rotatedSession.RefreshTokenHash)
                    .SetProperty(s => s.RefreshExpiresAt, rotatedSession.RefreshExpiresAt)
                    .SetProperty(s => s.LastRefreshedAt, rotatedSession.LastRefreshedAt)
                    .SetProperty(s => s.UpdatedAt, rotatedSession.UpdatedAt),
                    cancellationToken);

            return affectedRows == 1;
        }

        public async Task<int> DeleteEndedBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken)
        {
            // الجلسة "انتهت" قبل cutoff إذا أُلغيت أو انتهى أي من أجليها قبله.
            var ids = await _context.LoginSessions.IgnoreQueryFilters()
                .Where(s => s.RevokedAt < cutoffUtc
                            || s.RefreshExpiresAt < cutoffUtc
                            || s.AbsoluteExpiresAt < cutoffUtc)
                .OrderBy(s => s.Id)
                .Select(s => s.Id)
                .Take(Math.Clamp(batchSize, 1, DataRetentionPolicy.DatabaseBatchSize))
                .ToListAsync(cancellationToken);

            if (ids.Count == 0) return 0;
            return await _context.LoginSessions.IgnoreQueryFilters()
                .Where(s => ids.Contains(s.Id)
                    && (s.RevokedAt < cutoffUtc || s.RefreshExpiresAt < cutoffUtc || s.AbsoluteExpiresAt < cutoffUtc))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
