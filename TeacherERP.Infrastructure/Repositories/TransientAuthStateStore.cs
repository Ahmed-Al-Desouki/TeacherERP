using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories;

public sealed class TransientAuthStateStore(AppDbContext context, TimeProvider timeProvider) : ITransientAuthStateStore
{
    public async Task SavePayloadAsync<T>(string purpose, string email, T value, TimeSpan lifetime, CancellationToken cancellationToken)
    {
        var state = await GetOrCreateAsync(purpose, email, cancellationToken);
        state.PayloadJson = JsonSerializer.Serialize(value);
        state.PayloadExpiresAtUtc = timeProvider.GetUtcNow().UtcDateTime.Add(lifetime);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<T?> GetPayloadAsync<T>(string purpose, string email, CancellationToken cancellationToken)
    {
        var state = await FindAsync(purpose, email, cancellationToken);
        if (state?.PayloadJson is null || state.PayloadExpiresAtUtc <= timeProvider.GetUtcNow().UtcDateTime)
            return default;
        return JsonSerializer.Deserialize<T>(state.PayloadJson);
    }

    public async Task RemovePayloadAsync(string purpose, string email, CancellationToken cancellationToken)
    {
        var state = await FindAsync(purpose, email, cancellationToken);
        if (state is null) return;
        state.PayloadJson = null;
        state.PayloadExpiresAtUtc = default;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveGrantAsync(string purpose, string email, string grantHash, DateTime expiresAtUtc, CancellationToken cancellationToken)
    {
        var state = await GetOrCreateAsync(purpose, email, cancellationToken);
        state.GrantHash = grantHash;
        state.GrantExpiresAtUtc = expiresAtUtc;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ValidateGrantAsync(string purpose, string email, string grantHash, CancellationToken cancellationToken)
    {
        var state = await FindAsync(purpose, email, cancellationToken);
        if (state?.GrantHash is null || state.GrantExpiresAtUtc <= timeProvider.GetUtcNow().UtcDateTime) return false;
        var expected = Encoding.UTF8.GetBytes(state.GrantHash);
        var actual = Encoding.UTF8.GetBytes(grantHash);
        return expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    public async Task RemoveGrantAsync(string purpose, string email, CancellationToken cancellationToken)
    {
        var state = await FindAsync(purpose, email, cancellationToken);
        if (state is null) return;
        state.GrantHash = null;
        state.GrantExpiresAtUtc = default;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteExpiredAsync(DateTime nowUtc, CancellationToken cancellationToken)
        => await context.TransientAuthStates
            .Where(x => (x.CodeExpiresAtUtc == default || x.CodeExpiresAtUtc <= nowUtc)
                && (x.IssueWindowEndsAtUtc == default || x.IssueWindowEndsAtUtc <= nowUtc)
                && (x.GrantExpiresAtUtc == default || x.GrantExpiresAtUtc <= nowUtc)
                && (x.PayloadExpiresAtUtc == default || x.PayloadExpiresAtUtc <= nowUtc))
            .ExecuteDeleteAsync(cancellationToken);

    private Task<TransientAuthState?> FindAsync(string purpose, string email, CancellationToken cancellationToken)
        => context.TransientAuthStates.SingleOrDefaultAsync(x => x.Purpose == purpose && x.Email == Normalize(email), cancellationToken);

    private async Task<TransientAuthState> GetOrCreateAsync(string purpose, string email, CancellationToken cancellationToken)
    {
        var state = await FindAsync(purpose, email, cancellationToken);
        if (state is not null) return state;
        state = new TransientAuthState { Purpose = purpose, Email = Normalize(email) };
        context.TransientAuthStates.Add(state);
        return state;
    }

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
