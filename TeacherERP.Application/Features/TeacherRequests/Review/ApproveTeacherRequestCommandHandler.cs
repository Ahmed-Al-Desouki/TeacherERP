// جديد — TeacherERP.Application/Features/TeacherRequests/Review/ApproveTeacherRequestCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Administration;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    public class ApproveTeacherRequestCommandHandler : IRequestHandler<ApproveTeacherRequestCommand, Result<CreatedTeacherDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly ITeacherRequestReviewService _reviewService;

        public ApproveTeacherRequestCommandHandler(
            ICurrentUserService currentUser,
            ITeacherAccessRequestRepository requestRepository,
            ITeacherRequestReviewService reviewService)
        {
            _currentUser = currentUser;
            _requestRepository = requestRepository;
            _reviewService = reviewService;
        }

        public async Task<Result<CreatedTeacherDto>> Handle(ApproveTeacherRequestCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } reviewerId)
            {
                return Result<CreatedTeacherDto>.Failure(TeacherRequestErrors.ReviewerNotAllowed);
            }

            var found = await _requestRepository.GetByTokenHashAsync(TeacherRequestToken.Hash(request.Token), cancellationToken);
            if (found is null)
            {
                return Result<CreatedTeacherDto>.Failure(TeacherRequestErrors.NotFound);
            }

            return await _reviewService.ApproveAsync(found, reviewerId, ReviewerKind.Teacher, cancellationToken);
        }
    }
}
