using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.BulkCancelSessions
{
    public class BulkCancelSessionsCommandHandler : IRequestHandler<BulkCancelSessionsCommand, Result<BulkResultDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        private readonly ICurrentUserService _currentUser;
        private readonly IAuditRepository _auditRepository;

        public BulkCancelSessionsCommandHandler(
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

        public async Task<Result<BulkResultDto>> Handle(BulkCancelSessionsCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<BulkResultDto>.Failure(TenantErrors.Required);
            }

            // الماضي لا يُلغى (سجل تاريخي)، فنبدأ من الآن على الأكثر
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

            // إما يُلغى الكل أو لا شيء: أي حصة عليها تعويض معلّق تمنع الإلغاء الجماعي (نفس قاعدة التأجيل الجماعي)
            var cancellable = sessions.Where(s => !s.IsCancelled).ToList();
            if (cancellable.Count > 0)
            {
                var blocked = await _attendanceRepository.GetSessionIdsWithPendingCompensationAsync(
                    cancellable.Select(s => s.Id).ToArray(), cancellationToken);
                if (blocked.Count > 0)
                {
                    return Result<BulkResultDto>.Failure(ScheduleErrors.HasPendingCompensations);
                }
            }

            var affected = 0;
            foreach (var session in cancellable)
            {
                session.Cancel(request.Reason);
                _auditRepository.Add(AuditLog.Create(
                    session.TenantId, _currentUser.UserId, AuditAction.SessionBulkCancelled, null,
                    $"BulkCancel; SessionId={session.Id}; GroupId={session.GroupId}; Reason={request.Reason?.Trim()}"));
                affected++;
            }

            if (affected > 0)
            {
                foreach (var group in cancellable.GroupBy(session => session.GroupId))
                {
                    var groupSessions = group.ToArray();
                    var message = $"{groupSessions.Length} group session(s) were cancelled " +
                                  $"from {groupSessions.Min(session => session.StartTime):O} " +
                                  $"through {groupSessions.Max(session => session.StartTime):O}." +
                                  (string.IsNullOrWhiteSpace(request.Reason) ? string.Empty : $" Reason: {request.Reason.Trim()}");
                    await _notificationService.NotifyGroupStudentsAsync(
                        groupSessions[0].TenantId, group.Key, NotificationType.BulkSessionsCancelled,
                        message, group.Key, cancellationToken);
                }

                await _notificationService.NotifyStaffAsync(
                    cancellable[0].TenantId, NotificationType.BulkSessionsCancelled,
                    $"{affected} upcoming session(s) were cancelled.", null, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<BulkResultDto>.Success(new BulkResultDto(affected, sessions.Count - affected));
        }
    }
}
