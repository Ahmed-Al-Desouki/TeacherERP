using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Settings;

namespace TeacherERP.Infrastructure.Services;

public sealed class OtpService : IOtpService
{
    private const int MaxFailedAttempts = 5;
    private const int MaxCodesPerWindow = 3;
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan IssueWindow = TimeSpan.FromHours(1);
    private readonly AppDbContext _context;
    private readonly ILogger<OtpService> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly byte[] _pepper;

    public OtpService(AppDbContext context, ILogger<OtpService> logger, TimeProvider timeProvider, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _logger = logger;
        _timeProvider = timeProvider;
        // اشتقاق مفتاح منفصل للـ OTP؛ تدوير مفتاح JWT يبطل التحديات القصيرة فقط.
        _pepper = HMACSHA256.HashData(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret), Encoding.UTF8.GetBytes("TeacherERP:Otp:v1"));
    }

    public async Task<Result<OtpIssueResult>> GenerateAsync(OtpPurpose purpose, string identifier, CancellationToken cancellationToken)
    {
        var email = Normalize(identifier);
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var state = await FindOrCreateAsync(purpose, email, cancellationToken);

        if (state.CodeExpiresAtUtc > now)
        {
            if (state.IsLocked)
                return await RollbackFailure<OtpIssueResult>(transaction, "Otp.Locked", "Too many failed attempts. Please try again later.");
            if (state.ResendAvailableAtUtc > now)
            {
                var wait = (int)Math.Ceiling((state.ResendAvailableAtUtc - now).TotalSeconds);
                return await RollbackFailure<OtpIssueResult>(transaction, "Otp.ResendTooSoon", $"Please wait {wait} seconds before requesting a new code.");
            }
        }

        if (state.IssueWindowEndsAtUtc <= now)
        {
            state.IssueCount = 0;
            state.IssueWindowEndsAtUtc = now.Add(IssueWindow);
        }
        if (state.IssueCount >= MaxCodesPerWindow)
        {
            _logger.LogWarning("OTP hourly issue limit reached. Purpose {Purpose}", purpose);
            return await RollbackFailure<OtpIssueResult>(transaction, "Otp.HourlyLimit", "Too many verification codes were requested for this email. Please try again later.");
        }

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        state.OtpHash = HashCode(purpose, email, code);
        state.CodeExpiresAtUtc = now.Add(CodeLifetime);
        state.ResendAvailableAtUtc = now.Add(ResendCooldown);
        state.FailedAttempts = 0;
        state.IsLocked = false;
        state.IssueCount++;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result<OtpIssueResult>.Success(new OtpIssueResult(code, (int)CodeLifetime.TotalSeconds, (int)ResendCooldown.TotalSeconds));
    }

    public async Task<Result<bool>> VerifyAsync(OtpPurpose purpose, string identifier, string code, CancellationToken cancellationToken)
    {
        var email = Normalize(identifier);
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var state = await _context.TransientAuthStates.SingleOrDefaultAsync(x => x.Purpose == purpose.ToString() && x.Email == email, cancellationToken);
        if (state is null || state.OtpHash is null || state.CodeExpiresAtUtc <= now)
            return await RollbackFailure<bool>(transaction, "Otp.Expired", "The code has expired or was not requested. Please request a new one.");
        if (state.IsLocked)
            return await RollbackFailure<bool>(transaction, "Otp.TooManyAttempts", "Too many failed attempts. Please try again later.");

        var expected = Convert.FromHexString(state.OtpHash);
        var actual = Convert.FromHexString(HashCode(purpose, email, code));
        if (expected.Length != actual.Length || !CryptographicOperations.FixedTimeEquals(expected, actual))
        {
            state.FailedAttempts++;
            if (state.FailedAttempts >= MaxFailedAttempts)
            {
                state.IsLocked = true;
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return Result<bool>.Failure(new Error("Otp.TooManyAttempts", "Too many failed attempts. Please try again later."));
            }
            var remaining = MaxFailedAttempts - state.FailedAttempts;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result<bool>.Failure(new Error("Otp.Invalid", $"The verification code is invalid. {remaining} attempt(s) remaining."));
        }

        state.OtpHash = null;
        state.CodeExpiresAtUtc = default;
        state.ResendAvailableAtUtc = default;
        state.FailedAttempts = 0;
        state.IsLocked = false;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private async Task<TransientAuthState> FindOrCreateAsync(OtpPurpose purpose, string email, CancellationToken cancellationToken)
    {
        var state = await _context.TransientAuthStates.SingleOrDefaultAsync(x => x.Purpose == purpose.ToString() && x.Email == email, cancellationToken);
        if (state is not null) return state;
        state = new TransientAuthState { Purpose = purpose.ToString(), Email = email };
        _context.TransientAuthStates.Add(state);
        return state;
    }

    private async Task<Result<T>> RollbackFailure<T>(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction, string code, string message)
    {
        await transaction.RollbackAsync();
        return Result<T>.Failure(new Error(code, message));
    }

    private string HashCode(OtpPurpose purpose, string email, string code)
        => Convert.ToHexString(HMACSHA256.HashData(_pepper, Encoding.UTF8.GetBytes($"{purpose}:{email}:{code}")));

    private static string Normalize(string identifier) => identifier.Trim().ToLowerInvariant();
}
