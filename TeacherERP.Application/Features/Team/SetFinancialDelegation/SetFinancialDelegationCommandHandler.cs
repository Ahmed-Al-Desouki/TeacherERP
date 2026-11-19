// جديد — TeacherERP.Application/Features/Team/SetFinancialDelegation/SetFinancialDelegationCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.SetFinancialDelegation
{
    public class SetFinancialDelegationCommandHandler : IRequestHandler<SetFinancialDelegationCommand, Result<bool>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ICurrentUserService _currentUser;
        private readonly ITeamRepository _teamRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetFinancialDelegationCommandHandler(
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

        public async Task<Result<bool>> Handle(SetFinancialDelegationCommand request, CancellationToken cancellationToken)
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

            if (member.HasFinancialDelegation == request.Granted)
            {
                return Result<bool>.Success(true);
            }

            // الـ Domain يرفض غير المساعد، لكن الاستعلام فوق يضمن إنه مساعد أصلاً
            var result = member.SetFinancialDelegation(request.Granted);
            if (result.IsFailure)
            {
                return result;
            }

            var action = request.Granted
                ? AuditAction.FinancialDelegationGranted
                : AuditAction.FinancialDelegationRevoked;
            var reason = request.Granted
                ? "Financial delegation granted to assistant."
                : "Financial delegation revoked from assistant.";
            _auditRepository.Add(AuditLog.Create(tenant.TenantId, actorId, action, member.UserId, reason));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
