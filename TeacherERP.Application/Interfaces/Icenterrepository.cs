using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface ICenterRepository
    {
        // السناتر التي أملكها، أو أدرّس فيها (نشط) بمساحة أملكها
        Task<List<Center>> GetMineAsync(Guid userId, CancellationToken cancellationToken);

        // السنتر متتبَّع للتعديل. الحفظ يتم عبر IUnitOfWork
        Task<Center?> GetByIdAsync(Guid centerId, CancellationToken cancellationToken);
    }
}