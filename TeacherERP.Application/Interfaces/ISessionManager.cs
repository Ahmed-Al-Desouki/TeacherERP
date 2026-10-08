using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // دورة حياة الجلسات: الإصدار والإلغاء
    public interface ISessionManager
    {
        Task<AuthResponseDto> IssueAsync(ApplicationUser user, string deviceId, string deviceName, CancellationToken cancellationToken);
        Task RevokeAllForUserAsync(Guid userId, SessionRevokeReason reason, CancellationToken cancellationToken);
    }
}