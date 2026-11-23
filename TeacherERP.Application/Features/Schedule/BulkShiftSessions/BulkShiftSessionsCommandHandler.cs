using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.BulkShiftSessions
{
    public class BulkShiftSessionsCommandHandler : IRequestHandler<BulkShiftSessionsCommand, Result<BulkResultDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public BulkShiftSessionsCommandHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            IGroupRepository groupRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _groupRepository = groupRepository;
            _notificationService = notificationService;
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
            var roomConflicts = new Dictionary<(Guid GroupId, DateTime Start, Guid ConflictGroupId, DateTime ConflictStart), ScheduleConflictItem>();

            // فحص أزواج الحصص المختارة بعد إزاحتها يمنع التعارض بين مجموعات المدرس حتى داخل الدفعة نفسها.
            for (var i = 0; i < movable.Count; i++)
            {
                var left = movable[i];
                var leftStart = left.StartTime.AddMinutes(request.ShiftMinutes);
                var leftEnd = left.EndTime.AddMinutes(request.ShiftMinutes);
                for (var j = i + 1; j < movable.Count; j++)
                {
                    var right = movable[j];
                    var rightStart = right.StartTime.AddMinutes(request.ShiftMinutes);
                    var rightEnd = right.EndTime.AddMinutes(request.ShiftMinutes);
                    if (left.GroupId != right.GroupId && leftStart < rightEnd && leftEnd > rightStart)
                        return Result<BulkResultDto>.Failure(ScheduleErrors.TimeConflict);
                }
            }

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

                var group = await _groupRepository.GetWithSlotsAsync(session.GroupId, cancellationToken);
                if (group is null) return Result<BulkResultDto>.Failure(ScheduleErrors.NotFound);
                var overlaps = await _sessionRepository.GetCrossGroupConflictsAsync(
                    session.TenantId, group.Id, group.CenterId, group.Room, newStart, newEnd, movedIds, cancellationToken);
                if (overlaps.Any(x => x.TenantId == session.TenantId))
                    return Result<BulkResultDto>.Failure(ScheduleErrors.TimeConflict);
                foreach (var item in overlaps.Where(x => x.TenantId != session.TenantId))
                    roomConflicts[(group.Id, newStart, item.GroupId, item.StartTime)] = item;
            }

            if (roomConflicts.Count > 0 && !request.ConfirmRoomConflicts)
                return Result<BulkResultDto>.Failure(ScheduleErrors.RoomConflictConfirmationRequired,
                    roomConflicts.Values.OrderBy(x => x.StartTime).ToArray());

            foreach (var session in movable)
            {
                session.Reschedule(session.StartTime.AddMinutes(request.ShiftMinutes), session.EndTime.AddMinutes(request.ShiftMinutes));
            }

            if (movable.Count > 0)
            {
                if (request.ShiftMinutes != 0)
                {
                    foreach (var group in movable.GroupBy(session => session.GroupId))
                    {
                        var groupSessions = group.ToArray();
                        var message = $"{groupSessions.Length} group session(s) are now scheduled between " +
                                      $"{groupSessions.Min(session => session.StartTime):O} " +
                                      $"and {groupSessions.Max(session => session.StartTime):O}.";
                        await _notificationService.NotifyGroupStudentsAsync(
                            groupSessions[0].TenantId, group.Key, NotificationType.BulkSessionsRescheduled,
                            message, group.Key, cancellationToken);
                    }

                    await _notificationService.NotifyStaffAsync(
                        movable[0].TenantId, NotificationType.BulkSessionsRescheduled,
                        $"{movable.Count} upcoming session(s) were rescheduled.", null, cancellationToken);
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<BulkResultDto>.Success(new BulkResultDto(movable.Count, sessions.Count - movable.Count));
        }
    }
}
