using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // دورة حياة الجلسات: الإصدار والتجديد والإلغاء
    public interface ISessionManager
    {
        Task<AuthResponseDto> IssueAsync(ApplicationUser user, string deviceId, string deviceName, CancellationToken cancellationToken);
        Task<Result<AuthResponseDto>> RefreshAsync(string refreshToken, CancellationToken cancellationToken);

        // إلغاء الجلسة المرتبطة بالـ Refresh Token (يعمل حتى بعد انتهاء الـ Access Token)
        Task<Result<bool>> RevokeByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

        // إلغاء جلسة واحدة تخص المستخدم (Session.NotFound لو غير موجودة أو تخص غيره، ونجاح لو منتهية أصلاً)
        Task<Result<bool>> RevokeSessionAsync(Guid userId, Guid sessionId, SessionRevokeReason reason, CancellationToken cancellationToken);

        Task RevokeAllForUserAsync(Guid userId, SessionRevokeReason reason, CancellationToken cancellationToken);
    }
}