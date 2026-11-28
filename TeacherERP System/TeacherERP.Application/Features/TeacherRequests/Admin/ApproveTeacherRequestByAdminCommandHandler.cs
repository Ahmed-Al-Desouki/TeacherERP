// جديد — TeacherERP.Application/Features/TeacherRequests/Admin/ApproveTeacherRequestByAdminCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Administration;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    public class ApproveTeacherRequestByAdminCommandHandler : IRequestHandler<ApproveTeacherRequestByAdminCommand, Result<CreatedTeacherDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly ITeacherRequestReviewService _reviewService;

        public ApproveTeacherRequestByAdminCommandHandler(
            ICurrentUserService currentUser,
            ITeacherAccessRequestRepository requestRepository,
            ITeacherRequestReviewService reviewService)
        {
            _currentUser = currentUser;
            _requestRepository = requestRepository;
            _reviewService = reviewService;
        }

        public async Task<Result<CreatedTeacherDto>> Handle(ApproveTeacherRequestByAdminCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } reviewerId)
            {
                return Result<CreatedTeacherDto>.Failure(TeacherRequestErrors.ReviewerNotAllowed);
            }

            var found = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
            if (found is null)
            {
                return Result<CreatedTeacherDto>.Failure(TeacherRequestErrors.NotFound);
            }

            return await _reviewService.ApproveAsync(found, reviewerId, ReviewerKind.Administrator, cancellationToken);
        }
    }
}
