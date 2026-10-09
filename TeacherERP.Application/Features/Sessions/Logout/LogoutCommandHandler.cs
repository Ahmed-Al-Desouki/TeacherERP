using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ISessionManager _sessionManager;

        public LogoutCommandHandler(ICurrentUserService currentUser, ISessionManager sessionManager)
        {
            _currentUser = currentUser;
            _sessionManager = sessionManager;
        }

        public Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId || _currentUser.SessionId is not { } sessionId)
            {
                return Task.FromResult(Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));
            }

            return _sessionManager.RevokeSessionAsync(userId, sessionId, SessionRevokeReason.UserLogout, cancellationToken);
        }
    }
}