using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        void Add(RefreshToken refreshToken);

        // يُبطل كل الجلسات النشطة للمستخدم (الحفظ يتم عبر IUnitOfWork)
        Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}