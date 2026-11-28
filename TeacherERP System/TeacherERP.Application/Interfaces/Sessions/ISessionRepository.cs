using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface ISessionRepository
    {
        void Add(LoginSession session);

        // الجلسات النشطة للمستخدم وقت nowUtc (غير الملغاة وغير المنتهية). الحفظ يتم عبر IUnitOfWork
        Task<List<LoginSession>> GetActiveByUserAsync(Guid userId, DateTime nowUtc, CancellationToken cancellationToken);

        // الجلسة مع المستخدم (بغض النظر عن حالتها)، أو null إن لم توجد
        Task<LoginSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken);

        // تحديث دوّار ذري: ينجح فقط إذا كان الـ hash المقدم ما زال هو الحالي والجلسة نشطة.
        Task<bool> TryRotateRefreshTokenAsync(
            LoginSession rotatedSession,
            string expectedCurrentHash,
            DateTime nowUtc,
            CancellationToken cancellationToken);

        // حذف مباشر (بدون تحميل) لكل جلسة انتهت أو أُلغيت قبل cutoffUtc، ويرجع عدد المحذوف
        Task<int> DeleteEndedBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken);
    }
}
