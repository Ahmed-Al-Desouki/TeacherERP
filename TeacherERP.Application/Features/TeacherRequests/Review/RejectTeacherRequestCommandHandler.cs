// جديد — TeacherERP.Application/Features/TeacherRequests/Review/RejectTeacherRequestCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    public class RejectTeacherRequestCommandHandler : IRequestHandler<RejectTeacherRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly ITeacherRequestReviewService _reviewService;

        public RejectTeacherRequestCommandHandler(
            ICurrentUserService currentUser,
            ITeacherAccessRequestRepository requestRepository,
            ITeacherRequestReviewService reviewService)
        {
            _currentUser = currentUser;
            _requestRepository = requestRepository;
            _reviewService = reviewService;
        }

        public async Task<Result<bool>> Handle(RejectTeacherRequestCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } reviewerId)
            {
                return Result<bool>.Failure(TeacherRequestErrors.ReviewerNotAllowed);
            }

            var found = await _requestRepository.GetByTokenHashAsync(TeacherRequestToken.Hash(request.Token), cancellationToken);
            if (found is null)
            {
                return Result<bool>.Failure(TeacherRequestErrors.NotFound);
            }

            return await _reviewService.RejectAsync(found, reviewerId, ReviewerKind.Teacher, request.Reason, cancellationToken);
        }
    }
}
