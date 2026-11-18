// جديد — TeacherERP.Application/Features/Groups/UpdateGroup/UpdateGroupCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Features.Schedule;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.UpdateGroup
{
    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, Result<GroupDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ICenterRepository _centerRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly TimeProvider _timeProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionGenerator _sessionGenerator;

        public UpdateGroupCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ICenterRepository centerRepository,
            IGroupRepository groupRepository,
            ITenantRepository tenantRepository,
            IGroupSessionRepository sessionRepository,
            TimeProvider timeProvider,
            IUnitOfWork unitOfWork,
            ISessionGenerator sessionGenerator)
        {
            _currentTenant = currentTenant;
            _centerRepository = centerRepository;
            _groupRepository = groupRepository;
            _tenantRepository = tenantRepository;
            _sessionRepository = sessionRepository;
            _timeProvider = timeProvider;
            _unitOfWork = unitOfWork;
            _sessionGenerator = sessionGenerator;
        }

        public async Task<Result<GroupDto>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<GroupDto>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن المجموعة تخص المدرس الحالي فقط
            var group = await _groupRepository.GetTrackedWithSlotsAsync(request.GroupId, cancellationToken);
            if (group is null)
            {
                return Result<GroupDto>.Failure(GroupErrors.NotFound);
            }

            if (!await _centerRepository.IsActiveTeacherAtAsync(request.CenterId, tenant.TenantId, cancellationToken))
            {
                return Result<GroupDto>.Failure(GroupErrors.CenterNotAllowed);
            }

            if (await _groupRepository.NameExistsExceptAsync(request.Name.Trim(), group.Id, cancellationToken))
            {
                return Result<GroupDto>.Failure(GroupErrors.NameTaken);
            }

            var roomConflicts = new List<ScheduleConflictItem>();
            if (request.IsActive)
            {
                var tenantProfile = await _tenantRepository.GetByIdAsync(tenant.TenantId, cancellationToken);
                var templateConflicts = await _sessionRepository.GetTemplateConflictsAsync(
                    tenant.TenantId, group.Id, request.CenterId, request.Room, tenantProfile?.TimeZoneId ?? "UTC",
                    group.Slots.Select(x => new ScheduleSlotCandidate(x.Day, x.StartTime, x.DurationMinutes)).ToArray(), null, cancellationToken);
                if (templateConflicts.Any(x => x.TenantId == tenant.TenantId))
                    return Result<GroupDto>.Failure(ScheduleErrors.TimeConflict);
                roomConflicts.AddRange(templateConflicts.Where(x => x.TenantId != tenant.TenantId));
            }

            var fromUtc = _timeProvider.GetUtcNow().UtcDateTime;
            var currentSessions = await _sessionRepository.GetTrackedByRangeAsync(
                fromUtc, fromUtc.AddDays(28), new[] { group.Id }, cancellationToken);
            foreach (var session in currentSessions.Where(x => !x.IsCancelled))
            {
                var actualConflicts = await _sessionRepository.GetCrossGroupConflictsAsync(
                    tenant.TenantId, group.Id, request.CenterId, request.Room,
                    session.StartTime, session.EndTime, new[] { session.Id }, cancellationToken);
                if (actualConflicts.Any(x => x.TenantId == tenant.TenantId))
                    return Result<GroupDto>.Failure(ScheduleErrors.TimeConflict);
                roomConflicts.AddRange(actualConflicts.Where(x => x.TenantId != tenant.TenantId));
            }
            var distinctRoomConflicts = roomConflicts
                .DistinctBy(x => (x.GroupId, x.StartTime, x.EndTime)).ToArray();
            if (distinctRoomConflicts.Length > 0 && !request.ConfirmRoomConflicts)
                return Result<GroupDto>.Failure(ScheduleErrors.RoomConflictConfirmationRequired, distinctRoomConflicts);

            group.UpdateDetails(request.Name, request.Grade, request.CenterId, request.Room, request.Capacity, request.WhatsAppGroupLink);
            group.SetActive(request.IsActive);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // إعادة تفعيل مجموعة مؤرشفة تستكمل حصصها القادمة (الأرشفة تكتفي بإيقاف التوليد)
            if (group.IsActive)
            {
                await _sessionGenerator.GenerateForGroupAsync(group.Id, cancellationToken);
            }

            return Result<GroupDto>.Success(GroupDto.From(group));
        }
    }
}
