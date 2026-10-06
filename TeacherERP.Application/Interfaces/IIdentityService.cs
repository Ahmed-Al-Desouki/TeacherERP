using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken);
        Task<Result<Guid>> CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken);
        Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);
    }
}