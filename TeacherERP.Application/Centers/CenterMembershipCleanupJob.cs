using Microsoft.Extensions.Logging;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Centers
{
    public class CenterMembershipCleanupJob : ICenterMembershipCleanupJob
    {
        private readonly ICenterMembershipRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<CenterMembershipCleanupJob> _logger;

        public CenterMembershipCleanupJob(
            ICenterMembershipRepository repository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider,
            ILogger<CenterMembershipCleanupJob> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var cutoff = _timeProvider.GetUtcNow().UtcDateTime - CenterMembershipPolicy.ClosedDataRetention;
            var deleted = await _repository.PurgeClosedDataBeforeAsync(cutoff, CancellationToken.None);
            if (deleted > 0)
            {
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            }

            _logger.LogInformation("Center membership cleanup removed {DeletedCount} closed requests and invitations before {Cutoff:u}.", deleted, cutoff);
        }
    }
}
