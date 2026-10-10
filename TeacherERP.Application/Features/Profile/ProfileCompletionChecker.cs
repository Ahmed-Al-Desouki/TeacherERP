using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Features.Profile
{
    public class ProfileCompletionChecker : IProfileCompletionChecker
    {
        // نتيجة "مكتمل" فقط تُخزَّن (لا يعود البروفايل ناقصاً بعد اكتماله)، ونتيجة "ناقص" لا تُخزَّن أبداً
        // فلا يتأخر فتح الباب بعد إكمال الطالب لبياناته
        private static readonly TimeSpan CompleteCacheDuration = TimeSpan.FromMinutes(10);

        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;

        public ProfileCompletionChecker(IUserRepository userRepository, IMemoryCache cache)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public async Task<bool> IsCompleteAsync(Guid userId, CancellationToken cancellationToken)
        {
            var key = $"profile-complete:{userId:N}";

            if (_cache.TryGetValue(key, out bool _))
            {
                return true;
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            var isComplete = user?.IsProfileComplete == true;

            if (isComplete)
            {
                _cache.Set(key, true, CompleteCacheDuration);
            }

            return isComplete;
        }
    }
}