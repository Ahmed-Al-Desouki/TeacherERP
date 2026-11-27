using Microsoft.Extensions.Logging;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Centers
{
    public class CenterMembershipCleanupJob : ICenterMembershipCleanupJob
    {
        private readonly ICenterMembershipRepository _repository;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<CenterMembershipCleanupJob> _logger;

        public CenterMembershipCleanupJob(
            ICenterMembershipRepository repository,
            TimeProvider timeProvider,
            ILogger<CenterMembershipCleanupJob> logger)
        {
            _repository = repository;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var cutoff = _timeProvider.GetUtcNow().UtcDateTime - CenterMembershipPolicy.ClosedDataRetention;
            const int batchSize = 250;
            var deleted = await _repository.PurgeClosedDataBeforeAsync(cutoff, batchSize, CancellationToken.None);
            _logger.LogInformation("Center membership cleanup removed {DeletedCount} closed requests and invitations in batches capped at {BatchSize} before {Cutoff:u}.", deleted, batchSize, cutoff);
        }
    }
}
