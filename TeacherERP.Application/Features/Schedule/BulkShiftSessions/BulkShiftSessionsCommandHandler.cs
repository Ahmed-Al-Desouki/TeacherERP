using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.BulkShiftSessions
{
    public class BulkShiftSessionsCommandHandler : IRequestHandler<BulkShiftSessionsCommand, Result<BulkResultDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public BulkShiftSessionsCommandHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<BulkResultDto>> Handle(BulkShiftSessionsCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<BulkResultDto>.Failure(TenantErrors.Required);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var from = SessionTimeCalculator.NormalizeUtc(request.From);
            var to = SessionTimeCalculator.NormalizeUtc(request.To);
            if (from < now)
            {
                from = now;
            }

            if (to <= from)
            {
                return Result<BulkResultDto>.Success(new BulkResultDto(0, 0));
            }

            var sessions = await _sessionRepository.GetTrackedByRangeAsync(from, to, request.GroupIds, cancellationToken);
            var movable = sessions.Where(s => !s.IsCancelled).ToList();

            var movedIds = movable.Select(s => s.Id).ToArray();
            var occupiedStarts = movable.Select(s => (s.GroupId, s.StartTime)).ToHashSet();

            // التحقق من كل الحصص أولاً، فإما يتأجل الكل أو لا شيء (لا تأجيل جزئي)
            foreach (var session in movable)
            {
                var newStart = session.StartTime.AddMinutes(request.ShiftMinutes);
                var newEnd = session.EndTime.AddMinutes(request.ShiftMinutes);

                if (newStart <= now)
                {
                    return Result<BulkResultDto>.Failure(ScheduleErrors.NewTimeInPast);
                }

                // الوقت الجديد يطابق بداية حصة أخرى ستتحرك هي أيضاً: الـ Index الفريد يرفض الحفظ المتتابع
                if (occupiedStarts.Contains((session.GroupId, newStart))
                    || await _sessionRepository.HasConflictAsync(session.GroupId, newStart, newEnd, movedIds, cancellationToken))
                {
                    return Result<BulkResultDto>.Failure(ScheduleErrors.TimeConflict);
                }
            }

            foreach (var session in movable)
            {
                session.Reschedule(session.StartTime.AddMinutes(request.ShiftMinutes), session.EndTime.AddMinutes(request.ShiftMinutes));
            }

            if (movable.Count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<BulkResultDto>.Success(new BulkResultDto(movable.Count, sessions.Count - movable.Count));
        }
    }
}