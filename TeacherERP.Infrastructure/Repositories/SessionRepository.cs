using Microsoft.EntityFrameworkCore;
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

        public Task<List<LoginSession>> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            return _context.LoginSessions
                .Where(s => s.UserId == userId
                            && s.RevokedAt == null
                            && s.RefreshExpiresAt > now
                            && s.AbsoluteExpiresAt > now)
                .ToListAsync(cancellationToken);
        }
    }
}