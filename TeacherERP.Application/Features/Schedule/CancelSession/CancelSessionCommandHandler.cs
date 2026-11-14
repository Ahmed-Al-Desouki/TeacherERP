using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.CancelSession
{
    public class CancelSessionCommandHandler : IRequestHandler<CancelSessionCommand, Result<SessionTimeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CancelSessionCommandHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            IAttendanceRepository attendanceRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _attendanceRepository = attendanceRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
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
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<SessionTimeDto>.Success(SessionTimeDto.From(session));
        }
    }
}