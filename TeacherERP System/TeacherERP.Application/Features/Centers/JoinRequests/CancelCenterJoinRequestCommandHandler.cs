using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class CancelCenterJoinRequestCommandHandler : IRequestHandler<CancelCenterJoinRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITenantRepository _tenants;
        private readonly ICenterMembershipRepository _requests;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CancelCenterJoinRequestCommandHandler(
            ICurrentUserService currentUser, ITenantRepository tenants, ICenterMembershipRepository requests,
            IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _tenants = tenants;
            _requests = requests;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(CancelCenterJoinRequestCommand command, CancellationToken cancellationToken)
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

            request.Cancel(_timeProvider.GetUtcNow().UtcDateTime);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
