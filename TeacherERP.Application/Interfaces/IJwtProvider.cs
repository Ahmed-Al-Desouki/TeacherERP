using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface IJwtProvider
    {

        //Task<string> GenerateAccessTokenAsync(ApplicationUser user, Guid sessionId);
        // التوكن مرتبط بالجلسة عبر Claim باسم session_id
        Task<string> GenerateAccessTokenAsync(ApplicationUser user, Guid sessionId);
        string GenerateRefreshToken();
    }
}
