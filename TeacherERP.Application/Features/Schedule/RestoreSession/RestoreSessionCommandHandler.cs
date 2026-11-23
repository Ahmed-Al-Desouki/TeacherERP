using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.RestoreSession
{
    public class RestoreSessionCommandHandler : IRequestHandler<RestoreSessionCommand, Result<SessionTimeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RestoreSessionCommandHandler(
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

        public async Task<Result<SessionTimeDto>> Handle(RestoreSessionCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<SessionTimeDto>.Failure(TenantErrors.Required);
            }

            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session is null)
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.NotFound);
            }

            if (session.StartTime <= _timeProvider.GetUtcNow().UtcDateTime)
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.InPast);
            }

            // الاسترجاع المتكرر ينجح بدون تغيير (Idempotent)
            if (session.IsCancelled)
            {
                var group = await _groupRepository.GetWithSlotsAsync(session.GroupId, cancellationToken);
                if (group is null) return Result<SessionTimeDto>.Failure(ScheduleErrors.NotFound);

                var overlaps = await _sessionRepository.GetCrossGroupConflictsAsync(
                    session.TenantId, group.Id, group.CenterId, group.Room, session.StartTime, session.EndTime,
                    new[] { session.Id }, cancellationToken);
                if (overlaps.Any(x => x.TenantId == session.TenantId))
                    return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);
                var roomConflicts = overlaps.Where(x => x.TenantId != session.TenantId).ToArray();
                if (roomConflicts.Length > 0 && !request.ConfirmRoomConflicts)
                    return Result<SessionTimeDto>.Failure(ScheduleErrors.RoomConflictConfirmationRequired, roomConflicts);

                // أثناء الإلغاء ممكن يكون وقتها اتشغل بحصة أخرى للمجموعة
                if (await _sessionRepository.HasConflictAsync(
                        session.GroupId, session.StartTime, session.EndTime, new[] { session.Id }, cancellationToken))
                {
                    return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);
                }

                session.Restore();
                const string notificationMessage = "A cancelled group session has been restored. Check the updated schedule.";
                await _notificationService.NotifyGroupStudentsAsync(
                    session.TenantId, session.GroupId, NotificationType.SessionRestored,
                    notificationMessage, session.Id, cancellationToken);
                await _notificationService.NotifyStaffAsync(
                    session.TenantId, NotificationType.SessionRestored,
                    notificationMessage, session.Id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<SessionTimeDto>.Success(SessionTimeDto.From(session));
        }
    }
}
