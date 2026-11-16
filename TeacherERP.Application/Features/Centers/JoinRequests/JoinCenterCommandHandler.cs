using System.Data;
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class JoinCenterCommandHandler : IRequestHandler<JoinCenterCommand, Result<CenterJoinRequestListItem>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITenantRepository _tenants;
        private readonly ICenterRepository _centers;
        private readonly ICenterMembershipRepository _requests;
        private readonly ICenterJoinRequestNotifier _notifier;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public JoinCenterCommandHandler(
            ICurrentUserService currentUser, ITenantRepository tenants, ICenterRepository centers,
            ICenterMembershipRepository requests, ICenterJoinRequestNotifier notifier,
            IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _tenants = tenants;
            _centers = centers;
            _requests = requests;
            _notifier = notifier;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<CenterJoinRequestListItem>> Handle(JoinCenterCommand command, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
                return Result<CenterJoinRequestListItem>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

            var teacherTenant = await _tenants.GetByOwnerIdAsync(userId, cancellationToken);
            if (teacherTenant is not { Status: TenantStatus.Active })
                return Result<CenterJoinRequestListItem>.Failure(CenterErrors.TeacherSpaceNotFound);

            var center = await _centers.GetByIdAsync(command.CenterId, cancellationToken);
            if (center is null)
                return Result<CenterJoinRequestListItem>.Failure(CenterErrors.NotFound);
            if (await _centers.IsActiveTeacherAtAsync(center.Id, teacherTenant.Id, cancellationToken))
                return Result<CenterJoinRequestListItem>.Failure(CenterErrors.AlreadyMember);

            var createdAt = _timeProvider.GetUtcNow().UtcDateTime;
            var created = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (await _requests.GetPendingRequestAsync(center.Id, teacherTenant.Id, cancellationToken) is not null)
                    return Result<CenterJoinRequest>.Failure(CenterErrors.JoinRequestPending);

                if (await _requests.CountPendingRequestsAsync(teacherTenant.Id, cancellationToken)
                    >= CenterMembershipPolicy.MaxPendingRequestsPerTeacher)
                    return Result<CenterJoinRequest>.Failure(CenterErrors.JoinRequestLimitReached);

                var request = CenterJoinRequest.Create(center.Id, teacherTenant.Id, createdAt);
                _requests.Add(request);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<CenterJoinRequest>.Success(request);
            }, IsolationLevel.Serializable, cancellationToken);

            if (created.IsFailure)
                return Result<CenterJoinRequestListItem>.Failure(created.Error);

            // حفظ الطلب أولاً يضمن ظهوره لصاحب السنتر حتى لو تعذر وضع البريد في الطابور.
            await _notifier.NotifyOwnerAsync(center.OwnerUserId, teacherTenant.Id, center.Name, false, cancellationToken);
            return Result<CenterJoinRequestListItem>.Success(new CenterJoinRequestListItem(
                created.Value!.Id, center.Id, center.Name, teacherTenant.Id, teacherTenant.Name,
                null, teacherTenant.Subject, created.Value.Status, created.Value.CreatedAt,
                created.Value.LastNotifiedAtUtc, null, null));
        }
    }
}
