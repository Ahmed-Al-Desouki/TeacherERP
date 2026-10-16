using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface ICenterRepository
    {
        // السناتر التي أملكها، أو أدرّس فيها (نشط) بمساحة أملكها
        Task<List<Center>> GetMineAsync(Guid userId, CancellationToken cancellationToken);

        // هل المدرس (بمساحته) نشط في هذا السنتر؟
        Task<bool> IsActiveTeacherAtAsync(Guid centerId, Guid tenantId, CancellationToken cancellationToken);

        // السنتر متتبَّع للتعديل. الحفظ يتم عبر IUnitOfWork
        Task<Center?> GetByIdAsync(Guid centerId, CancellationToken cancellationToken);
    }
}