using Hangfire;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Infrastructure.Services
{
    public class AuthSessionService : IAuthSessionService
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AuthSessionService(
            IJwtProvider jwtProvider,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IBackgroundJobClient backgroundJobClient)
        {
            _jwtProvider = jwtProvider;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<AuthResponseDto> IssueAsync(ApplicationUser user, string deviceInfo, CancellationToken cancellationToken)
        {
            // 1. توليد التوكنز
            var (accessToken, jwtId) = await _jwtProvider.GenerateAccessTokenAsync(user);
            var refreshTokenValue = _jwtProvider.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // 2. إنشاء وحفظ الـ Refresh Token
            var refreshTokenEntity = RefreshToken.Create(
                userId: user.Id,
                token: refreshTokenValue,
                jwtId: jwtId,
                expiryDate: refreshTokenExpiry,
                deviceInfo: deviceInfo
            );

            _refreshTokenRepository.Add(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. إرسال تنبيه الدخول الجديد في الخلفية
            var loginTime = DateTime.UtcNow;
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendNewLoginAlertAsync(user.Email!, deviceInfo, loginTime));

            return new AuthResponseDto(accessToken, refreshTokenValue, refreshTokenExpiry);
        }
    }
}