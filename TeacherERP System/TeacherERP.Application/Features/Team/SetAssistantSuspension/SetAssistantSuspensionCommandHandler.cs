// جديد — TeacherERP.Application/Features/Team/SetAssistantSuspension/SetAssistantSuspensionCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.SetAssistantSuspension
{
    public class SetAssistantSuspensionCommandHandler : IRequestHandler<SetAssistantSuspensionCommand, Result<bool>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ICurrentUserService _currentUser;
        private readonly ITeamRepository _teamRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetAssistantSuspensionCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ICurrentUserService currentUser,
            ITeamRepository teamRepository,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork)
        {
            _currentTenant = currentTenant;
            _currentUser = currentUser;
            _teamRepository = teamRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SetAssistantSuspensionCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            if (_currentUser.UserId is not { } actorId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var member = await _teamRepository.GetTrackedAssistantAsync(request.UserId, cancellationToken);
            if (member is null)
            {
                return Result<bool>.Failure(TeamErrors.NotFound);
            }

            // التكرار ينجح بدون تغيير (Idempotent). الانتقال بين Active وSuspended فقط
            var changed = false;

            if (request.IsSuspended && member.Status == MembershipStatus.Active)
            {
                member.Suspend();
                _auditRepository.Add(AuditLog.Create(tenant.TenantId, actorId, AuditAction.AssistantSuspended, member.UserId, null));
                changed = true;
            }
            else if (!request.IsSuspended && member.Status == MembershipStatus.Suspended)
            {
                member.Activate();
                _auditRepository.Add(AuditLog.Create(tenant.TenantId, actorId, AuditAction.AssistantUnsuspended, member.UserId, null));
                changed = true;
            }

            if (changed)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<bool>.Success(true);
        }
    }
}
