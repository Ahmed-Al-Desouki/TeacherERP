using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.GetMyCenters
{
    public class GetMyCentersQueryHandler : IRequestHandler<GetMyCentersQuery, Result<List<CenterDto>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITenantRepository _tenantRepository;

        public GetMyCentersQueryHandler(ICurrentUserService currentUser, ITenantRepository tenantRepository)
        {
            _currentUser = currentUser;
            _tenantRepository = tenantRepository;
        }

        public async Task<Result<List<CenterDto>>> Handle(GetMyCentersQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<CenterDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var memberships = await _tenantRepository.GetMembershipsAsync(userId, cancellationToken);

            return Result<List<CenterDto>>.Success(memberships.Select(CenterDto.From).ToList());
        }
    }
}