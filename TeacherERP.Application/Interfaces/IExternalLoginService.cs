using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // الدخول عبر مزودين خارجيين (Google)
    public interface IExternalLoginService
    {
        // Value == null تعني أن المستخدم غير موجود (وليس فشلاً)
        Task<Result<ApplicationUser?>> FindByExternalLoginAsync(string provider, string providerKey, CancellationToken cancellationToken);
        Task<Result<bool>> LinkExternalLoginAsync(ApplicationUser user, string provider, string providerKey, CancellationToken cancellationToken);
    }
}