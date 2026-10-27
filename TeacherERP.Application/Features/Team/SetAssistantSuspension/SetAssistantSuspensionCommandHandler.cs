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
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetAssistantSuspensionCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork)
        {
            _currentTenant = currentTenant;
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SetAssistantSuspensionCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<bool>.Failure(TenantErrors.Required);
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
                changed = true;
            }
            else if (!request.IsSuspended && member.Status == MembershipStatus.Suspended)
            {
                member.Activate();
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