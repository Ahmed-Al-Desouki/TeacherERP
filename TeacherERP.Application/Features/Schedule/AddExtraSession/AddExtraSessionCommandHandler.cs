using MediatR;
using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.AddExtraSession
{
    public class AddExtraSessionCommandHandler : IRequestHandler<AddExtraSessionCommand, Result<SessionTimeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public AddExtraSessionCommandHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupRepository groupRepository,
            IGroupSessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _groupRepository = groupRepository;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<SessionTimeDto>> Handle(AddExtraSessionCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<SessionTimeDto>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن المجموعة تخص المدرس الحالي فقط
            var group = await _groupRepository.GetWithSlotsAsync(request.GroupId, cancellationToken);
            if (group is null)
            {
                return Result<SessionTimeDto>.Failure(GroupErrors.NotFound);
            }

            if (!group.IsActive)
            {
                return Result<SessionTimeDto>.Failure(GroupErrors.Inactive);
            }

            var start = SessionTimeCalculator.NormalizeUtc(request.StartTime);
            if (start <= _timeProvider.GetUtcNow().UtcDateTime)
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.NewTimeInPast);
            }

            var end = start.AddMinutes(request.DurationMinutes);

            if (await _sessionRepository.HasConflictAsync(group.Id, start, end, Array.Empty<Guid>(), cancellationToken))
            {
                return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);
            }

            var session = Session.CreateExtra(tenant.TenantId, group.Id, start, end);
            _sessionRepository.AddRange(new[] { session });
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<SessionTimeDto>.Success(SessionTimeDto.From(session));
        }
    }
}