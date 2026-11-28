// جديد — TeacherERP.Application/Features/Enrollment/CancelRequest/CancelRequestCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.CancelRequest
{
    public class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IEnrollmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CancelRequestCommandHandler(
            ICurrentUserService currentUser,
            IEnrollmentRepository repository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(CancelRequestCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } studentId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            // طلبي أنا فقط وما زال معلّقاً (غير الموجود وطلب غيري نفس الرد)
            var enrollmentRequest = await _repository.GetOwnPendingRequestAsync(request.RequestId, studentId, cancellationToken);
            if (enrollmentRequest is null)
            {
                return Result<bool>.Failure(EnrollmentErrors.RequestNotFound);
            }

            enrollmentRequest.Cancel(_timeProvider.GetUtcNow().UtcDateTime);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}