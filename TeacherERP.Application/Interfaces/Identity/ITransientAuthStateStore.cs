using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces;

public interface ITransientAuthStateStore
{
    Task SavePayloadAsync<T>(string purpose, string email, T value, TimeSpan lifetime, CancellationToken cancellationToken);
    Task<T?> GetPayloadAsync<T>(string purpose, string email, CancellationToken cancellationToken);
    Task RemovePayloadAsync(string purpose, string email, CancellationToken cancellationToken);
    Task SaveGrantAsync(string purpose, string email, string grantHash, DateTime expiresAtUtc, CancellationToken cancellationToken);
    Task<bool> ValidateGrantAsync(string purpose, string email, string grantHash, CancellationToken cancellationToken);
    Task RemoveGrantAsync(string purpose, string email, CancellationToken cancellationToken);
    Task<int> DeleteExpiredAsync(DateTime nowUtc, CancellationToken cancellationToken);
}
