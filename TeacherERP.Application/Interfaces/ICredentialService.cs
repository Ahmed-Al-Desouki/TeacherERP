using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // بيانات الدخول: التحقق من الباسورد وإعادة تعيينه
    public interface ICredentialService
    {
        Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);
        Task<Result<bool>> ResetPasswordAsync(ApplicationUser user, string newPassword, CancellationToken cancellationToken);
    }
}