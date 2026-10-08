using Hangfire;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Sessions
{
    public class SessionManager : ISessionManager
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public SessionManager(
            IJwtProvider jwtProvider,
            ISessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            IBackgroundJobClient backgroundJobClient)
        {
            _jwtProvider = jwtProvider;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<AuthResponseDto> IssueAsync(ApplicationUser user, string deviceId, string deviceName, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // 1. جلسة الجهاز نفسه السابقة تُستبدل، فلا يستهلك الجهاز أكثر من خانة
            var activeSessions = await _sessionRepository.GetActiveByUserAsync(user.Id, cancellationToken);
            foreach (var existing in activeSessions.Where(s => s.DeviceId == deviceId))
            {
                existing.Revoke(SessionRevokeReason.ReplacedByNewLogin, now);
            }

            // 2. يُخزَّن Hash الـ Refresh Token فقط، ويُرسل للعميل بالشكل {sessionId}.{secret}
            var secret = _jwtProvider.GenerateRefreshToken();
            var session = LoginSession.Create(
                user.Id, deviceId, deviceName, TokenHasher.Hash(secret), now,
                SessionPolicy.RefreshLifetime, SessionPolicy.AbsoluteLifetime);

            _sessionRepository.Add(session);

            // 3. Access Token مرتبط بالجلسة
            var accessToken = await _jwtProvider.GenerateAccessTokenAsync(user, session.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. تنبيه الدخول الجديد في الخلفية
            var toEmail = user.Email!;
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendNewLoginAlertAsync(toEmail, deviceName, now));

            return new AuthResponseDto(accessToken, $"{session.Id:N}.{secret}", session.RefreshExpiresAt);
        }

        public async Task RevokeAllForUserAsync(Guid userId, SessionRevokeReason reason, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var activeSessions = await _sessionRepository.GetActiveByUserAsync(userId, cancellationToken);
            foreach (var session in activeSessions)
            {
                session.Revoke(reason, now);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}