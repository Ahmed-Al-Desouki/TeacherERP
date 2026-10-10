using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // التوكن مع الأدوار التي وُضعت فيه (لتفادي سؤال الداتابيز عنها مرة تانية)
    public record AccessTokenResult(string Token, IReadOnlyList<string> Roles);

    public interface IJwtProvider
    {
        // التوكن مرتبط بالجلسة عبر Claim باسم session_id
        Task<AccessTokenResult> GenerateAccessTokenAsync(ApplicationUser user, Guid sessionId);
        string GenerateRefreshToken();
    }
}