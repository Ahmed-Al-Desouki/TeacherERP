// جديد — TeacherERP.Application/Features/Team/SetFinancialDelegation/SetFinancialDelegationCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.SetFinancialDelegation
{
    public class SetFinancialDelegationCommandHandler : IRequestHandler<SetFinancialDelegationCommand, Result<bool>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetFinancialDelegationCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork)
        {
            _currentTenant = currentTenant;
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SetFinancialDelegationCommand request, CancellationToken cancellationToken)
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

            // الـ Domain يرفض غير المساعد، لكن الاستعلام فوق يضمن إنه مساعد أصلاً
            var result = member.SetFinancialDelegation(request.Granted);
            if (result.IsFailure)
            {
                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}