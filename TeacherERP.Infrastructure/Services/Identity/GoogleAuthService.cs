using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Server;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
using TeacherERP.Infrastructure.Settings;

namespace TeacherERP.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleAuthSettings _settings;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(IOptions<GoogleAuthSettings> settings, ILogger<GoogleAuthService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<Result<GoogleUserInfo>> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken)
        {
            // حماية: بدون Audience محدد لن يتم فحص الـ aud نهائياً
            if (_settings.ClientIds.Count == 0)
            {
                _logger.LogError("GoogleAuthSettings:ClientIds is not configured.");
                return Result<GoogleUserInfo>.Failure(new Error("Auth.GoogleNotConfigured", "Google sign-in is not available."));
            }

            try
            {
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = _settings.ClientIds
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

                if (!payload.EmailVerified)
                {
                    return Result<GoogleUserInfo>.Failure(new Error("Auth.GoogleEmailNotVerified", "Your Google email is not verified."));
                }

                var fullName = string.IsNullOrWhiteSpace(payload.Name) ? payload.Email : payload.Name;
                return Result<GoogleUserInfo>.Success(new GoogleUserInfo(payload.Subject, payload.Email, fullName));
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Invalid Google ID token received.");
                return Result<GoogleUserInfo>.Failure(new Error("Auth.InvalidGoogleToken", "The Google token is invalid or expired."));
            }
        }
    }
}