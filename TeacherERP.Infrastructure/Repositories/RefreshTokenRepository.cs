using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }

        public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsUsed && rt.ExpiryDate > now)
                .ToListAsync(cancellationToken);

            // الإبطال عبر دالة الكيان، والحفظ يتم من الـ Handler عبر IUnitOfWork
            foreach (var token in activeTokens)
            {
                token.Revoke();
            }
        }
    }
}