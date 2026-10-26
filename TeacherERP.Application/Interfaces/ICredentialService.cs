// موجود (تعديل) — TeacherERP.Application/Interfaces/ICredentialService.cs
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // بيانات الدخول: التحقق من الباسورد وتغييره وإعادة تعيينه
    public interface ICredentialService
    {
        // يفحص القفل والباسورد فقط. حالة الحساب وتفعيل الإيميل يقررهما الـ Handler
        Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);
        Task<Result<bool>> ResetPasswordAsync(ApplicationUser user, string newPassword, CancellationToken cancellationToken);

        // تغيير الباسورد للمسجّل دخوله بالباسورد الحالي (لا يعمل لحساب بلا باسورد)
        Task<Result<bool>> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword, CancellationToken cancellationToken);
    }
}