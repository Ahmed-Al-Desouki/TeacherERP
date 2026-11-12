// موجود (تعديل) — TeacherERP.Application/Features/Schedule/GetSessions/GetSessionsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.GetSessions
{
    public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, Result<PagedResult<SessionDto>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly TimeProvider _timeProvider;

        public GetSessionsQueryHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<PagedResult<SessionDto>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { })
            {
                return Result<PagedResult<SessionDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<SessionDto>>.Failure(TenantErrors.Required);
            }

            var from = request.From is { } f ? SessionTimeCalculator.NormalizeUtc(f) : _timeProvider.GetUtcNow().UtcDateTime;
            var to = request.To is { } t ? SessionTimeCalculator.NormalizeUtc(t) : from.AddDays(SchedulePolicy.DefaultSessionsQueryDays);

            if (to <= from || (to - from).TotalDays > SchedulePolicy.MaxSessionsQueryDays)
            {
                return Result<PagedResult<SessionDto>>.Failure(ScheduleErrors.InvalidRange());
            }

            var page = await _sessionRepository.GetByRangeAsync(
                from, to, request.GroupId, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<SessionDto>>.Success(page.Map(SessionDto.From));
        }
    }
}
