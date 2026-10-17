using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.RestoreSession
{
    public class RestoreSessionCommandHandler : IRequestHandler<RestoreSessionCommand, Result<SessionTimeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RestoreSessionCommandHandler(
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
                // أثناء الإلغاء ممكن يكون وقتها اتشغل بحصة أخرى للمجموعة
                if (await _sessionRepository.HasConflictAsync(
                        session.GroupId, session.StartTime, session.EndTime, new[] { session.Id }, cancellationToken))
                {
                    return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);
                }

                session.Restore();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<SessionTimeDto>.Success(SessionTimeDto.From(session));
        }
    }
}