using TeacherERP.Domain.Shared;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken);
        Task<Result<Guid>> CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken);
        Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);
        Task<Result<bool>> PromoteToAdminAsync(Guid userId, CancellationToken cancellationToken);
    }
}
