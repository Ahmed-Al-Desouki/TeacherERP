using System.Security.Cryptography;
using System.Text;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Infrastructure.Services;

public sealed class ResetGrantService(ITransientAuthStateStore store, TimeProvider timeProvider) : IResetGrantService
{
    private static readonly TimeSpan GrantLifetime = TimeSpan.FromMinutes(10);

    public async Task<ResetGrant> IssueAsync(string email, CancellationToken cancellationToken)
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        await store.SaveGrantAsync("PasswordResetGrant", email, hash, timeProvider.GetUtcNow().UtcDateTime.Add(GrantLifetime), cancellationToken);
        return new ResetGrant(token, (int)GrantLifetime.TotalSeconds);
    }

    public Task<bool> IsValidAsync(string email, string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(token)) return Task.FromResult(false);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        return store.ValidateGrantAsync("PasswordResetGrant", email, hash, cancellationToken);
    }

    public Task RevokeAsync(string email, CancellationToken cancellationToken)
        => store.RemoveGrantAsync("PasswordResetGrant", email, cancellationToken);
}
