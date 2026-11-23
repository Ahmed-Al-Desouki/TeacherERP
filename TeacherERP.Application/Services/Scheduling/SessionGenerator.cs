// موجود (تعديل) — TeacherERP.Application/Scheduling/SessionGenerator.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Scheduling
{
    public class SessionGenerator : ISessionGenerator
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public SessionGenerator(
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository,
            IGroupRepository groupRepository,
            IGroupSessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
            _groupRepository = groupRepository;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<int> GenerateForGroupAsync(Guid groupId, CancellationToken cancellationToken, DateOnly? fromLocalDate = null)
        {
            var group = await _groupRepository.GetWithSlotsAsync(groupId, cancellationToken);
            if (group is null || !group.IsActive)
            {
                return 0;
            }

            var generated = await BuildAsync(new[] { group }, fromLocalDate, cancellationToken);
            if (generated > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return generated;
        }

        public async Task<int> GenerateForAllGroupsAsync(CancellationToken cancellationToken)
        {
            var groups = await _groupRepository.GetActiveWithSlotsAsync(cancellationToken);

            var generated = await BuildAsync(groups, null, cancellationToken);
            if (generated > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return generated;
        }

        // يضيف الحصص الناقصة فقط (بدون حفظ) ويرجع عددها
        private async Task<int> BuildAsync(IReadOnlyCollection<Group> groups, DateOnly? fromLocalDate, CancellationToken cancellationToken)
        {
            // بدون سنتر محدد لا نولّد شيئاً (لا نخمّن المدرس)
            if (_currentTenant.Current is not { } current || groups.Count == 0)
            {
                return 0;
            }

            var tenant = await _tenantRepository.GetByIdAsync(current.TenantId, cancellationToken);
            var zone = SessionTimeCalculator.ResolveZone(tenant?.TimeZoneId);

            var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
            var today = SessionTimeCalculator.LocalDateOf(nowUtc, zone);
            var startDate = fromLocalDate is { } requested && requested > today ? requested : today;
            var lastDay = today.AddDays(SchedulePolicy.GenerationHorizonDays);
            var windowEndUtc = SessionTimeCalculator.ToUtc(lastDay.AddDays(1), TimeSpan.Zero, zone);

            var total = 0;

            foreach (var group in groups)
            {
                var existing = await _sessionRepository.GetStartTimesAsync(group.Id, nowUtc, windowEndUtc, cancellationToken);
                var sessions = new List<Session>();

                for (var date = startDate; date <= lastDay; date = date.AddDays(1))
                {
                    foreach (var slot in group.Slots.Where(s => s.Day == date.DayOfWeek))
                    {
                        var startUtc = SessionTimeCalculator.ToUtc(date, slot.StartTime, zone);

                        // لا نولّد في الماضي، ولا نكرر حصة موجودة
                        if (startUtc <= nowUtc || !existing.Add(startUtc))
                        {
                            continue;
                        }

                        sessions.Add(Session.Create(
                            current.TenantId, group.Id, startUtc, startUtc.AddMinutes(slot.DurationMinutes), slot.Id));
                    }
                }

                if (sessions.Count > 0)
                {
                    _sessionRepository.AddRange(sessions);
                    total += sessions.Count;
                }
            }

            return total;
        }
    }
}