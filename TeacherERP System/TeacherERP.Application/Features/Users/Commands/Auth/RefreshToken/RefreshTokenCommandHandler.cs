using MediatR;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
    {
        private readonly ISessionManager _sessionManager;

        public RefreshTokenCommandHandler(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
            => _sessionManager.RefreshAsync(request.RefreshToken, cancellationToken);
    }
}