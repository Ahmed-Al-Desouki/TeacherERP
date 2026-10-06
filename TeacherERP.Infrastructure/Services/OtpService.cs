using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<string> GenerateAndSaveOtpAsync(Guid userId, CancellationToken cancellationToken)
        {
            // توليد كود عشوائي من 6 أرقام
            var otpCode = new Random().Next(100000, 999999).ToString();

            // حفظ الـ OTP في الذاكرة (Cache) لمدة 10 دقائق باستخدام الـ UserId كمفتاح
            var cacheKey = $"OTP_{userId}";
            _cache.Set(cacheKey, otpCode, TimeSpan.FromMinutes(10));

            return Task.FromResult(otpCode);
        }

        public Task<bool> VerifyOtpAsync(Guid userId, string otpCode, CancellationToken cancellationToken)
        {
            var cacheKey = $"OTP_{userId}";

            if (_cache.TryGetValue(cacheKey, out string? storedOtp) && storedOtp == otpCode)
            {
                // مسح الكود بعد استخدامه لضمان عدم إمكانية إعادة استخدامه
                _cache.Remove(cacheKey);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}