// جديد — TeacherERP.Application/Features/TeacherRequests/Admin/RejectTeacherRequestByAdminCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    public class RejectTeacherRequestByAdminCommandHandler : IRequestHandler<RejectTeacherRequestByAdminCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly ITeacherRequestReviewService _reviewService;

        public RejectTeacherRequestByAdminCommandHandler(
            ICurrentUserService currentUser,
            ITeacherAccessRequestRepository requestRepository,
            ITeacherRequestReviewService reviewService)
        {
            _currentUser = currentUser;
            _requestRepository = requestRepository;
            _reviewService = reviewService;
        }

        public async Task<Result<bool>> Handle(RejectTeacherRequestByAdminCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } reviewerId)
            {
                return Result<bool>.Failure(TeacherRequestErrors.ReviewerNotAllowed);
            }

            var found = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
            if (found is null)
            {
                return Result<bool>.Failure(TeacherRequestErrors.NotFound);
            }

            return await _reviewService.RejectAsync(found, reviewerId, ReviewerKind.Administrator, request.Reason, cancellationToken);
        }
    }
}
