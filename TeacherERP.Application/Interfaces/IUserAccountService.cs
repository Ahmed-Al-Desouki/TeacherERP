using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // دورة حياة الحساب: البحث والإنشاء
    public interface IUserAccountService
    {
        Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken);

        // Value == null تعني أن المستخدم غير موجود (وليس فشلاً)
        Task<Result<ApplicationUser?>> FindByEmailAsync(string email, CancellationToken cancellationToken);

        Task<Result<Guid>> CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken);

        // إنشاء بدون باسورد: الدخول يتم عبر مزود خارجي (Google)
        Task<Result<Guid>> CreateExternalUserAsync(ApplicationUser user, string role, CancellationToken cancellationToken);
    }
}