using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.GetMyCenters
{
    public class GetMyCentersQueryHandler : IRequestHandler<GetMyCentersQuery, Result<List<CenterDto>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICenterRepository _centerRepository;

        public GetMyCentersQueryHandler(ICurrentUserService currentUser, ICenterRepository centerRepository)
        {
            _currentUser = currentUser;
            _centerRepository = centerRepository;
        }

        public async Task<Result<List<CenterDto>>> Handle(GetMyCentersQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<CenterDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var centers = await _centerRepository.GetMineAsync(userId, cancellationToken);

            return Result<List<CenterDto>>.Success(centers.Select(center => CenterDto.From(center, userId)).ToList());
        }
    }
}