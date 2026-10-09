using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly ISessionManager _sessionManager;

        public LogoutCommandHandler(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
            => _sessionManager.RevokeByRefreshTokenAsync(request.RefreshToken, cancellationToken);
    }
}