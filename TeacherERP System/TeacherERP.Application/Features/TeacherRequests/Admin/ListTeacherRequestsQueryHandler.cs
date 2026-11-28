// جديد — TeacherERP.Application/Features/TeacherRequests/Admin/ListTeacherRequestsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    public class ListTeacherRequestsQueryHandler : IRequestHandler<ListTeacherRequestsQuery, Result<PagedResult<TeacherAccessRequestListItem>>>
    {
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly TimeProvider _timeProvider;

        public ListTeacherRequestsQueryHandler(ITeacherAccessRequestRepository requestRepository, TimeProvider timeProvider)
        {
            _requestRepository = requestRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<PagedResult<TeacherAccessRequestListItem>>> Handle(
            ListTeacherRequestsQuery request, CancellationToken cancellationToken)
        {
            var page = await _requestRepository.GetPageAsync(
                request.Status, _timeProvider.GetUtcNow().UtcDateTime, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<TeacherAccessRequestListItem>>.Success(page);
        }
    }
}
