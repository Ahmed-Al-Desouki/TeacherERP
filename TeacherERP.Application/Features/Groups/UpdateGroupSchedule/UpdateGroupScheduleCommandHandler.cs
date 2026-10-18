// جديد — TeacherERP.Application/Features/Groups/UpdateGroupSchedule/UpdateGroupScheduleCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.UpdateGroupSchedule
{
    public class UpdateGroupScheduleCommandHandler : IRequestHandler<UpdateGroupScheduleCommand, Result<GroupDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionGenerator _sessionGenerator;
        private readonly TimeProvider _timeProvider;

        public UpdateGroupScheduleCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository,
            IGroupRepository groupRepository,
            IGroupSessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            ISessionGenerator sessionGenerator,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
            _groupRepository = groupRepository;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _sessionGenerator = sessionGenerator;
            _timeProvider = timeProvider;
        }

        public async Task<Result<GroupDto>> Handle(UpdateGroupScheduleCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } current)
            {
                return Result<GroupDto>.Failure(TenantErrors.Required);
            }

            var group = await _groupRepository.GetTrackedWithSlotsAsync(request.GroupId, cancellationToken);
            if (group is null)
            {
                return Result<GroupDto>.Failure(GroupErrors.NotFound);
            }

            var tenant = await _tenantRepository.GetByIdAsync(current.TenantId, cancellationToken);
            var zone = SessionTimeCalculator.ResolveZone(tenant?.TimeZoneId);

            // التعديل لا يمسّ الماضي: يبدأ من اليوم على الأقل، وحصص اليوم الجارية تبقى كما هي
            var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
            var today = SessionTimeCalculator.LocalDateOf(nowUtc, zone);
            var effectiveDate = request.EffectiveFrom is { } requested && requested > today ? requested : today;
            var effectiveFromUtc = SessionTimeCalculator.ToUtc(effectiveDate, TimeSpan.Zero, zone);
            if (effectiveFromUtc < nowUtc)
            {
                effectiveFromUtc = nowUtc;
            }

            // مقارنة القالب القديم بالجديد بمفتاح (اليوم، وقت البداية)
            var desired = request.Slots.ToDictionary(s => (s.Day, s.StartTime));
            var existing = group.Slots.ToDictionary(s => (s.Day, s.StartTime));

            var removed = existing.Where(pair => !desired.ContainsKey(pair.Key)).Select(pair => pair.Value).ToList();
            var added = desired.Where(pair => !existing.ContainsKey(pair.Key)).Select(pair => pair.Value).ToList();
            var resized = existing
                .Where(pair => desired.TryGetValue(pair.Key, out var input) && input.DurationMinutes != pair.Value.DurationMinutes)
                .Select(pair => pair.Value)
                .ToList();

            // الحصص المتأثرة: المولّدة من حصص قالب محذوفة أو تغيّرت مدتها
            var affectedSlotIds = removed.Concat(resized).Select(s => s.Id).ToArray();
            var sessions = affectedSlotIds.Length == 0
                ? new List<Session>()
                : await _sessionRepository.GetTrackedBySlotIdsAsync(affectedSlotIds, cancellationToken);

            var withAttendance = await _sessionRepository.GetSessionIdsWithAttendanceAsync(
                sessions.Select(s => s.Id).ToArray(), cancellationToken);

            var removedIds = removed.Select(s => s.Id).ToHashSet();
            var newDurations = resized.ToDictionary(s => s.Id, s => desired[(s.Day, s.StartTime)].DurationMinutes);
            var toDelete = new List<Session>();

            foreach (var session in sessions)
            {
                var isUpcoming = session.StartTime >= effectiveFromUtc;
                var isUntouched = !session.IsModified && !withAttendance.Contains(session.Id);

                if (session.SlotId is { } slotId && removedIds.Contains(slotId))
                {
                    // القادمة غير المعدّلة وبلا حضور تُحذف وتُولَّد من جديد، والباقي (ماضٍ أو معدّل أو عليه حضور) يبقى مستقلاً
                    if (isUpcoming && isUntouched)
                    {
                        toDelete.Add(session);
                    }
                    else
                    {
                        session.DetachFromSlot();
                    }
                }
                else if (isUpcoming && isUntouched && session.SlotId is { } resizedSlotId && newDurations.TryGetValue(resizedSlotId, out var minutes))
                {
                    session.SyncEnd(session.StartTime.AddMinutes(minutes));
                }
            }

            foreach (var slot in resized)
            {
                slot.SetDuration(newDurations[slot.Id]);
            }

            foreach (var slot in removed)
            {
                group.RemoveSlot(slot);
            }

            _groupRepository.RemoveSlots(removed);

            foreach (var input in added)
            {
                _groupRepository.TrackNewSlot(group.AddSlot(input.Day, input.StartTime, input.DurationMinutes));
            }

            _sessionRepository.RemoveRange(toDelete);
            group.MarkAsUpdated();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // توليد الحصص الناقصة من التاريخ الفعلي (آمن للتكرار)
            if (group.IsActive)
            {
                await _sessionGenerator.GenerateForGroupAsync(group.Id, cancellationToken, effectiveDate);
            }

            return Result<GroupDto>.Success(GroupDto.From(group));
        }
    }
}