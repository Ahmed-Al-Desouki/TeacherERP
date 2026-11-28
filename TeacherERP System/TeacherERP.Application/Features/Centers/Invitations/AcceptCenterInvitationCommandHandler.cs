using System.Data;
using MediatR;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.Invitations
{
    public class AcceptCenterInvitationCommandHandler : IRequestHandler<AcceptCenterInvitationCommand, Result<CenterDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITenantRepository _tenants;
        private readonly ICenterMembershipRepository _invitations;
        private readonly ICenterRepository _centers;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public AcceptCenterInvitationCommandHandler(
            ICurrentUserService currentUser, ITenantRepository tenants, ICenterMembershipRepository invitations,
            ICenterRepository centers, IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _tenants = tenants;
            _invitations = invitations;
            _centers = centers;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<CenterDto>> Handle(AcceptCenterInvitationCommand command, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
                return Result<CenterDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

            var tenant = await _tenants.GetByOwnerIdAsync(userId, cancellationToken);
            if (tenant is not { Status: TenantStatus.Active })
                return Result<CenterDto>.Failure(CenterErrors.TeacherSpaceNotFound);

            var invitation = await _invitations.GetInvitationByHashAsync(CenterInvitationToken.Hash(command.Token), cancellationToken);
            if (invitation is null)
                return Result<CenterDto>.Failure(CenterErrors.InvitationNotFound);
            if (invitation.Status != CenterInvitationStatus.Pending)
                return Result<CenterDto>.Failure(CenterErrors.InvitationAlreadyUsed);

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            if (invitation.IsExpired(now))
            {
                invitation.Expire();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<CenterDto>.Failure(CenterErrors.InvitationExpired);
            }

            var center = await _centers.GetByIdAsync(invitation.CenterId, cancellationToken);
            if (center is null)
                return Result<CenterDto>.Failure(CenterErrors.NotFound);
            if (await _centers.IsActiveTeacherAtAsync(center.Id, tenant.Id, cancellationToken))
                return Result<CenterDto>.Failure(CenterErrors.AlreadyMember);

            var accepted = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var link = await _centers.GetCenterTeacherAsync(center.Id, tenant.Id, cancellationToken);
                if (link is null)
                    _centers.AddCenterTeacher(CenterTeacher.CreateActive(center.Id, tenant.Id, CenterTeacherOrigin.Invitation));
                else if (link.Status != CenterTeacherStatus.Active)
                    link.Activate(CenterTeacherOrigin.Invitation);

                var pending = await _invitations.GetPendingRequestAsync(center.Id, tenant.Id, cancellationToken);
                pending?.Approve(invitation.CreatedByUserId, now);
                invitation.Accept(tenant.Id, now);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }, IsolationLevel.ReadCommitted, cancellationToken);

            return accepted
                ? Result<CenterDto>.Success(CenterDto.From(center, userId))
                : Result<CenterDto>.Failure(CenterErrors.InvitationNotFound);
        }
    }
}
