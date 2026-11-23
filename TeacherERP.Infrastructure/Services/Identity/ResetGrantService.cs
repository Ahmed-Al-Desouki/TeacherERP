using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Infrastructure.Services
{
    public class ResetGrantService : IResetGrantService
    {
        private static readonly TimeSpan GrantLifetime = TimeSpan.FromMinutes(10);
        private readonly IMemoryCache _cache;

        public ResetGrantService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public ResetGrant Issue(string email)
        {
            // 32 بايت عشوائية آمنة = 64 حرف Hex (آمنة للاستخدام في الـ JSON والـ URL)
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            _cache.Set(BuildKey(email), token, GrantLifetime);

            return new ResetGrant(token, (int)GrantLifetime.TotalSeconds);
        }

        public bool IsValid(string email, string token)
        {
            if (string.IsNullOrEmpty(token) || !_cache.TryGetValue(BuildKey(email), out string? stored) || stored is null)
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(stored), Encoding.UTF8.GetBytes(token));
        }

        public void Revoke(string email) => _cache.Remove(BuildKey(email));

        private static string BuildKey(string email) => $"reset-grant:{email.Trim().ToLowerInvariant()}";
    }
}