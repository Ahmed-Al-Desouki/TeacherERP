using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    public record GoogleUserInfo(string Subject, string Email, string FullName);

    public interface IGoogleAuthService
    {
        Task<Result<GoogleUserInfo>> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken);
    }
}