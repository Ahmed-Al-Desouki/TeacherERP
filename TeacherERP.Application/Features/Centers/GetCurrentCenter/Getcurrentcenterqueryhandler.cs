using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.GetCurrentCenter
{
    public class GetCurrentCenterQueryHandler : IRequestHandler<GetCurrentCenterQuery, Result<CenterDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;

        public GetCurrentCenterQueryHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
        }

        public async Task<Result<CenterDto>> Handle(GetCurrentCenterQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<CenterDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<CenterDto>.Failure(TenantErrors.Required);
            }

            var membership = await _tenantRepository.GetMembershipAsync(userId, tenant.TenantId, cancellationToken);
            if (membership is null)
            {
                return Result<CenterDto>.Failure(TenantErrors.Forbidden);
            }

            return Result<CenterDto>.Success(CenterDto.From(membership));
        }
    }
}