using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.BulkCancelSessions
{
    public class BulkCancelSessionsCommandHandler : IRequestHandler<BulkCancelSessionsCommand, Result<BulkResultDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public BulkCancelSessionsCommandHandler(
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

            var affected = 0;
            foreach (var session in sessions.Where(s => !s.IsCancelled))
            {
                session.Cancel(request.Reason);
                affected++;
            }

            if (affected > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<BulkResultDto>.Success(new BulkResultDto(affected, sessions.Count - affected));
        }
    }
}