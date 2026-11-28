using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.GetMySessions
{
    public class GetMySessionsQueryHandler : IRequestHandler<GetMySessionsQuery, Result<List<SessionDto>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ISessionRepository _sessionRepository;
        private readonly TimeProvider _timeProvider;

        public GetMySessionsQueryHandler(
            ICurrentUserService currentUser,
            ISessionRepository sessionRepository,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _sessionRepository = sessionRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<List<SessionDto>>> Handle(GetMySessionsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<SessionDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var sessions = await _sessionRepository.GetActiveByUserAsync(userId, now, cancellationToken);
            var currentSessionId = _currentUser.SessionId;

            var result = sessions
                .OrderByDescending(s => s.LastRefreshedAt)
                .Select(s => new SessionDto(s.Id, s.DeviceName, s.CreatedAt, s.LastRefreshedAt, s.Id == currentSessionId))
                .ToList();

            return Result<List<SessionDto>>.Success(result);
        }
    }
}