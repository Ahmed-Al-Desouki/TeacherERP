using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.RevokeSession
{
    public class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ISessionManager _sessionManager;

        public RevokeSessionCommandHandler(ICurrentUserService currentUser, ISessionManager sessionManager)
        {
            _currentUser = currentUser;
            _sessionManager = sessionManager;
        }

        public Task<Result<bool>> Handle(RevokeSessionCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Task.FromResult(Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));
            }

            return _sessionManager.RevokeSessionAsync(userId, request.SessionId, SessionRevokeReason.UserLogout, cancellationToken);
        }
    }
}