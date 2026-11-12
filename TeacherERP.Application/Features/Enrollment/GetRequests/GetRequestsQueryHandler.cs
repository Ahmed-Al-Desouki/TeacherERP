// موجود (تعديل) — TeacherERP.Application/Features/Enrollment/GetRequests/GetRequestsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.GetRequests
{
    public class GetRequestsQueryHandler : IRequestHandler<GetRequestsQuery, Result<PagedResult<EnrollmentRequestListDto>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IEnrollmentRepository _repository;

        public GetRequestsQueryHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IEnrollmentRepository repository)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _repository = repository;
        }

        public async Task<Result<PagedResult<EnrollmentRequestListDto>>> Handle(
            GetRequestsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { })
            {
                return Result<PagedResult<EnrollmentRequestListDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<EnrollmentRequestListDto>>.Failure(TenantErrors.Required);
            }

            var page = await _repository.GetRequestsPageAsync(request.Status, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<EnrollmentRequestListDto>>.Success(page.Map(EnrollmentRequestListDto.From));
        }
    }
}
