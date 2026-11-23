using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Sessions
{
    // تنظيف دوري: حذف الجلسات التي انتهت أو أُلغيت منذ أكثر من مدة الاحتفاظ
    public class SessionCleanupJob : ISessionCleanupJob
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<SessionCleanupJob> _logger;

        public SessionCleanupJob(
            ISessionRepository sessionRepository,
            TimeProvider timeProvider,
            ILogger<SessionCleanupJob> logger)
        {
            _sessionRepository = sessionRepository;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var cutoff = _timeProvider.GetUtcNow().UtcDateTime - SessionPolicy.EndedSessionRetention;

            var deleted = await _sessionRepository.DeleteEndedBeforeAsync(cutoff, CancellationToken.None);

            _logger.LogInformation(
                "Session cleanup removed {DeletedCount} sessions that ended before {Cutoff:u}.",
                deleted, cutoff);
        }
    }
}