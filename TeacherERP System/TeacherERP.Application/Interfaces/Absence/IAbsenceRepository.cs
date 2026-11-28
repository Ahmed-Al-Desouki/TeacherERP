// موجود (تعديل) — TeacherERP.Application/Interfaces/IAbsenceRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // كل الاستعلامات داخل المدرس الحالي (الفلتر العام يعمل). الحفظ يتم عبر IUnitOfWork
    public interface IAbsenceRepository
    {
        // حصص انتهت قبل cutoff ولم تُعالَج، بترتيب البداية (متتبَّعة للتعديل)
        Task<List<Session>> GetUnprocessedEndedSessionsAsync(DateTime cutoffUtc, int take, CancellationToken cancellationToken);

        // طلاب كان تسجيلهم في مجموعة الحصة ساري وقتها، بلا أي سجل حضور فيها، ولا تعويض (معلّق أو معتمد) يغطيها
        Task<List<Guid>> GetAbsentCandidateIdsAsync(Session session, CancellationToken cancellationToken);

        // طلاب حضروا هذه الحصة فعلاً (Present)
        Task<List<Guid>> GetPresentStudentIdsAsync(Guid sessionId, CancellationToken cancellationToken);

        // عضويات الطلاب عند المدرس الحالي متتبَّعة للتعديل
        Task<List<TenantMember>> GetTrackedStudentMembersAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken);

        // هل كان الطالب مسجلاً في مجموعة الحصة وقت بدايتها؟
        Task<bool> WasEnrolledAtAsync(Session session, Guid studentId, CancellationToken cancellationToken);

        // عدد الغيابات المسجّلة بعد آخر حضور للطالب عند المدرس الحالي (الغياب المتتالي الفعلي من السجلات)
        Task<int> CountTrailingAbsencesAsync(Guid studentId, CancellationToken cancellationToken);
    }
}