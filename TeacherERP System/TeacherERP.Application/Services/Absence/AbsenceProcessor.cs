// جديد — TeacherERP.Application/Absence/AbsenceProcessor.cs
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Absence
{
    public class AbsenceProcessor : IAbsenceProcessor
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IAbsenceService _absenceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<AbsenceProcessor> _logger;

        public AbsenceProcessor(
            ICurrentTenantAccessor currentTenant,
            IAbsenceRepository absenceRepository,
            IAbsenceService absenceService,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider,
            ILogger<AbsenceProcessor> logger)
        {
            _currentTenant = currentTenant;
            _absenceRepository = absenceRepository;
            _absenceService = absenceService;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task<int> ProcessAsync(CancellationToken cancellationToken)
        {
            // بدون سنتر محدد لا نعالج شيئاً (لا نخمّن المدرس)
            if (_currentTenant.Current is null)
            {
                return 0;
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var cutoff = now.AddMinutes(-AbsencePolicy.ProcessingGraceMinutes);

            var sessions = await _absenceRepository.GetUnprocessedEndedSessionsAsync(
                cutoff, AbsencePolicy.MaxSessionsPerRun, cancellationToken);

            var recorded = 0;

            // الترتيب الزمني مهم: العداد "غياب متتالي" فلازم الحصص تُحسب بترتيبها
            foreach (var session in sessions)
            {
                if (!session.IsCancelled)
                {
                    recorded += await ProcessSessionAsync(session, cancellationToken);
                }

                session.MarkAbsencesProcessed(now);

                // حفظ لكل حصة: أي فشل يتكرر في الدورة الجاية من غير تكرار ما اتحفظ
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            if (sessions.Count > 0)
            {
                _logger.LogInformation(
                    "Absence processing finished: {Sessions} sessions, {Absences} absences recorded.", sessions.Count, recorded);
            }

            return recorded;
        }

        private async Task<int> ProcessSessionAsync(Session session, CancellationToken cancellationToken)
        {
            var recorded = 0;

            // 1) من حضر الحصة يقطع سلسلة غيابه (يحمي من تأخر الـ Job عن حضور فعلي بعد غياب سابق)
            var presentIds = await _absenceRepository.GetPresentStudentIdsAsync(session.Id, cancellationToken);
            if (presentIds.Count > 0)
            {
                var presentMembers = await _absenceRepository.GetTrackedStudentMembersAsync(presentIds, cancellationToken);
                foreach (var member in presentMembers.Where(m => m.Status == MembershipStatus.Active && m.ConsecutiveAbsences > 0))
                {
                    member.ResetAbsence();
                }
            }

            // 2) الغائبون (التعويض المعلّق أو المعتمد يغطي الحصة فلا يُحسب غياباً، والرفض يُحسب لحظة الرفض)
            var candidateIds = await _absenceRepository.GetAbsentCandidateIdsAsync(session, cancellationToken);
            if (candidateIds.Count == 0)
            {
                return recorded;
            }

            var members = await _absenceRepository.GetTrackedStudentMembersAsync(candidateIds, cancellationToken);
            foreach (var member in members)
            {
                // الموقوف والمحظور غير مطلوب منهم الحضور أصلاً
                if (member.Status != MembershipStatus.Active)
                {
                    continue;
                }

                await _absenceService.RegisterAbsenceAsync(member, session.Id, cancellationToken);
                recorded++;
            }

            return recorded;
        }
    }
}