using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class RejectCenterJoinRequestCommandHandler : IRequestHandler<RejectCenterJoinRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICenterMembershipRepository _requests;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RejectCenterJoinRequestCommandHandler(
            ICurrentUserService currentUser, ICenterMembershipRepository requests, IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _requests = requests;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(RejectCenterJoinRequestCommand command, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } ownerId)
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

            var request = await _requests.GetRequestForCenterOwnerAsync(command.RequestId, ownerId, cancellationToken);
            if (request is null)
                return Result<bool>.Failure(CenterErrors.JoinRequestNotFound);
            if (request.Status != CenterJoinRequestStatus.Pending)
                return Result<bool>.Failure(CenterErrors.JoinRequestNotPending);

            request.Reject(ownerId, _timeProvider.GetUtcNow().UtcDateTime, command.Reason);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
