using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);

        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<Result<OtpIssueResult>> GenerateAsync(OtpPurpose purpose, string identifier, CancellationToken cancellationToken)
        {
            var key = BuildKey(purpose, identifier);
            var now = DateTime.UtcNow;

            if (_cache.TryGetValue(key, out OtpEntry? existing) && existing is not null)
            {
                lock (existing)
                {
                    // بعد استنفاد المحاولات لا نسمح بإصدار كود جديد حتى انتهاء مدة القفل
                    if (existing.IsLocked)
                    {
                        return Task.FromResult(Result<OtpIssueResult>.Failure(
                            new Error("Otp.Locked", "Too many failed attempts. Please try again later.")));
                    }

                    if (existing.ResendAvailableAtUtc > now)
                    {
                        var wait = (int)Math.Ceiling((existing.ResendAvailableAtUtc - now).TotalSeconds);
                        return Task.FromResult(Result<OtpIssueResult>.Failure(
                            new Error("Otp.ResendTooSoon", $"Please wait {wait} seconds before requesting a new code.")));
                    }
                }
            }

            // كود عشوائي آمن تشفيرياً من 6 أرقام (يسمح بالأصفار في البداية)
            var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

            _cache.Set(key, new OtpEntry(code, now + ResendCooldown), CodeLifetime);

            return Task.FromResult(Result<OtpIssueResult>.Success(
                new OtpIssueResult(code, (int)CodeLifetime.TotalSeconds, (int)ResendCooldown.TotalSeconds)));
        }

        public Task<Result<bool>> VerifyAsync(OtpPurpose purpose, string identifier, string code, CancellationToken cancellationToken)
        {
            var key = BuildKey(purpose, identifier);

            if (!_cache.TryGetValue(key, out OtpEntry? entry) || entry is null)
            {
                return Task.FromResult(Result<bool>.Failure(
                    new Error("Otp.Expired", "The code has expired or was not requested. Please request a new one.")));
            }

            lock (entry)
            {
                if (entry.IsLocked)
                {
                    return Task.FromResult(Result<bool>.Failure(
                        new Error("Otp.TooManyAttempts", "Too many failed attempts. Please try again later.")));
                }

                if (!IsMatch(entry.Code, code))
                {
                    entry.FailedAttempts++;

                    if (entry.FailedAttempts >= MaxFailedAttempts)
                    {
                        // نبقي الكيان مقفولاً (ولا نحذفه) حتى لا يستطيع المهاجم طلب كود جديد فوراً
                        entry.IsLocked = true;
                        _cache.Set(key, entry, CodeLifetime);

                        return Task.FromResult(Result<bool>.Failure(
                            new Error("Otp.TooManyAttempts", "Too many failed attempts. Please try again later.")));
                    }

                    var remaining = MaxFailedAttempts - entry.FailedAttempts;
                    return Task.FromResult(Result<bool>.Failure(
                        new Error("Otp.Invalid", $"The verification code is invalid. {remaining} attempt(s) remaining.")));
                }

                // الكود صحيح: يُحذف فوراً لمنع إعادة استخدامه
                _cache.Remove(key);
                return Task.FromResult(Result<bool>.Success(true));
            }
        }

        private static string BuildKey(OtpPurpose purpose, string identifier)
            => $"otp:{purpose}:{identifier.Trim().ToLowerInvariant()}";

        // مقارنة بزمن ثابت لمنع هجمات قياس الزمن (Timing Attacks)
        private static bool IsMatch(string expected, string actual)
            => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(actual));

        private sealed class OtpEntry
        {
            public OtpEntry(string code, DateTime resendAvailableAtUtc)
            {
                Code = code;
                ResendAvailableAtUtc = resendAvailableAtUtc;
            }

            public string Code { get; }
            public DateTime ResendAvailableAtUtc { get; }
            public int FailedAttempts { get; set; }
            public bool IsLocked { get; set; }
        }
    }
}