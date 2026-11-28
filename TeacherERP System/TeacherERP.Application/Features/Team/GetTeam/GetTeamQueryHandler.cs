// جديد — TeacherERP.Application/Features/Team/GetTeam/GetTeamQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.GetTeam
{
    public class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, Result<List<TeamMemberDto>>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITeamRepository _teamRepository;

        public GetTeamQueryHandler(ICurrentTenantAccessor currentTenant, ITeamRepository teamRepository)
        {
            _currentTenant = currentTenant;
            _teamRepository = teamRepository;
        }

        public async Task<Result<List<TeamMemberDto>>> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<List<TeamMemberDto>>.Failure(TenantErrors.Required);
            }

            var items = await _teamRepository.GetAssistantsAsync(cancellationToken);

            return Result<List<TeamMemberDto>>.Success(items.Select(TeamMemberDto.From).ToList());
        }
    }
}