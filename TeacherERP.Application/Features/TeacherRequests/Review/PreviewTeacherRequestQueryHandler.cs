// جديد — TeacherERP.Application/Features/TeacherRequests/Review/PreviewTeacherRequestQueryHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    public class PreviewTeacherRequestQueryHandler : IRequestHandler<PreviewTeacherRequestQuery, Result<TeacherRequestPreviewDto>>
    {
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly TimeProvider _timeProvider;

        public PreviewTeacherRequestQueryHandler(ITeacherAccessRequestRepository requestRepository, TimeProvider timeProvider)
        {
            _requestRepository = requestRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<TeacherRequestPreviewDto>> Handle(PreviewTeacherRequestQuery request, CancellationToken cancellationToken)
        {
            var found = await _requestRepository.GetByTokenHashAsync(TeacherRequestToken.Hash(request.Token), cancellationToken);
            if (found is null)
            {
                return Result<TeacherRequestPreviewDto>.Failure(TeacherRequestErrors.NotFound);
            }

            var error = TeacherRequestRules.CheckReviewable(found, _timeProvider.GetUtcNow().UtcDateTime);
            if (error is not null)
            {
                return Result<TeacherRequestPreviewDto>.Failure(error);
            }

            return Result<TeacherRequestPreviewDto>.Success(
                new TeacherRequestPreviewDto(found.FullName, found.Email, found.Subject, found.Note, found.ExpiresAtUtc));
        }
    }
}
