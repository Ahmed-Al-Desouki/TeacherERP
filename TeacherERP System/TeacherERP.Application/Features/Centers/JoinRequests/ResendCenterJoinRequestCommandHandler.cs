using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class ResendCenterJoinRequestCommandHandler : IRequestHandler<ResendCenterJoinRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITenantRepository _tenants;
        private readonly ICenterMembershipRepository _requests;
        private readonly ICenterRepository _centers;
        private readonly ICenterJoinRequestNotifier _notifier;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public ResendCenterJoinRequestCommandHandler(
            ICurrentUserService currentUser, ITenantRepository tenants, ICenterMembershipRepository requests,
            ICenterRepository centers, ICenterJoinRequestNotifier notifier, IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _tenants = tenants;
            _requests = requests;
            _centers = centers;
            _notifier = notifier;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(ResendCenterJoinRequestCommand command, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

            var tenant = await _tenants.GetByOwnerIdAsync(userId, cancellationToken);
            if (tenant is null)
                return Result<bool>.Failure(CenterErrors.TeacherSpaceNotFound);

            var request = await _requests.GetRequestForTeacherAsync(command.RequestId, tenant.Id, cancellationToken);
            if (request is null)
                return Result<bool>.Failure(CenterErrors.JoinRequestNotFound);
            if (request.Status != CenterJoinRequestStatus.Pending)
                return Result<bool>.Failure(CenterErrors.JoinRequestNotPending);

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            if (!request.CanResendAt(now, CenterMembershipPolicy.RequestReminderInterval))
                return Result<bool>.Failure(CenterErrors.JoinRequestResendTooSoon);

            var center = await _centers.GetByIdAsync(request.CenterId, cancellationToken);
            if (center is null)
                return Result<bool>.Failure(CenterErrors.NotFound);

            var queued = await _notifier.NotifyOwnerAsync(center.OwnerUserId, tenant.Id, center.Name, true, cancellationToken);
            if (!queued)
                return Result<bool>.Failure(new Error("Center.NotificationUnavailable", "The reminder could not be queued. Please try again later."));

            request.MarkOwnerNotified(now);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
