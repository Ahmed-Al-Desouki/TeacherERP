// موجود (تعديل) — TeacherERP.Application/Features/Enrollment/GetRequests/GetRequestsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.GetRequests
{
    public class GetRequestsQueryHandler : IRequestHandler<GetRequestsQuery, Result<List<EnrollmentRequestListDto>>>
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

        public async Task<Result<List<EnrollmentRequestListDto>>> Handle(GetRequestsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<EnrollmentRequestListDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<List<EnrollmentRequestListDto>>.Failure(TenantErrors.Required);
            }

            var items = await _repository.GetRequestsAsync(request.Status, cancellationToken);

            return Result<List<EnrollmentRequestListDto>>.Success(items.Select(EnrollmentRequestListDto.From).ToList());
        }
    }
}