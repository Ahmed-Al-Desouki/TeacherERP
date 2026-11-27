using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.CancelSession
{
    public class CancelSessionCommandHandler : IRequestHandler<CancelSessionCommand, Result<SessionTimeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        private readonly ICurrentUserService _currentUser;
        private readonly IAuditRepository _auditRepository;

        public CancelSessionCommandHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            IAttendanceRepository attendanceRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider,
            ICurrentUserService currentUser,
            IAuditRepository auditRepository)
        {
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _attendanceRepository = attendanceRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
            _currentUser = currentUser;
            _auditRepository = auditRepository;
        }

        public async Task<Result<SessionTimeDto>> Handle(CancelSessionCommand request, CancellationToken cancellationToken)
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

            // الإلغاء المتكرر ينجح بدون تغيير (Idempotent)
            if (!session.IsCancelled)
            {
                // تعويض معلّق مربوط بالحصة يمنع إلغاءها حتى يُبتّ فيه
                var blocked = await _attendanceRepository.GetSessionIdsWithPendingCompensationAsync(
                    new[] { session.Id }, cancellationToken);
                if (blocked.Count > 0)
                {
                    return Result<SessionTimeDto>.Failure(ScheduleErrors.HasPendingCompensations);
                }

                session.Cancel(request.Reason);
                _auditRepository.Add(AuditLog.Create(
                    session.TenantId, _currentUser.UserId, AuditAction.SessionCancelled, null,
                    $"SessionId={session.Id}; GroupId={session.GroupId}; Reason={request.Reason?.Trim()}"));
                var message = string.IsNullOrWhiteSpace(request.Reason)
                    ? "Your group session was cancelled."
                    : $"Your group session was cancelled. Reason: {request.Reason.Trim()}";
                await _notificationService.NotifyGroupStudentsAsync(
                    session.TenantId, session.GroupId, NotificationType.SessionCancelled,
                    message, session.Id, cancellationToken);
                await _notificationService.NotifyStaffAsync(
                    session.TenantId, NotificationType.SessionCancelled,
                    $"A group session was cancelled. {message}", session.Id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<SessionTimeDto>.Success(SessionTimeDto.From(session));
        }
    }
}
