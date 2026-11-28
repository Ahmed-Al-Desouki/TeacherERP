using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.LogoutAll
{
    public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ISessionManager _sessionManager;

        public LogoutAllCommandHandler(ICurrentUserService currentUser, ISessionManager sessionManager)
        {
            _currentUser = currentUser;
            _sessionManager = sessionManager;
        }

        public async Task<Result<bool>> Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            await _sessionManager.RevokeAllForUserAsync(userId, SessionRevokeReason.LogoutAll, cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}