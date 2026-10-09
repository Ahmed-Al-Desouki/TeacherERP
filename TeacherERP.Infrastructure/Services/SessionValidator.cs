using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Services
{
    public class SessionValidator : ISessionValidator
    {
        // الإلغاء يمسح الكاش فوراً، فالمدة تؤثر فقط على الإيقاف الإداري (خلال 30 ثانية على الأكثر)
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeProvider _timeProvider;

        public SessionValidator(AppDbContext context, IMemoryCache cache, TimeProvider timeProvider)
        {
            _context = context;
            _cache = cache;
            _timeProvider = timeProvider;
        }

        public async Task<bool> IsActiveAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            var key = BuildKey(sessionId);
            if (_cache.TryGetValue(key, out bool cached))
            {
                return cached;
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            // الجلسة نشطة + صاحبها غير موقوف أو محظور، في Query واحد
            var isActive = await _context.LoginSessions
                .AsNoTracking()
                .AnyAsync(s => s.Id == sessionId
                               && s.RevokedAt == null
                               && s.RefreshExpiresAt > now
                               && s.AbsoluteExpiresAt > now
                               && s.User.Status == StudentStatus.Active, cancellationToken);

            _cache.Set(key, isActive, CacheDuration);
            return isActive;
        }

        public void Invalidate(Guid sessionId) => _cache.Remove(BuildKey(sessionId));

        private static string BuildKey(Guid sessionId) => $"session-active:{sessionId:N}";
    }
}