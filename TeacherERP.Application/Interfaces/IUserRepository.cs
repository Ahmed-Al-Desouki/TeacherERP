using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface IUserRepository
    {
        // المستخدم مع بياناته (متتبَّع للتعديل). الحفظ يتم عبر IUnitOfWork
        Task<ApplicationUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

        // هل رقم الموبايل (بالصيغة الدولية) مستخدم عند مستخدم آخر؟
        Task<bool> PhoneNumberExistsAsync(string phoneNumber, Guid excludeUserId, CancellationToken cancellationToken);
    }
}