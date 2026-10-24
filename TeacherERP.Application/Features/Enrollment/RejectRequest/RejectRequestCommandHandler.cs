// موجود (تعديل) — TeacherERP.Application/Features/Enrollment/RejectRequest/RejectRequestCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.RejectRequest
{
    public class RejectRequestCommandHandler : IRequestHandler<RejectRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IEnrollmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RejectRequestCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IEnrollmentRepository repository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(RejectRequestCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            var enrollmentRequest = await _repository.GetTrackedRequestAsync(request.RequestId, cancellationToken);
            if (enrollmentRequest is null)
            {
                return Result<bool>.Failure(EnrollmentErrors.RequestNotFound);
            }

            if (enrollmentRequest.Status != EnrollmentRequestStatus.Pending)
            {
                return Result<bool>.Failure(EnrollmentErrors.NotPending);
            }

            enrollmentRequest.Reject(userId, _timeProvider.GetUtcNow().UtcDateTime, request.Note);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}