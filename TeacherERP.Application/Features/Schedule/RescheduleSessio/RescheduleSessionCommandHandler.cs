using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.RescheduleSession
{
    public class RescheduleSessionCommandHandler : IRequestHandler<RescheduleSessionCommand, Result<SessionTimeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RescheduleSessionCommandHandler(
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

            if (await _sessionRepository.HasConflictAsync(session.GroupId, newStart, newEnd, new[] { session.Id }, cancellationToken))
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);
            }

            session.Reschedule(newStart, newEnd);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<SessionTimeDto>.Success(SessionTimeDto.From(session));
        }
    }
}