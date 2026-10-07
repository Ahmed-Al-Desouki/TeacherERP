using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface IAuthSessionService
    {
        Task<AuthResponseDto> IssueAsync(ApplicationUser user, string deviceInfo, CancellationToken cancellationToken);
    }
}