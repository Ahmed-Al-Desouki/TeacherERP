using Hangfire;
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public LoginCommandHandler(
            IIdentityService identityService,
            IJwtProvider jwtProvider,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IBackgroundJobClient backgroundJobClient)
        {
            _identityService = identityService;
            _jwtProvider = jwtProvider;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من صحة بيانات الدخول واسترجاع المستخدم
            var userResult = await _identityService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);
            if (userResult.IsFailure)
            {
                return Result<AuthResponseDto>.Failure(userResult.Error);
            }

            var user = userResult.Value!;

            // 2. التحقق من حالة المستخدم (Business Rule)[cite: 3]
            if (user.Status != StudentStatus.Active)
            {
                return Result<AuthResponseDto>.Failure(new Error("Auth.AccountSuspended", "This account is suspended or blacklisted."));
            }

            // 3. توليد التوكنز
            var (accessToken, jwtId) = await _jwtProvider.GenerateAccessTokenAsync(user);
            var refreshTokenValue = _jwtProvider.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // 4. إنشاء كيان الـ Refresh Token عبر دالة المصنع[cite: 3]
            var refreshTokenEntity = RefreshToken.Create(
                userId: user.Id,
                token: refreshTokenValue,
                jwtId: jwtId,
                expiryDate: refreshTokenExpiry,
                deviceInfo: request.DeviceInfo
            );

            // 5. الحفظ في قاعدة البيانات عبر Unit of Work
            _refreshTokenRepository.Add(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. إرسال تنبيه الدخول الجديد في الخلفية لعدم تعطيل الـ API
            var loginTime = DateTime.UtcNow;
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendNewLoginAlertAsync(user.Email, request.DeviceInfo, loginTime));

            // 7. إرجاع النتيجة بنجاح
            var response = new AuthResponseDto(accessToken, refreshTokenValue, refreshTokenExpiry);
            return Result<AuthResponseDto>.Success(response);
        }
    }
}