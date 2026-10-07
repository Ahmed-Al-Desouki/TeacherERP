using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken);
        Task<Result<Guid>> CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken);
        Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);

        // Value == null تعني أن المستخدم غير موجود (وليس فشلاً)
        Task<Result<ApplicationUser?>> FindByEmailAsync(string email, CancellationToken cancellationToken);
        Task<Result<ApplicationUser?>> FindByExternalLoginAsync(string provider, string providerKey, CancellationToken cancellationToken);

        Task<Result<Guid>> CreateExternalUserAsync(ApplicationUser user, string role, CancellationToken cancellationToken);
        Task<Result<bool>> LinkExternalLoginAsync(ApplicationUser user, string provider, string providerKey, CancellationToken cancellationToken);

        Task<Result<bool>> ResetPasswordAsync(ApplicationUser user, string newPassword, CancellationToken cancellationToken);
    }
}