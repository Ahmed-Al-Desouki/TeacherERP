// موجود (تعديل) — TeacherERP.Application/Interfaces/IUserAccountService.cs
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // دورة حياة الحساب: البحث والإنشاء والتفعيل والحذف
    public interface IUserAccountService
    {
        Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken);

        // Value == null تعني أن المستخدم غير موجود (وليس فشلاً)
        Task<Result<ApplicationUser?>> FindByEmailAsync(string email, CancellationToken cancellationToken);

        Task<Result<Guid>> CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken);

        // إنشاء بدون باسورد: الدخول يتم عبر مزود خارجي (Google)
        Task<Result<Guid>> CreateExternalUserAsync(ApplicationUser user, string role, CancellationToken cancellationToken);

        // أدوار الـ Identity للحساب
        Task<Result<IReadOnlyList<string>>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken);

        // تأكيد ملكية الإيميل (تفعيل حساب المساعد بعد الـ OTP)
        Task<Result<bool>> ConfirmEmailAsync(ApplicationUser user, CancellationToken cancellationToken);

        // حذف حساب (لإلغاء مساعد لم يُفعَّل بعد). المستدعي مسؤول عن التأكد أنه بلا بيانات مرتبطة
        Task<Result<bool>> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken);
    }
}