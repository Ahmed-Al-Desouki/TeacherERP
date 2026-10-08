using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface ISessionRepository
    {
        void Add(LoginSession session);

        // الجلسات النشطة للمستخدم (غير الملغاة وغير المنتهية). الحفظ يتم عبر IUnitOfWork
        Task<List<LoginSession>> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}