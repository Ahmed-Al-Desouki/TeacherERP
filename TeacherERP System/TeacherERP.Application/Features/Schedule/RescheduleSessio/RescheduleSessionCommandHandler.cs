using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.RescheduleSession
{
    public class RescheduleSessionCommandHandler : IRequestHandler<RescheduleSessionCommand, Result<SessionTimeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RescheduleSessionCommandHandler(
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

        public async Task<Result<SessionTimeDto>> Handle(RescheduleSessionCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<SessionTimeDto>.Failure(TenantErrors.Required);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            // الفلتر العام يضمن أن الحصة تخص المدرس الحالي فقط
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session is null)
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.NotFound);
            }

            // حصص الماضي سجل تاريخي (حضور وغياب)، لا تُعدَّل
            if (session.StartTime <= now)
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.InPast);
            }

            if (session.IsCancelled)
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.Cancelled);
            }

            var newStart = SessionTimeCalculator.NormalizeUtc(request.NewStartTime);
            if (newStart <= now)
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.NewTimeInPast);
            }

            var duration = request.DurationMinutes ?? (int)(session.EndTime - session.StartTime).TotalMinutes;
            var newEnd = newStart.AddMinutes(duration);

            var group = await _groupRepository.GetWithSlotsAsync(session.GroupId, cancellationToken);
            if (group is null) return Result<SessionTimeDto>.Failure(ScheduleErrors.NotFound);

            var overlaps = await _sessionRepository.GetCrossGroupConflictsAsync(
                session.TenantId, group.Id, group.CenterId, group.Room, newStart, newEnd,
                new[] { session.Id }, cancellationToken);
            if (overlaps.Any(x => x.TenantId == session.TenantId))
                return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);
            var roomConflicts = overlaps.Where(x => x.TenantId != session.TenantId).ToArray();
            if (roomConflicts.Length > 0 && !request.ConfirmRoomConflicts)
                return Result<SessionTimeDto>.Failure(ScheduleErrors.RoomConflictConfirmationRequired, roomConflicts);

            if (await _sessionRepository.HasConflictAsync(session.GroupId, newStart, newEnd, new[] { session.Id }, cancellationToken))
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);
            }

            var scheduleChanged = session.StartTime != newStart || session.EndTime != newEnd;
            session.Reschedule(newStart, newEnd);
            if (scheduleChanged)
            {
                const string notificationMessage = "A group session was rescheduled. Check the updated schedule.";
                await _notificationService.NotifyGroupStudentsAsync(
                    session.TenantId, session.GroupId, NotificationType.SessionRescheduled,
                    notificationMessage, session.Id, cancellationToken);
                await _notificationService.NotifyStaffAsync(
                    session.TenantId, NotificationType.SessionRescheduled,
                    notificationMessage, session.Id, cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<SessionTimeDto>.Success(SessionTimeDto.From(session));
        }
    }
}
