using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.GetSessions
{
    public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, Result<List<SessionDto>>>
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

        public async Task<Result<List<SessionDto>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<SessionDto>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<List<SessionDto>>.Failure(TenantErrors.Required);
            }

            var from = request.From is { } f ? NormalizeUtc(f) : _timeProvider.GetUtcNow().UtcDateTime;
            var to = request.To is { } t ? NormalizeUtc(t) : from.AddDays(SchedulePolicy.DefaultSessionsQueryDays);

            if (to <= from || (to - from).TotalDays > SchedulePolicy.MaxSessionsQueryDays)
            {
                return Result<List<SessionDto>>.Failure(new Error(
                    "Schedule.InvalidRange",
                    $"The range must be positive and at most {SchedulePolicy.MaxSessionsQueryDays} days."));
            }

            var sessions = await _sessionRepository.GetByRangeAsync(
                from, to, request.GroupId, userId, tenant.Role == TenantRole.Owner, cancellationToken);

            return Result<List<SessionDto>>.Success(sessions.Select(SessionDto.From).ToList());
        }

        // أي وقت بدون Kind يُعامل كـ UTC (كل النظام UTC)
        private static DateTime NormalizeUtc(DateTime value)
            => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}