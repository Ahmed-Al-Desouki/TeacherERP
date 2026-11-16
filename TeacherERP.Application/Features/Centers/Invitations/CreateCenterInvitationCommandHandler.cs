using MediatR;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.Invitations
{
    public class CreateCenterInvitationCommandHandler : IRequestHandler<CreateCenterInvitationCommand, Result<CenterInvitationDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICenterRepository _centers;
        private readonly ICenterMembershipRepository _invitations;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CreateCenterInvitationCommandHandler(
            ICurrentUserService currentUser, ICenterRepository centers, ICenterMembershipRepository invitations,
            IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _centers = centers;
            _invitations = invitations;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<CenterInvitationDto>> Handle(CreateCenterInvitationCommand command, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } ownerId)
                return Result<CenterInvitationDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

            var center = await _centers.GetOwnedByIdAsync(command.CenterId, ownerId, cancellationToken);
            if (center is null)
                return Result<CenterInvitationDto>.Failure(CenterErrors.NotFound);

            var token = CenterInvitationToken.Generate();
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var invitation = CenterInvitation.Create(
                center.Id, ownerId, CenterInvitationToken.Hash(token), now + CenterMembershipPolicy.InvitationLifetime);
            _invitations.Add(invitation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // الرمز الخام يُعرض مرة واحدة فقط؛ قاعدة البيانات تحتفظ ببصمته.
            return Result<CenterInvitationDto>.Success(new CenterInvitationDto(token, invitation.ExpiresAtUtc));
        }
    }
}
