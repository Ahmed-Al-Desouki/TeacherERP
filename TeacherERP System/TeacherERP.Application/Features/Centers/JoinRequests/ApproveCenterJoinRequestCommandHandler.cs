using System.Data;
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class ApproveCenterJoinRequestCommandHandler : IRequestHandler<ApproveCenterJoinRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICenterMembershipRepository _requests;
        private readonly ITenantRepository _tenants;
        private readonly ICenterRepository _centers;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public ApproveCenterJoinRequestCommandHandler(
            ICurrentUserService currentUser, ICenterMembershipRepository requests, ITenantRepository tenants,
            ICenterRepository centers, IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _requests = requests;
            _tenants = tenants;
            _centers = centers;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(ApproveCenterJoinRequestCommand command, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } ownerId)
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

            var request = await _requests.GetRequestForCenterOwnerAsync(command.RequestId, ownerId, cancellationToken);
            if (request is null)
                return Result<bool>.Failure(CenterErrors.JoinRequestNotFound);
            if (request.Status != CenterJoinRequestStatus.Pending)
                return Result<bool>.Failure(CenterErrors.JoinRequestNotPending);

            var tenant = await _tenants.GetByIdAsync(request.TenantId, cancellationToken);
            if (tenant is not { Status: TenantStatus.Active })
                return Result<bool>.Failure(CenterErrors.TeacherSpaceNotFound);

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var link = await _centers.GetCenterTeacherAsync(request.CenterId, request.TenantId, cancellationToken);
                if (link is null)
                    _centers.AddCenterTeacher(CenterTeacher.CreateActive(request.CenterId, request.TenantId, CenterTeacherOrigin.Request));
                else if (link.Status != CenterTeacherStatus.Active)
                    link.Activate(CenterTeacherOrigin.Request);

                request.Approve(ownerId, _timeProvider.GetUtcNow().UtcDateTime);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<bool>.Success(true);
            }, IsolationLevel.ReadCommitted, cancellationToken);
        }
    }
}
