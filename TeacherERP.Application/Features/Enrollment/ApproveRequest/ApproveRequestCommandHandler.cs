// جديد — TeacherERP.Application/Features/Enrollment/ApproveRequest/ApproveRequestCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.ApproveRequest
{
    public class ApproveRequestCommandHandler : IRequestHandler<ApproveRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IEnrollmentRepository _repository;
        private readonly IEnrollmentService _enrollmentService;

        public ApproveRequestCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IEnrollmentRepository repository,
            IEnrollmentService enrollmentService)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _repository = repository;
            _enrollmentService = enrollmentService;
        }

        public async Task<Result<bool>> Handle(ApproveRequestCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن الطلب يخص المدرس الحالي فقط
            var enrollmentRequest = await _repository.GetTrackedRequestAsync(request.RequestId, cancellationToken);
            if (enrollmentRequest is null)
            {
                return Result<bool>.Failure(EnrollmentErrors.RequestNotFound);
            }

            // المساعد يقرر في مجموعاته المخصصة له فقط (غيرها نفس رد غير الموجود)
            if (tenant.Role != TenantRole.Owner
                && !await _repository.IsAssistantAssignedAsync(enrollmentRequest.GroupId, userId, cancellationToken))
            {
                return Result<bool>.Failure(EnrollmentErrors.RequestNotFound);
            }

            if (enrollmentRequest.Status != EnrollmentRequestStatus.Pending)
            {
                return Result<bool>.Failure(EnrollmentErrors.NotPending);
            }

            return await _enrollmentService.ApproveAsync(enrollmentRequest, userId, cancellationToken);
        }
    }
}