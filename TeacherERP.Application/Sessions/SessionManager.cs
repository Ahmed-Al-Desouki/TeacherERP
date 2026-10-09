using Hangfire;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Sessions
{
    public class SessionManager : ISessionManager
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionValidator _sessionValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IUserTimeZoneAccessor _userTimeZoneAccessor;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<SessionManager> _logger;

        public SessionManager(
            IJwtProvider jwtProvider,
            ISessionRepository sessionRepository,
            ISessionValidator sessionValidator,
            IUnitOfWork unitOfWork,
            IBackgroundJobClient backgroundJobClient,
            IUserTimeZoneAccessor userTimeZoneAccessor,
            TimeProvider timeProvider,
            ILogger<SessionManager> logger)
        {
            _jwtProvider = jwtProvider;
            _sessionRepository = sessionRepository;
            _sessionValidator = sessionValidator;
            _unitOfWork = unitOfWork;
            _backgroundJobClient = backgroundJobClient;
            _userTimeZoneAccessor = userTimeZoneAccessor;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task<AuthResponseDto> IssueAsync(ApplicationUser user, string deviceId, string deviceName, CancellationToken cancellationToken)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var activeSessions = await _sessionRepository.GetActiveByUserAsync(user.Id, now, cancellationToken);

            // 1. جلسة الجهاز نفسه السابقة تُستبدل، فلا يستهلك الجهاز أكثر من خانة
            var replacedSessions = activeSessions.Where(s => s.DeviceId == deviceId).ToList();
            foreach (var existing in replacedSessions)
            {
                existing.Revoke(SessionRevokeReason.ReplacedByNewLogin, now);
            }

            // 2. حد الأجهزة: نترك خانة للجلسة الجديدة ونطرد الأقل استخداماً مؤخراً من الباقي
            var otherSessions = activeSessions.Where(s => s.DeviceId != deviceId).ToList();
            var evictedSessions = otherSessions
                .OrderBy(s => s.LastRefreshedAt)
                .Take(Math.Max(0, otherSessions.Count - (SessionPolicy.MaxActiveSessions - 1)))
                .ToList();

            foreach (var evicted in evictedSessions)
            {
                evicted.Revoke(SessionRevokeReason.DeviceLimit, now);
            }

            // 3. يُخزَّن Hash الـ Refresh Token فقط، ويُرسل للعميل بالشكل {sessionId}.{secret}
            var secret = _jwtProvider.GenerateRefreshToken();
            var session = LoginSession.Create(
                user.Id, deviceId, deviceName, TokenHasher.Hash(secret), now,
                SessionPolicy.RefreshLifetime, SessionPolicy.AbsoluteLifetime);

            _sessionRepository.Add(session);

            // 4. Access Token مرتبط بالجلسة
            var accessToken = await _jwtProvider.GenerateAccessTokenAsync(user, session.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. بعد الحفظ: مسح الكاش حتى تتوقف توكنات الجلسات الملغاة فوراً
            foreach (var revoked in replacedSessions.Concat(evictedSessions))
            {
                _sessionValidator.Invalidate(revoked.Id);
            }

            // 6. الإيميلات في الخلفية (الوقت UTC مع منطقة المستخدم ليُحوَّل عند الصياغة)
            var toEmail = user.Email!;
            var timeZoneId = _userTimeZoneAccessor.TimeZoneId;

            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendNewLoginAlertAsync(toEmail, deviceName, now, timeZoneId));

            foreach (var evicted in evictedSessions)
            {
                var evictedDeviceName = evicted.DeviceName;
                _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                    emailService.SendSessionEvictedAlertAsync(toEmail, evictedDeviceName, deviceName, now, timeZoneId));
            }

            return new AuthResponseDto(accessToken, $"{session.Id:N}.{secret}", session.RefreshExpiresAt);
        }

        public async Task<Result<AuthResponseDto>> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var invalidToken = Result<AuthResponseDto>.Failure(
                new Error("Auth.InvalidRefreshToken", "The refresh token is invalid or has expired. Please log in again."));

            // 1. تفكيك التوكن: {sessionId}.{secret}
            if (!TryParse(refreshToken, out var sessionId, out var secret))
            {
                return invalidToken;
            }

            var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session is null)
            {
                return invalidToken;
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var presentedHash = TokenHasher.Hash(secret);

            // 2. اكتشاف إعادة استخدام توكن قديم
            if (session.PreviousRefreshTokenHash is not null && TokenHasher.Matches(presentedHash, session.PreviousRefreshTokenHash))
            {
                if (session.IsActive(now) && now - session.LastRefreshedAt > SessionPolicy.ReuseGracePeriod)
                {
                    session.Revoke(SessionRevokeReason.TokenReuse, now);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _sessionValidator.Invalidate(session.Id);

                    _logger.LogWarning(
                        "Refresh token reuse detected. Session {SessionId} of user {UserId} was revoked.",
                        session.Id, session.UserId);
                }

                return invalidToken;
            }

            // 3. الجلسة لازم تكون نشطة والتوكن هو الحالي
            if (!session.IsActive(now) || !TokenHasher.Matches(presentedHash, session.RefreshTokenHash))
            {
                return invalidToken;
            }

            // 4. حالة المستخدم
            var user = session.User;
            if (user.Status != StudentStatus.Active)
            {
                return Result<AuthResponseDto>.Failure(
                    new Error("Auth.AccountSuspended", "This account is suspended or blacklisted."));
            }

            // 5. تبديل الـ Refresh Token وإصدار Access Token جديد (بالأدوار الحالية من الداتابيز)
            var newSecret = _jwtProvider.GenerateRefreshToken();
            session.Rotate(TokenHasher.Hash(newSecret), now, SessionPolicy.RefreshLifetime);

            var accessToken = await _jwtProvider.GenerateAccessTokenAsync(user, session.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthResponseDto>.Success(
                new AuthResponseDto(accessToken, $"{session.Id:N}.{newSecret}", session.RefreshExpiresAt));
        }

        public async Task<Result<bool>> RevokeByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var invalidToken = Result<bool>.Failure(
                new Error("Auth.InvalidRefreshToken", "The refresh token is invalid."));

            if (!TryParse(refreshToken, out var sessionId, out var secret))
            {
                return invalidToken;
            }

            var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session is null)
            {
                return invalidToken;
            }

            // حامل التوكن الحالي أو السابق هو صاحب الجلسة (السابق يسمح بالخروج لو ضاع التوكن الجديد بعد تجديد)
            var presentedHash = TokenHasher.Hash(secret);
            var isCurrent = TokenHasher.Matches(presentedHash, session.RefreshTokenHash);
            var isPrevious = session.PreviousRefreshTokenHash is not null
                             && TokenHasher.Matches(presentedHash, session.PreviousRefreshTokenHash);

            if (!isCurrent && !isPrevious)
            {
                return invalidToken;
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            // الجلسة المنتهية أصلاً: ننجح بدون تغيير (Idempotent)
            if (session.IsActive(now))
            {
                session.Revoke(SessionRevokeReason.UserLogout, now);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _sessionValidator.Invalidate(session.Id);
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RevokeSessionAsync(Guid userId, Guid sessionId, SessionRevokeReason reason, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);

            // لا نكشف وجود جلسة تخص مستخدماً آخر
            if (session is null || session.UserId != userId)
            {
                return Result<bool>.Failure(new Error("Session.NotFound", "The session was not found."));
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            // الجلسة المنتهية أصلاً: ننجح بدون تغيير (Idempotent)
            if (session.IsActive(now))
            {
                session.Revoke(reason, now);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _sessionValidator.Invalidate(session.Id);
            }

            return Result<bool>.Success(true);
        }

        public async Task RevokeAllForUserAsync(Guid userId, SessionRevokeReason reason, CancellationToken cancellationToken)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            var activeSessions = await _sessionRepository.GetActiveByUserAsync(userId, now, cancellationToken);
            foreach (var session in activeSessions)
            {
                session.Revoke(reason, now);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // بعد الحفظ: مسح الكاش حتى تتوقف الـ Access Tokens فوراً
            foreach (var session in activeSessions)
            {
                _sessionValidator.Invalidate(session.Id);
            }
        }

        private static bool TryParse(string token, out Guid sessionId, out string secret)
        {
            sessionId = Guid.Empty;
            secret = string.Empty;

            // الـ Guid بصيغة N طوله 32 حرف، ثم نقطة، ثم السر
            const int guidLength = 32;
            if (token.Length <= guidLength + 1 || token[guidLength] != '.')
            {
                return false;
            }

            if (!Guid.TryParseExact(token[..guidLength], "N", out sessionId))
            {
                return false;
            }

            secret = token[(guidLength + 1)..];
            return true;
        }
    }
}